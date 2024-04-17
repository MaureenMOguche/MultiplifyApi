﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Multiplify.Domain;
using Multiplify.Domain.Common;

namespace Multiplify.Infrastructure;
public class ApplicationDbContext(IHttpContextAccessor contextAccessor,
    IConfiguration config) : IdentityDbContext
{
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        var connectionString = config.GetConnectionString("DefaultConnection");

        optionsBuilder.UseSqlServer(connectionString);
        //Configure your DbContext here
        
        base.OnConfiguring(optionsBuilder);
    }
    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var entries = ChangeTracker.Entries<AuditableEntity>();
        var currentTime = DateTime.Now;
        var currentUser = contextAccessor.HttpContext?.User?.Identity?.Name;

        foreach (var entry in entries)
        {
            if (entry.State == EntityState.Added)
            {
                entry.Entity.CreatedBy = currentUser ?? "System";
                entry.Entity.CreatedOn = currentTime;
            }
            else if (entry.State == EntityState.Modified)
            {
                entry.Entity.LastModifiedBy = currentUser ?? "System";
                entry.Entity.LastModifiedOn = currentTime;
            }
        }
        return base.SaveChangesAsync(cancellationToken);
    }

    public DbSet<AppUser> User { get; set; }
    public DbSet<WaitList> WaitList { get; set; }
}