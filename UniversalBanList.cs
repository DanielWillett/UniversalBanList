using BlazingFlame.UniversalBanList.API;
using BlazingFlame.UniversalBanList.Database;
using Cysharp.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using OpenMod.API.Plugins;
using OpenMod.EntityFrameworkCore.MySql.Extensions;
using OpenMod.Unturned.Plugins;
using SDG.NetTransport;
using SDG.Unturned;
using System;
using System.Reflection;

[assembly: PluginMetadata("uBanList", DisplayName = "Universal Ban List")]

namespace BlazingFlame.UniversalBanList;
public sealed class UniversalBanList : OpenModUnturnedPlugin
{
    private readonly IConfiguration m_Configuration;
    private readonly IStringLocalizer m_StringLocalizer;
    private readonly ILogger<UniversalBanList> m_Logger;
    private readonly IBanListEventListenerService m_EventListener;
    private readonly BanListContext m_DbContext;
    public readonly Action<ITransportConnection, string, uint>? NotifyBan;
    public bool UseMySQL { get; private set; }

    public UniversalBanList(BanListContext dbContext,
        IBanListEventListenerService eventListener,
        IConfiguration configuration,
        IStringLocalizer stringLocalizer,
        ILogger<UniversalBanList> logger,
        IServiceProvider serviceProvider) : base(serviceProvider)
    {
        m_EventListener = eventListener;
        m_DbContext = dbContext;
        m_Configuration = configuration;
        m_StringLocalizer = stringLocalizer;
        m_Logger = logger;
        try
        {
            MethodInfo? notifyBan = typeof(Provider).GetMethod("notifyBannedInternal", BindingFlags.NonPublic | BindingFlags.Static);
            if (notifyBan != null)
                NotifyBan = (Action<ITransportConnection, string, uint>)notifyBan.CreateDelegate(typeof(Action<ITransportConnection, string, uint>));
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Unable to get void Provider.notifyBannedInternal(ITransportConnection, string, uint) method. Ban strings may look a little different.");
        }
    }

    protected override async UniTask OnLoadAsync()
    {
        UseMySQL = m_Configuration.GetSection("database:use_mysql").Get<bool>();
        if (UseMySQL)
        {
            m_Logger.LogInformation("Migrating database...");
            await m_DbContext.Database.MigrateAsync();
        }
        else
        {
            m_Logger.LogInformation("Database migration skipped.");
        }
        CheckAPIConfig();
        m_EventListener.Subscribe();
        m_Logger.LogInformation("Hello World!");
    }

    protected override UniTask OnUnloadAsync()
    {
        m_EventListener.Unsubscribe();
        m_Logger.LogInformation(m_StringLocalizer["plugin_events:plugin_stop"]);
        return UniTask.CompletedTask;
    }
    
    private void CheckAPIConfig()
    {
        string? endPoint = m_Configuration["banlist_api:api_endpoints:query_bans:base_endpoint"];
        m_Logger.LogInformation("Endpoint: query_bans.");
        if (endPoint != null && Uri.TryCreate(endPoint, UriKind.Absolute, out Uri uri))
        {
            m_Logger.LogInformation(" End Point:    \"" + uri + "\".");
        }
        else
        {
            m_Logger.LogWarning($" Invalid or missing endpoint: {endPoint ?? "Unspecified"}.");
        }
    }
}

public class PluginContainerConfigurator : IPluginContainerConfigurator
{
    public void ConfigureContainer(IPluginServiceConfigurationContext context)
    {
        context.ContainerBuilder.AddMySqlDbContext<BanListContext>();
    }
}