using BlazingFlame.UniversalBanList.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazingFlame.UniversalBanList.API;
public static class BanListAPIServiceEx
{
    public static bool TryGetActiveBan(this IReadOnlyCollection<UniversalBan> bans, out UniversalBan mostRecentBan)
    {
        foreach (UniversalBan ban in bans.OrderByDescending(x => x.TimeStamp))
        {
            if (ban is { IsCancelled: false, IsAppealed: false })
            {
                mostRecentBan = ban;
                return true;
            }
        }

        mostRecentBan = null!;
        return false;
    }
    public static async Task<UniversalBan?> GetBan(this IBanListService service, uint id, bool serverDetail = false, bool networkDetail = false, bool staffDetail = false)
    {
        IReadOnlyCollection<UniversalBan> bans = await service.QueryBans(
            new ListBansQuery
            {
                BanId = id,
                ServerDetail = serverDetail,
                NetworkDetail = networkDetail,
                StaffDetail = staffDetail
            }).ConfigureAwait(false);

        return bans.FirstOrDefault();
    }
}
