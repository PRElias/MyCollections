using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MyCollections.Models
{
    public class MyCollectionsContext : IdentityDbContext<User>
    {
        public MyCollectionsContext(DbContextOptions<MyCollectionsContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            //modelBuilder
            //    .Query<GamesDistinctView>().ToView("GamesDistinct");

            //modelBuilder
            //    .Query<GamesDetailsView>().ToView("GamesDetails");
        }

        public DbSet<System> System { get; set; }
        public DbSet<Store> Store { get; set; }
        public DbSet<Game> Game { get; set; }
        public DbSet<Book> Book { get; set; }
        public DbSet<Param> Param { get; set; }
        //public DbQuery<GamesDistinctView> GamesDistinctView { get; set; }
        //public DbQuery<GamesDetailsView> GamesDetailsView { get; set; }
        public DbSet<GameDetails> GameDetails { get; set; }

        public DbSet<User> User { get; set; }
    }

}
