using OpenMod.API.Commands;
using OpenMod.API.Users;
using OpenMod.Core.Users;
using Steamworks;
using System.Globalization;
using System.Threading.Tasks;

namespace BlazingFlame.UniversalBanList.Util;
public static class CommandUtility
{
    public static async Task<ulong> GetSteam64Async(this ICommandContext ctx, IUserManager userManager, int index)
    {
        string player = await ctx.Parameters.GetAsync<string>(index).ConfigureAwait(false);
        if (!ulong.TryParse(player, NumberStyles.Number, CultureInfo.InvariantCulture, out ulong id) || new CSteamID(id).GetEAccountType() != EAccountType.k_EAccountTypeIndividual)
        {
            IUser? user = await userManager.FindUserAsync(KnownActorTypes.Player, player, UserSearchMode.FindByName).ConfigureAwait(false);
            if (user != null)
                ulong.TryParse(user.Id, NumberStyles.Number, CultureInfo.InvariantCulture, out id);
        }

        return id;
    }
}
