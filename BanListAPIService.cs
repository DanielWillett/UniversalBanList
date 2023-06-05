using BlazingFlame.UniversalBanList.API;
using BlazingFlame.UniversalBanList.Models;
using Cysharp.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OpenMod.API.Ioc;
using OpenMod.API.Plugins;
using OpenMod.API.Prioritization;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Threading.Tasks;
using UnityEngine.Networking;

namespace BlazingFlame.UniversalBanList;

[PluginServiceImplementation(Lifetime = ServiceLifetime.Transient, Priority = Priority.Lowest)]
internal sealed class BanListAPIService : IBanListService
{
    private static readonly JsonSerializerOptions SerializerOptions = new JsonSerializerOptions
    {
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
        AllowTrailingCommas = true,
        ReadCommentHandling = JsonCommentHandling.Skip
    };

    private readonly ILogger<BanListAPIService> m_Logger;
    private readonly IPluginAccessor<UniversalBanList> m_PluginAccessor;
    public BanListAPIService(ILogger<BanListAPIService> logger, IPluginAccessor<UniversalBanList> pluginAccessor)
    {
        m_Logger = logger;
        m_PluginAccessor = pluginAccessor;
    }

    private string GetEndpoint(string id)
    {
        string? baseEndPoint = null;
        if (m_PluginAccessor.Instance is { Configuration: { } config })
            baseEndPoint = config["banlist_api:api_endpoints:" + id + ":base_endpoint"];

        if (!string.IsNullOrEmpty(baseEndPoint))
            return baseEndPoint!;

        throw new InvalidOperationException("Plugin or " + id + " endpoint not initialized.");
    }
    public static string GetQuery(string baseEndPoint, in ListBansQuery query)
    {
        string GetExtra(in ListBansQuery q2)
        {
            return (q2.ResultsLimit.HasValue ? "&limit=" + q2.ResultsLimit.Value.ToString(CultureInfo.InvariantCulture) : string.Empty) +
                   (q2.ServerDetail ? "&server_detail" : string.Empty) +
                   (q2.NetworkDetail ? "&network_detail" : string.Empty) +
                   (q2.StaffDetail ? "&staff_detail" : string.Empty);
        }
        if (query.BanId.HasValue)
        {
            return baseEndPoint + "?id=" + query.BanId.Value.ToString(CultureInfo.InvariantCulture) + GetExtra(in query);
        }

        if (query.IsJustSteam64)
        {
            return baseEndPoint + "?steam64=" + query.Steam64!.Value.ToString(CultureInfo.InvariantCulture) + GetExtra(in query);
        }

        StringBuilder sb = new StringBuilder(baseEndPoint, baseEndPoint.Length + 16);
        bool init = false;

        void Append(string key, string? value)
        {
            if (!init)
            {
                sb.Append('?');
                init = true;
            }
            else sb.Append('&');
            sb.Append(key);
            if (value != null)
                sb.Append('=').Append(value);
        }

        if (query.Steam64.HasValue)
            Append("steam64", query.Steam64.Value.ToString(CultureInfo.InvariantCulture));
        if (query.StaffSteam64.HasValue)
            Append("admin64", query.StaffSteam64.Value.ToString(CultureInfo.InvariantCulture));
        if (query.StaffId.HasValue)
            Append("admin", query.StaffId.Value.ToString(CultureInfo.InvariantCulture));
        if (query.StaffIsConsole.HasValue)
            Append("admin_is_console", query.StaffIsConsole.Value ? "1" : "0");
        if (query.DiscordMessageId.HasValue)
            Append("message_id", query.DiscordMessageId.Value.ToString(CultureInfo.InvariantCulture));
        if (query.ResultsLimit is > 0)
            Append("limit", query.ResultsLimit.Value.ToString(CultureInfo.InvariantCulture));
        if (query.LowerBound.HasValue && query.UpperBound.HasValue)
        {
            DateTime lower = query.LowerBound.Value.UtcDateTime;
            DateTime upper = query.UpperBound.Value.UtcDateTime;
            if (upper < lower)
                (upper, lower) = (lower, upper);
            Append("before", lower.ToString("s", CultureInfo.InvariantCulture));
            Append("after", upper.ToString("s", CultureInfo.InvariantCulture));
        }
        else if (query.LowerBound.HasValue)
            Append("before", query.LowerBound.Value.UtcDateTime.ToString("s", CultureInfo.InvariantCulture));
        else if (query.UpperBound.HasValue)
            Append("after", query.UpperBound.Value.UtcDateTime.ToString("s", CultureInfo.InvariantCulture));

        if (query.CancelLowerBound.HasValue && query.CancelUpperBound.HasValue)
        {
            DateTime lower = query.CancelLowerBound.Value.UtcDateTime;
            DateTime upper = query.CancelUpperBound.Value.UtcDateTime;
            if (upper < lower)
                (upper, lower) = (lower, upper);
            Append("cancelled_before", lower.ToString("s", CultureInfo.InvariantCulture));
            Append("cancelled_after", upper.ToString("s", CultureInfo.InvariantCulture));
        }
        else if (query.CancelLowerBound.HasValue)
            Append("cancelled_before", query.CancelLowerBound.Value.UtcDateTime.ToString("s", CultureInfo.InvariantCulture));
        else if (query.CancelUpperBound.HasValue)
            Append("cancelled_after", query.CancelUpperBound.Value.UtcDateTime.ToString("s", CultureInfo.InvariantCulture));

        if (query.AppealLowerBound.HasValue && query.AppealUpperBound.HasValue)
        {
            DateTime lower = query.AppealLowerBound.Value.UtcDateTime;
            DateTime upper = query.AppealUpperBound.Value.UtcDateTime;
            if (upper < lower)
                (upper, lower) = (lower, upper);
            Append("appealed_before", lower.ToString("s", CultureInfo.InvariantCulture));
            Append("appealed_after", upper.ToString("s", CultureInfo.InvariantCulture));
        }
        else if (query.AppealLowerBound.HasValue)
            Append("appealed_before", query.AppealLowerBound.Value.UtcDateTime.ToString("s", CultureInfo.InvariantCulture));
        else if (query.AppealUpperBound.HasValue)
            Append("appealed_after", query.AppealUpperBound.Value.UtcDateTime.ToString("s", CultureInfo.InvariantCulture));

        if (query.IsAppealed.HasValue)
            Append("is_appealed", query.IsAppealed.Value ? "1" : "0");
        if (query.IsCancelled.HasValue)
            Append("is_cancelled", query.IsCancelled.Value ? "1" : "0");

        if (query.ServerDetail)
            Append("server_detail", null);
        if (query.NetworkDetail)
            Append("network_detail", null);
        if (query.StaffDetail)
            Append("staff_detail", null);


        return sb.ToString();
    }

