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

        // public IEnumerable<Param> FindAllParamns()
        // {
        //     var params = database.GetCollection<Param>().FindAll().ToList();
        //     return params;
        // }
        
        // public bool Upsert(Param params)
        // {
        //     return database.GetCollection<Param>().Upsert(params);
        // }
    }
}
