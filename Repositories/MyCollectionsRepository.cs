using LiteDB;
using MyCollections.Models;
using System.Linq;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using System.IO;
using Newtonsoft.Json;
using JsonSerializer = Newtonsoft.Json.JsonSerializer;

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

        public Config GetAll()
        {
            var config = database.GetCollection<Config>().FindAll().ToList().FirstOrDefault();
            if (config == null)
            {
                config = new Config();
                config.Id = 1;
                config.steamId = "";
                config.steamKey = "";
            }
            return config;
        }
        public bool Upsert(Config config)
        {
            return database.GetCollection<Config>().Upsert(config);
        }

        public void SaveJson(List<Game> data)
        {
            using (StreamWriter file = File.CreateText(@"docs\games\games.json"))
            {
                JsonSerializer serializer = new JsonSerializer();
                serializer.Serialize(file, data.ToArray());
            }
        }
    }
}
