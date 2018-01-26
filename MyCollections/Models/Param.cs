using System.ComponentModel.DataAnnotations;

namespace MyCollections.Models
{
    public class Param
    {
        [Key]
        public string key { get; set; }
        public string value { get; set; }
    }
}
