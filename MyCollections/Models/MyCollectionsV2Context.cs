﻿using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MyCollections.Models
{
    public class MyCollectionsContext : DbContext
    {
        public MyCollectionsContext(DbContextOptions<MyCollectionsContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder
                .Query<GamesDistinctView>().ToView("GamesDistinct");
        }

        public DbSet<System> System { get; set; }
        public DbSet<Store> Store { get; set; }
        public DbSet<Game> Game { get; set; }
        public DbSet<Book> Book { get; set; }
        public DbSet<Param> Param { get; set; }
        public DbQuery<GamesDistinctView> GamesDistinctView { get; set; }
    }

}
