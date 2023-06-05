using Newtonsoft.Json;
using System;
using System.Text.Json.Serialization;

namespace BlazingFlame.UniversalBanList.Models;

public sealed class UniversalBan
{
    [JsonPropertyName("pk")]
    [JsonProperty("pk")]
    public uint Id { get; set; }
    
    [JsonPropertyName("banned_player_id")]
    [JsonProperty("banned_player_id")]
    public ulong Steam64 { get; set; }
    
    [JsonPropertyName("admin_is_console")]
    [JsonProperty("admin_is_console")]
    public bool StaffIsConsole { get; set; }
    
    [JsonPropertyName("admin")]
    [JsonProperty("admin")]
    public uint? StaffId { get; set; }
    
    [JsonPropertyName("admin_steam_id")]
    [JsonProperty("admin_steam_id")]
    public ulong? StaffSteam64 { get; set; }

    [JsonPropertyName("admin_detail")]
    [JsonProperty("admin_detail", DefaultValueHandling = DefaultValueHandling.Ignore)]
    [System.Text.Json.Serialization.JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public UniversalStaff? Staff { get; set; }

    [JsonPropertyName("server")]
    [JsonProperty("server")]
    public uint ServerId { get; set; }
    
    [JsonPropertyName("server_detail")]
    [JsonProperty("server_detail", DefaultValueHandling = DefaultValueHandling.Ignore)]
    [System.Text.Json.Serialization.JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public UniversalServer? Server { get; set; }
    
    [JsonPropertyName("cancelled")]
    [JsonProperty("cancelled")]
    public bool IsCancelled { get; set; }
    
    [JsonPropertyName("appealed")]
    [JsonProperty("appealed")]
    public bool IsAppealed { get; set; }
    
    [JsonPropertyName("timestamp")]
    [JsonProperty("timestamp")]
    public DateTimeOffset TimeStamp { get; set; }
    
    [JsonPropertyName("cancel_timestamp")]
    [JsonProperty("cancel_timestamp")]
    public DateTimeOffset? CancelTimeStamp { get; set; }
    
    [JsonPropertyName("appeal_timestamp")]
    [JsonProperty("appeal_timestamp")]
    public DateTimeOffset? AppealTimeStamp { get; set; }

    [JsonPropertyName("reason")]
    [JsonProperty("reason")]
    public string? Reason { get; set; }

    [JsonPropertyName("report_message_id")]
    [JsonProperty("report_message_id")]
    public ulong? ReportMessageId { get; set; }
}
