using BlazingFlame.UniversalBanList.API;
using Microsoft.Extensions.Localization;
using OpenMod.API.Users;
using OpenMod.Core.Commands;
using System;

namespace BlazingFlame.UniversalBanList.Commands;

[CommandParent(typeof(CommandUniversalWhitelist))]
[Command("add")]
[CommandAlias("create")]
[CommandDescription("Add a whitelist to a player.")]
[CommandSyntax("<player>")]
public class CommandUniversalWhitelistAdd : CommandUniversalWhitelist
{
    public CommandUniversalWhitelistAdd(IBanListWhitelistingService whitelistingService, IStringLocalizer stringLocalizer, IBanListService banList, IUserManager userManager, IServiceProvider serviceProvider)
        : base(whitelistingService, stringLocalizer, banList, userManager, serviceProvider) { }
}
