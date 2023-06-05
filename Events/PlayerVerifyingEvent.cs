using OpenMod.API.Eventing;
using OpenMod.Core.Eventing;
using SDG.Unturned;

namespace BlazingFlame.UniversalBanList.Events;
public class PlayerVerifyingEvent : Event, ICancellableEvent
{
    public SteamPending PendingPlayer { get; }
    public bool IsCancelled { get; set; }
    public ESteamRejection Rejection { get; set; } = ESteamRejection.PLUGIN;
    public string? Reason { get; set; } = null;

    public PlayerVerifyingEvent(SteamPending pendingPlayer)
    {
        PendingPlayer = pendingPlayer;
    }
}
