using Newtonsoft.Json;
using System;
using System.Text.Json.Serialization;

namespace BlazingFlame.UniversalBanList.Models;

public sealed class UniversalStaff
{
    [JsonPropertyName("pk")]
    [JsonProperty("pk")]
    public uint Id { get; set; }

    [JsonPropertyName("steam_id")]
    [JsonProperty("steam_id")]
    public ulong Steam64 { get; set; }

    [JsonPropertyName("network")]
    [JsonProperty("network")]
    public uint NetworkId { get; set; }

    [JsonPropertyName("network_detail")]
    [JsonProperty("network_detail", DefaultValueHandling = DefaultValueHandling.Ignore)]
    [System.Text.Json.Serialization.JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public UniversalNetwork? Network { get; set; }

    [JsonPropertyName("server")]
    [JsonProperty("server")]
    public uint? ServerId { get; set; }

    [JsonPropertyName("server_detail")]
    [JsonProperty("server_detail", DefaultValueHandling = DefaultValueHandling.Ignore)]
    [System.Text.Json.Serialization.JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public UniversalServer? Server { get; set; }
    
    [JsonPropertyName("player_name")]
    [JsonProperty("player_name")]
    public string? PlayerName { get; set; }
    
    [JsonPropertyName("hire_timestamp")]
    [JsonProperty("hire_timestamp")]
    public DateTimeOffset HiredTimeStamp { get; set; }

    [JsonPropertyName("unhire_timestamp")]
    [JsonProperty("unhire_timestamp")]
    public DateTimeOffset? UnhiredTimeStamp { get; set; }

    [JsonPropertyName("discord_id")]
    [JsonProperty("discord_id")]
    public ulong? DiscordId { get; set; }

    [JsonPropertyName("is_hired")]
    [JsonProperty("is_hired")]
    public bool IsHired { get; set; }
}
