using Microsoft.EntityFrameworkCore;
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

        public DbSet<System> System { get; set; }
        public DbSet<Store> Store { get; set; }
        public DbSet<Game> Game { get; set; }
        public DbSet<Book> Book { get; set; }
        public DbSet<Param> Param { get; set; }
    }

}
