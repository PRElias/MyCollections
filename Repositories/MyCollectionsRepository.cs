using LiteDB;
using MyCollections.Models;
using System.Linq;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;

namespace MyCollections.Repositories
{
    public class MyCollectionsRepository
    {
        private readonly IConfiguration _config;
        private static LiteDatabase database;

        public MyCollectionsRepository(IConfiguration config)
        {
            database = new LiteDatabase("Database.db");
            _config = config;
        }

        public IEnumerable<Config> FindConfig()
        {
            var config = database.GetCollection<Config>().FindAll().ToList();
            if (config == null)
            {
                var newConfig = new Config();
                newConfig.Id = 1;
                newConfig.key = "SteamAPI";
                newConfig.value = "";
            }
            return config;
        }
        
        public bool Upsert(Config config)
        {
            return database.GetCollection<Config>().Upsert(config);
        }
    }
}
