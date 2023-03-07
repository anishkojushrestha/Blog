﻿using Blog.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Blog.Data
{
    public class ApplicationDbContext:IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options):base(options)
        {

        }

        public DbSet<ApplicationUser>? applicationUsers { get; set; }
        public DbSet<Post>? posts { get; set; }
        public DbSet<Page>? pages { get; set; }
    }
}