    public Task<IReadOnlyCollection<UniversalBan>> QueryBans(in ListBansQuery query)
    {
        string q = GetQuery(GetEndpoint("query_bans"), in query);
        m_Logger.LogDebug("Querying bans @ \"" + q + "\".");
        return QueryBans(q, query.ServerDetail || query.NetworkDetail || query.StaffDetail).AsTask();
    }
    private static async UniTask<IReadOnlyCollection<UniversalBan>> QueryBans(string queryString, bool detail)
    {
        using UnityWebRequest webRequest = UnityWebRequest.Get(queryString);
        await webRequest.SendWebRequest();

        string response = webRequest.downloadHandler.text;

        List<UniversalBan>? bans = JsonSerializer.Deserialize<List<UniversalBan>>(response, SerializerOptions);

        if (detail)
        {
            if (bans is { Count: > 1 })
                UnifyBans(bans);
            else if (bans is { Count: 1 })
                UnifyBan(bans[0]);
        }
        return (IReadOnlyCollection<UniversalBan>?)bans?.AsReadOnly() ?? Array.Empty<UniversalBan>();
    }

    private static void UnifyBans(IReadOnlyList<UniversalBan> bans)
    {
        for (int i = bans.Count - 1; i >= 1; --i)
        {
            UniversalBan b1 = bans[i];
            for (int j = 0; j < i; ++j)
            {
                UniversalBan b2 = bans[j];
                if (b1.ServerId == b2.ServerId)
                {
                    b1.Server = b2.Server;
                    break;
                }
            }
            for (int j = 0; j < i; ++j)
            {
                UniversalBan b2 = bans[j];
                if (b1.StaffId == b2.StaffId)
                {
                    b1.Staff = b2.Staff;
                    break;
                }
            }
        }

        for (int i = 0; i < bans.Count; ++i)
            UnifyBan(bans[i]);
    }
    private static void UnifyBan(UniversalBan ban)
    {
        if (ban.Server != null && ban.Staff != null)
        {
            if (ban.Staff.ServerId == ban.Server.Id)
                ban.Staff.Server = ban.Server;
            if (ban.Server.Network != null && ban.Staff.NetworkId == ban.Server.NetworkId)
                ban.Staff.Network = ban.Server.Network;
        }
    }
}
