using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MyCollections.Models
{
    public class GameDetails
    {
        [Key]
        public int GameDetailsID { get; set; }
        public string FriendlyName { get; set; }
        public string Name { get; set; }
        public int? SteamApID { get; set; }
        public int? IGDBId { get; set; }
        public string IGDBUrl { get; set; }
        public string ExophaseUrl { get; set; }
        public string Summary { get; set; }
        public string Storyline { get; set; }
        public DateTime? FirstReleaseDate { get; set; }
        public List<string> Genres { get; set; } = new List<string>();
        public List<string> Developers { get; set; } = new List<string>();
        public List<string> Publishers { get; set; } = new List<string>();
        public double? TotalRating { get; set; }
        public string IDDBData { get; set; }
        public DateTime DateUpdated { get; set; }
    }
}

