using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MyCollections.Models
{
    public class GameDetails
    {
        [Key]
        public int GameDetailsID { get; set; }
        public string Name { get; set; }
        public int? SteamApID { get; set; }
        public int? IGDBId {get; set;}
        public string IDDBData {get; set;}
        public DateTime DateUpdated { get; set; }
    }
}
