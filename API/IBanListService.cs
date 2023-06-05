using BlazingFlame.UniversalBanList.Models;
using Cysharp.Threading.Tasks;
using OpenMod.API.Ioc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BlazingFlame.UniversalBanList.API;
[Service]
public interface IBanListService
{
    /// <remarks>Use <see cref="BanListAPIServiceEx.TryGetActiveBan"/> to properly check if any bans are applied.</remarks>
    /// <returns>A collection of bans applied to a player.</returns>
    /// <exception cref="InvalidOperationException">Plugin or endpoint not initialized.</exception>
    /// <exception cref="JsonException">Failed to read response.</exception>
    /// <exception cref="UnityWebRequestException">Request error from web API.</exception>
    Task<IReadOnlyCollection<UniversalBan>> QueryBans(in ListBansQuery query);
}

public struct ListBansQuery
{
    public uint? BanId { get; set; }
    public ulong? Steam64 { get; set; }
    public uint? StaffId { get; set; }
    public ulong? StaffSteam64 { get; set; }
    public bool? StaffIsConsole { get; set; }
    public DateTimeOffset? LowerBound { get; set; }
    public DateTimeOffset? UpperBound { get; set; }
    public DateTimeOffset? CancelLowerBound { get; set; }
    public DateTimeOffset? CancelUpperBound { get; set; }
    public DateTimeOffset? AppealLowerBound { get; set; }
    public DateTimeOffset? AppealUpperBound { get; set; }
    public uint? ServerId { get; set; }
    public bool? IsAppealed { get; set; }
    public bool? IsCancelled { get; set; }
    public ulong? DiscordMessageId { get; set; }
    public int? ResultsLimit { get; set; }
    public bool ServerDetail { get; set; }
    public bool NetworkDetail { get; set; }
    public bool StaffDetail { get; set; }

    public bool IsJustSteam64 => Steam64.HasValue &&
                                 !BanId.HasValue && !StaffId.HasValue && !StaffSteam64.HasValue &&
                                 !StaffIsConsole.HasValue &&
                                 !LowerBound.HasValue && !UpperBound.HasValue &&
                                 !CancelLowerBound.HasValue && !CancelUpperBound.HasValue &&
                                 !AppealLowerBound.HasValue && !AppealUpperBound.HasValue &&
                                 !ServerId.HasValue && !IsAppealed.HasValue && !IsCancelled.HasValue &&
                                 !DiscordMessageId.HasValue;
}