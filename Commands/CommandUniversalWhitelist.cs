using BlazingFlame.UniversalBanList.API;
using BlazingFlame.UniversalBanList.Models;
using BlazingFlame.UniversalBanList.Util;
using Cysharp.Threading.Tasks;
using OpenMod.API.Users;
using OpenMod.Core.Commands;
using OpenMod.Unturned.Commands;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Microsoft.Extensions.Localization;

namespace BlazingFlame.UniversalBanList.Commands;

[Command("ubanwhitelist")]
[CommandAlias("uwhitelist")]
[CommandAlias("uwl")]
[CommandDescription("Add or remove a whitelist from a player.")]
[CommandSyntax("<player|add|remove>")]
public class CommandUniversalWhitelist : UnturnedCommand
{
    protected readonly IBanListWhitelistingService m_WhitelistingService;
    protected readonly IUserManager m_UserManager;
    protected readonly IBanListService m_BanList;
    protected readonly IStringLocalizer m_StringLocalizer;
    public CommandUniversalWhitelist(IBanListWhitelistingService whitelistingService, IStringLocalizer stringLocalizer, IBanListService banList, IUserManager userManager, IServiceProvider serviceProvider) : base(serviceProvider)
    {
        m_StringLocalizer = stringLocalizer;
        m_WhitelistingService = whitelistingService;
        m_UserManager = userManager;
        m_BanList = banList;
    }
    protected override async UniTask OnExecuteAsync()
    {
        ulong id = await Context.GetSteam64Async(m_UserManager, 0);
        if (id == 0)
        {
            await Context.Actor.PrintMessageAsync(m_StringLocalizer["commands:player_not_found"]);
            return;
        }

        IReadOnlyCollection<UniversalBan> bans = await m_BanList.QueryBans(new ListBansQuery
        {
            Steam64 = id
        });
        await m_WhitelistingService.AddWhitelists(id, bans.Select(x => x.Id));
        await Context.Actor.PrintMessageAsync(m_StringLocalizer["commands:ubanwhitelists_add_success", new { BanCount = bans.Count, s = bans.Count == 1 ? string.Empty : "s", Steam64 = id }]);
    }
}
