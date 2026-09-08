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
        public int? SteamApID { get; set; }
        public string SteamOriginalImageURL { get; set; }
        public int? IGDBId { get; set; }
        public bool Selected { get; set; }
        public string FriendlyName
        {
            get
            {
                return this.Name.Replace("'", "").ToUpperInvariant();
            }
        }
    }
}

