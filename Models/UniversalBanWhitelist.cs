using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using BlazingFlame.UniversalBanList.Database;
using Newtonsoft.Json;

namespace BlazingFlame.UniversalBanList.Models;

[Table("universal_ban_whitelists")]
public class UniversalBanWhitelist
{
    [Key]
    [Required]
    [JsonPropertyName("ban_id")]
    [JsonProperty("ban_id")]
    public uint BanId { get; set; }

    [Required]
    [JsonPropertyName("steam_64")]
    [JsonProperty("steam_64")]
    public ulong Steam64 { get; set; }

    [Required]
    [DefaultValue("UTC_TIMESTAMP()")]
    [JsonPropertyName("whitelist_timestamp")]
    [JsonProperty("whitelist_timestamp")]
    [TypeConverter(typeof(UtcDateTimeOffsetConverter))]
    public DateTimeOffset Timestamp { get; set; }
}