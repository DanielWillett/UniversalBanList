using BlazingFlame.UniversalBanList.Models;
using Microsoft.EntityFrameworkCore;
using OpenMod.EntityFrameworkCore;
using OpenMod.EntityFrameworkCore.Configurator;
using System;

namespace BlazingFlame.UniversalBanList.Database;
public sealed class BanListContext : OpenModDbContext<BanListContext>
{
    public DbSet<UniversalBanWhitelist> Whitelists { get; }

    public BanListContext(IServiceProvider serviceProvider) : base(serviceProvider)
    {
        Whitelists = Set<UniversalBanWhitelist>();
    }
    public BanListContext(IDbContextConfigurator configurator, IServiceProvider serviceProvider) : base(configurator, serviceProvider)
    {
        Whitelists = Set<UniversalBanWhitelist>();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<UniversalBanWhitelist>().HasKey(x => x.BanId);
    }
}
