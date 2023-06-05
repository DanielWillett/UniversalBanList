using System.Collections.Generic;
using BlazingFlame.UniversalBanList.Models;
using OpenMod.API.Ioc;
using System.Threading.Tasks;

namespace BlazingFlame.UniversalBanList.API;
[Service]
public interface IBanListWhitelistingService
{
    Task<bool> IsWhitelisted(ulong steam64);
    Task<bool> IsWhitelisted(ulong steam64, uint banId);
    Task<bool> IsWhitelisted(ulong steam64, IEnumerable<uint> banIds);
    Task<UniversalBanWhitelist> AddWhitelist(ulong steam64, uint banId);
    Task AddWhitelists(ulong steam64, IEnumerable<uint> banIds);
    Task<int> RemoveWhitelists(ulong steam64);
    Task<int> RemoveWhitelists(ulong steam64, uint banId);
}
