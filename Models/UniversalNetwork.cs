using Newtonsoft.Json;
using System;
using System.Text.Json.Serialization;

namespace BlazingFlame.UniversalBanList.Models;

public class UniversalNetwork
{
    [JsonPropertyName("pk")]
    [JsonProperty("pk")]
    public uint Id { get; set; }
    
    [JsonPropertyName("display_name")]
    [JsonProperty("display_name")]
    public string? DisplayName { get; set; }

    [JsonPropertyName("owner")]
    [JsonProperty("owner")]
    public ulong? Owner { get; set; }

    [JsonPropertyName("owner_discord")]
    [JsonProperty("owner_discord")]
    public ulong? OwnerDiscord { get; set; }

    [JsonPropertyName("join_timestamp")]
    [JsonProperty("join_timestamp")]
    public DateTimeOffset JoinTimeStamp { get; set; }
}
