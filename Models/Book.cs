using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MyCollections.Models
{
    public class Book
    {
        [Key]
        public int BookID { get; set; }
        [Required]
        public string Name { get; set; }
        public string Author { get; set; }
        public bool EBook { get; set; }
        public float Price { get; set; }
        public DateTime BuyDate { get; set; } = new DateTime(2018, 1, 1);
        public bool Active { get; set; }
    }
}
