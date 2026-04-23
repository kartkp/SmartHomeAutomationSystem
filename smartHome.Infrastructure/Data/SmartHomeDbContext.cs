using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using smartHome.Core.Entities;

namespace smartHome.Infrastructure.Data;
public class SmartHomeDbContext : DbContext
{
    public SmartHomeDbContext(DbContextOptions options) : base(options) { }

    public DbSet<Device> Devices { get; set; }
    public DbSet<CommandHistory> CommandHistory { get; set; }
    public DbSet<DeviceGroup> DeviceGroups { get; set; }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Device>()
            .HasOne(d => d.Group)
            .WithMany(g => g.Devices)
            .HasForeignKey(d => d.GroupId);

        base.OnModelCreating(modelBuilder);
    }
}