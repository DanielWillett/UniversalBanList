using BlazingFlame.UniversalBanList.API;
using BlazingFlame.UniversalBanList.Events;
using Cysharp.Threading.Tasks;
using HarmonyLib;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OpenMod.API;
using OpenMod.API.Eventing;
using OpenMod.API.Ioc;
using OpenMod.API.Plugins;
using OpenMod.Core.Helpers;
using SDG.Unturned;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Priority = OpenMod.API.Prioritization.Priority;

namespace BlazingFlame.UniversalBanList;

[PluginServiceImplementation(Lifetime = ServiceLifetime.Transient, Priority = Priority.Lowest)]
internal class UniversalBanListEventListener : IBanListEventListenerService
{
    private static bool _isSendVerifyPacketContinuation;
    private readonly ILogger<UniversalBanListEventListener> m_Logger;
    private readonly IEventBus m_EventBus;
    private readonly IOpenModHost m_openModHost;
    private readonly IPluginAccessor<UniversalBanList> m_PluginAccessor;
    public UniversalBanListEventListener(ILogger<UniversalBanListEventListener> logger, IPluginAccessor<UniversalBanList> pluginAccessor, IEventBus eventBus, IOpenModHost openModHost)
    {
        m_Logger = logger;
        m_EventBus = eventBus;
        m_openModHost = openModHost;
        m_PluginAccessor = pluginAccessor;
    }
    public void Subscribe()
    {
        Patches.Patch(m_Logger);
        Patches.OnStartVerifying += OnStartVerifyingPendingPlayer;
    }
    public void Unsubscribe()
    {
        Patches.Unpatch(m_Logger);
        Patches.OnStartVerifying -= OnStartVerifyingPendingPlayer;
    }
    private void OnStartVerifyingPendingPlayer(SteamPending player, ref bool shouldDeferContinuation)
    {
        PlayerVerifyingEvent @event = new PlayerVerifyingEvent(player);
        Task t = m_EventBus.EmitAsync(m_openModHost, null, @event);

        if (t.IsCompleted)
        {
            if (@event.IsCancelled)
            {
                m_Logger.LogDebug($"Removing player from already completed cancelled verify player event: {@event.PendingPlayer.playerID.playerName}.");
                shouldDeferContinuation = true;
                Patches.RemovePlayer(@event, m_PluginAccessor);
            }
            else
                m_Logger.LogDebug($"Allowing player to join from already completed verify player event: {@event.PendingPlayer.playerID.playerName}.");

            return;
        }

        shouldDeferContinuation = true;
        
        AsyncHelper.Schedule("UniversalBanList.VerifyPlayerEvent", async () =>
        {
            try
            {
                await t.ConfigureAwait(false);
                await UniTask.SwitchToMainThread();
                if (@event.IsCancelled)
                {
                    m_Logger.LogDebug($"Removing player from cancelled verify player event: {@event.PendingPlayer.playerID.playerName}.");
                    Patches.RemovePlayer(@event, m_PluginAccessor);
                }
                else
                {
                    m_Logger.LogDebug($"Allowing player to join from verify player event: {@event.PendingPlayer.playerID.playerName}.");
                    Patches.ContinueSendingVerifyPacket(player);
                }
            }
            catch
            {
                await UniTask.SwitchToMainThread();
                Patches.ContinueSendingVerifyPacket(player);
                throw;
            }
        });
    }
    
    private static class Patches
    {
        private const string HarmonyId = "blazingflame.universalbanlist";
        private static readonly Harmony _banListPatcher = new Harmony(HarmonyId);
        private static volatile int _isPatched;
        private static readonly List<ulong> _pendingPlayers = new List<ulong>();

        internal delegate void StartVerifying(SteamPending player, ref bool shouldDeferContinuation);
        internal static event StartVerifying? OnStartVerifying;


        internal static void Patch(ILogger logger)
        {
            if (Interlocked.Exchange(ref _isPatched, 1) != 0)
                return;

            MethodInfo? sendVerifyPacketMethod = GetStartVerifyPacket();
            if (sendVerifyPacketMethod == null)
            {
                logger.LogError("Unable to find method: void SteamPending.sendVerifyPacket()");
                return;
            }

            try
            {
                _banListPatcher.Patch(sendVerifyPacketMethod, prefix: new HarmonyMethod(new Func<SteamPending, bool>(PrefixSteamPlayerSendVerifyPacket).Method));
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Unable to patch method: void SteamPending.sendVerifyPacket() with bool UniversalBanListEventListener.Patches.PrefixSteamPlayerSendVerifyPacket(SteamPending).");
            }
        }
        internal static void Unpatch(ILogger logger)
        {
            if (Interlocked.Exchange(ref _isPatched, 0) == 0)
                return;

            try
            {
                MethodInfo? sendVerifyPacketMethod = GetStartVerifyPacket();
                if (sendVerifyPacketMethod != null)
                    _banListPatcher.Unpatch(sendVerifyPacketMethod, new Func<SteamPending, bool>(PrefixSteamPlayerSendVerifyPacket).Method);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Unable to unpatch method: void SteamPending.sendVerifyPacket().");
            }
        }
        private static bool PrefixSteamPlayerSendVerifyPacket(SteamPending __instance)
        {
            if (OnStartVerifying == null) return true; // plugin is unloaded

            // could technically recall this method while the verify event is running if another player gets verified.
            if (_pendingPlayers.Contains(__instance.playerID.steamID.m_SteamID))
                return false;

            if (_isSendVerifyPacketContinuation)
                return true;

            // stops the method invocation and queues it to be called after the async event is done
            bool shouldDeferContinuation = false;
            OnStartVerifying.Invoke(__instance, ref shouldDeferContinuation);
            if (shouldDeferContinuation)
                _pendingPlayers.Add(__instance.playerID.steamID.m_SteamID);
            return !shouldDeferContinuation;
        }
        internal static void RemovePlayer(PlayerVerifyingEvent @event, IPluginAccessor<UniversalBanList>? pluginAccessor)
        {
            _pendingPlayers.Remove(@event.PendingPlayer.playerID.steamID.m_SteamID);

            // 'method' is a delegate for Provider.notifyBannedInternal, which rejects a player for being banned, showing the classic banned message.
            if (@event.Rejection is ESteamRejection.AUTH_PUB_BAN or ESteamRejection.AUTH_VAC_BAN && pluginAccessor?.Instance?.NotifyBan is { } method)
                method.Invoke(@event.PendingPlayer.transportConnection, @event.Reason ?? string.Empty, SteamBlacklist.PERMANENT);
            else Provider.reject(@event.PendingPlayer.transportConnection, @event.Rejection, @event.Reason);
        }
        internal static void ContinueSendingVerifyPacket(SteamPending player)
        {
            _pendingPlayers.Remove(player.playerID.steamID.m_SteamID);

            // disconnected
            if (!player.transportConnection.TryGetIPv4Address(out _))
                return;

            _isSendVerifyPacketContinuation = true;
            try
            {
                player.sendVerifyPacket();
            }
            finally
            {
                _isSendVerifyPacketContinuation = false;
            }
        }

        private static MethodInfo? GetStartVerifyPacket() => typeof(SteamPending).GetMethod("sendVerifyPacket",
            BindingFlags.Instance | BindingFlags.Public, null, CallingConventions.Any, Array.Empty<Type>(), null);
    }
}
