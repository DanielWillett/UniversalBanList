using BlazingFlame.UniversalBanList.API;
using BlazingFlame.UniversalBanList.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using OpenMod.API.Ioc;
using OpenMod.API.Prioritization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazingFlame.UniversalBanList.Database;

[PluginServiceImplementation(Lifetime = ServiceLifetime.Transient, Priority = Priority.Lowest)]
internal class BanListMySqlWhitelistService : IBanListWhitelistingService
{
    private readonly BanListContext m_DbContext;
    public BanListMySqlWhitelistService(BanListContext dbContex)
    {
        m_DbContext = dbContex;
    }

    public Task<bool> IsWhitelisted(ulong steam64) => m_DbContext.Whitelists.AnyAsync(x => x.Steam64 == steam64);
    public Task<bool> IsWhitelisted(ulong steam64, uint banId) => m_DbContext.Whitelists.AnyAsync(x => x.Steam64 == steam64 && x.BanId == banId);
    public async Task<bool> IsWhitelisted(ulong steam64, IEnumerable<uint> banIds)
    {
        List<UniversalBanWhitelist> whitelists = await m_DbContext.Whitelists.Where(x => x.Steam64 == steam64).ToListAsync();
        foreach (uint ban in banIds)
        {
            bool whitelisted = false;
            for (int j = 0; j < whitelists.Count; ++j)
            {
                if (whitelists[j].BanId == ban)
                {
                    whitelisted = true;
                    break;
                }
            }

            if (!whitelisted)
                return false;
        }

        return true;
    }

    public async Task<UniversalBanWhitelist> AddWhitelist(ulong steam64, uint banId)
    {
        // todo dont duplicate entries
        UniversalBanWhitelist wh = new UniversalBanWhitelist { BanId = banId, Steam64 = steam64, Timestamp = DateTimeOffset.UtcNow };
        m_DbContext.Whitelists.Add(wh);
        await m_DbContext.SaveChangesAsync(false);
        return wh;
    }
    public Task AddWhitelists(ulong steam64, IEnumerable<uint> banIds)
    {
        // todo dont duplicate entries
        DateTimeOffset now = DateTimeOffset.UtcNow;
        m_DbContext.Whitelists.AddRange(banIds.Select(x => new UniversalBanWhitelist { BanId = x, Steam64 = steam64, Timestamp = now }));
        return m_DbContext.SaveChangesAsync(false);
    }
    public Task<int> RemoveWhitelists(ulong steam64)
    {
        m_DbContext.Whitelists.RemoveRange(m_DbContext.Whitelists.Where(x => x.Steam64 == steam64));
        return m_DbContext.SaveChangesAsync(false);
    }
    public Task<int> RemoveWhitelists(ulong steam64, uint banId)
    {
        m_DbContext.Whitelists.RemoveRange(m_DbContext.Whitelists.Where(x => x.Steam64 == steam64 && x.BanId == banId));
        return m_DbContext.SaveChangesAsync(false);
    }
}
