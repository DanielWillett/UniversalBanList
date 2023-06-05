using BlazingFlame.UniversalBanList.API;
using BlazingFlame.UniversalBanList.Util;
using Cysharp.Threading.Tasks;
using OpenMod.API.Users;
using OpenMod.Core.Commands;
using System;
using System.Drawing;
using Microsoft.Extensions.Localization;

namespace BlazingFlame.UniversalBanList.Commands;

[CommandParent(typeof(CommandUniversalWhitelist))]
[Command("remove")]
[CommandAlias("delete")]
[CommandDescription("Remove all whitelists from a player.")]
[CommandSyntax("<player>")]
public class CommandUniversalWhitelistRemove : CommandUniversalWhitelist
{
    public CommandUniversalWhitelistRemove(IBanListWhitelistingService whitelistingService, IStringLocalizer stringLocalizer, IBanListService banList, IUserManager userManager, IServiceProvider serviceProvider)
        : base(whitelistingService, stringLocalizer, banList, userManager, serviceProvider) { }

    protected override async UniTask OnExecuteAsync()
    {
        ulong id = await Context.GetSteam64Async(m_UserManager, 0);
        if (id == 0)
            await Context.Actor.PrintMessageAsync(m_StringLocalizer["commands:player_not_found"]);
        
        int c = await m_WhitelistingService.RemoveWhitelists(id);
        await Context.Actor.PrintMessageAsync(m_StringLocalizer["commands:ubanwhitelists_remove_success", new { Count = c, s = c == 1 ? string.Empty : "s", Steam64 = id }]);
    }
}
