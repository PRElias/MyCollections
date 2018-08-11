using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MyCollections.Models
{
    public class Store
    {
        [Key]
        public int StoreID { get; set; }
        [Required]
        public string Name { get; set; }
        public string Logo { get; set; }
    }
}
