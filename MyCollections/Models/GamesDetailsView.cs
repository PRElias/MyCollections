using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyCollections.Models
{
    public class GamesDetailsView
    {
        public string Game { get; set; }
        public int StoreId { get; set; }
        public string Store { get; set; }
        public string StoreLogo { get; set; }
        public int SystemId { get; set; }
        public string System { get; set; }
        public string SystemLogo { get; set; }
        public string Summary {get; set;}
        public string Rating {get; set;}
        public string ReleaseDate {get; set;}
    }
}
