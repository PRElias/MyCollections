using System;

namespace MyCollections.Models
{
    public class Game
    {
        public int GameID { get; set; }
        public string Name { get; set; }
        public string LogoURL { get; set; }
        public string Store { get; set; }
        public string System { get; set; }
        public bool Disabled { get; set; }
        public DateTime? BuyDate { get; set; } = new DateTime(2018, 1, 1);
        public float? Price { get; set; }
        public int? PlayedTime { get; set; }
        public bool Purchased { get; set; }
        public int? SteamApID { get; set; }
        public string SteamOriginalImageURL {get; set;}
        public int? IGDBId {get; set;}
        public bool Selected {get; set;}
    }
}
