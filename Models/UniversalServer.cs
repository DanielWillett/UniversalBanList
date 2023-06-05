using Newtonsoft.Json;
using System;
using System.Text.Json.Serialization;

namespace BlazingFlame.UniversalBanList.Models;

public sealed class UniversalServer
{
    [JsonPropertyName("pk")]
    [JsonProperty("pk")]
    public uint Id { get; set; }

    [JsonPropertyName("display_name")]
    [JsonProperty("display_name")]
    public string? DisplayName { get; set; }

    [JsonPropertyName("last_ip")]
    [JsonProperty("last_ip")]
    public uint LastIP { get; set; }

    [JsonPropertyName("last_port")]
    [JsonProperty("last_port")]
    public ushort LastPort { get; set; }
    
    [JsonPropertyName("join_timestamp")]
    [JsonProperty("join_timestamp")]
    public DateTimeOffset JoinTimeStamp { get; set; }
    
    [JsonPropertyName("network_id")]
    [JsonProperty("network_id")]
    public uint NetworkId { get; set; }

    [System.Text.Json.Serialization.JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("network_detail")]
    [JsonProperty("network_detail", DefaultValueHandling = DefaultValueHandling.Ignore)]
    public UniversalNetwork? Network { get; set; }
    
    [JsonPropertyName("last_online_timestamp")]
    [JsonProperty("last_online_timestamp")]
    public DateTimeOffset? LastOnlineTimeStamp { get; set; }
}
