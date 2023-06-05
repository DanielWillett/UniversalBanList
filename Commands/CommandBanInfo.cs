using BlazingFlame.UniversalBanList.API;
using BlazingFlame.UniversalBanList.Models;
using Cysharp.Threading.Tasks;
using OpenMod.API.Users;
using OpenMod.Core.Commands;
using OpenMod.Core.Users;
using OpenMod.Unturned.Commands;
using Steamworks;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using BlazingFlame.UniversalBanList.Util;
using Microsoft.Extensions.Localization;

namespace BlazingFlame.UniversalBanList.Commands;

[Command("ubans")]
[CommandDescription("View all universal bans for a player.")]
[CommandSyntax("<player>")]
public class CommandBanInfo : UnturnedCommand
{
    private readonly IStringLocalizer m_StringLocalizer;
    private readonly IBanListService m_BanList;
    private readonly IUserManager m_UserManager;
    public CommandBanInfo(IUserManager userManager, IStringLocalizer stringLocalizer, IServiceProvider serviceProvider, IBanListService banList) : base(serviceProvider)
    {
        m_StringLocalizer = stringLocalizer;
        m_UserManager = userManager;
        m_BanList = banList;
    }

    protected override async UniTask OnExecuteAsync()
    {
        ulong id = await Context.GetSteam64Async(m_UserManager, 0).ConfigureAwait(false);
        if (id == 0)
            await Context.Actor.PrintMessageAsync(m_StringLocalizer["commands:player_not_found"]);

        IReadOnlyCollection<UniversalBan> bans = await m_BanList.QueryBans(new ListBansQuery
        {
            Steam64 = id
        });
        if (bans.Count < 1)
        {
            await Context.Actor.PrintMessageAsync(m_StringLocalizer["commands:ubans_no_bans", new { Steam64 = id }]);
            return;
        }
        foreach (UniversalBan ban in bans)
        {
            await Context.Actor.PrintMessageAsync(m_StringLocalizer["commands:ubans_bans_entry", new { Ban = ban, Steam64 = id }]);
        }
    }
}