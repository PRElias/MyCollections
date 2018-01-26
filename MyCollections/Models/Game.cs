using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MyCollections.Models
{
    public class Game
    {
        [Key]
        public int GameID { get; set; }
        [Required]
        public string Name { get; set; }
        public string Cover { get; set; }
        public string Logo { get; set; }
        public int SystemID { get; set; }
        public System System { get; set; }
        public int StoreID { get; set; }
        public Store Store { get; set; }
        public DateTime BuyDate { get; set; } = new DateTime(2018, 1, 1);
        public float Price { get; set; }
        public int PlayedTime { get; set; }
        public bool Purchased { get; set; }
        public int SteamApID { get; set; }
        public bool Active { get; set; }
    }
}
