using BlazingFlame.UniversalBanList.API;
using BlazingFlame.UniversalBanList.Events;
using BlazingFlame.UniversalBanList.Models;
using Microsoft.Extensions.Logging;
using OpenMod.API.Eventing;
using SDG.Unturned;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazingFlame.UniversalBanList.Listeners;
public class PlayerVerifyingEventListener : IEventListener<PlayerVerifyingEvent>
{
    private readonly IBanListWhitelistingService m_WhitelistingService;
    private readonly IBanListService m_BanListService;
    private readonly ILogger m_Logger;
    public PlayerVerifyingEventListener(ILogger<PlayerVerifyingEventListener> logger, IBanListService banListService, IBanListWhitelistingService whitelistingService)
    {
        m_Logger = logger;
        m_BanListService = banListService;
        m_WhitelistingService = whitelistingService;
    }
    public async Task HandleEventAsync(object? sender, PlayerVerifyingEvent @event)
    {
        ulong steam64 = @event.PendingPlayer.playerID.steamID.m_SteamID;
        IReadOnlyCollection<UniversalBan> bans = await m_BanListService.QueryBans(new ListBansQuery { Steam64 = steam64 }).ConfigureAwait(false);
        if (bans.Count == 0)
            return;
        UniversalBan[] newBans = bans.Where(x => !x.IsAppealed && !x.IsCancelled).ToArray();
        m_Logger.LogDebug($"Found bans for player: {bans.Count} ({newBans.Length} active).");

        if (newBans.Length == 0 || await m_WhitelistingService.IsWhitelisted(steam64, newBans.Select(x => x.Id)))
        {
            if (newBans.Length != 0)
                m_Logger.LogDebug("Whitelisted, allowing.");
            return;
        }

        @event.IsCancelled = true;
        @event.Rejection = ESteamRejection.AUTH_PUB_BAN;

        if (newBans.OrderByDescending(x => x.TimeStamp).FirstOrDefault()?.Reason is { Length: > 0 } reason)
            @event.Reason = "Universal Ban: " + reason;
        else
            @event.Reason = "Universal Ban";
    }
}
