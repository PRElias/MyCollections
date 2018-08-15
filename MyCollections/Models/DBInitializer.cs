using System.Linq;

namespace MyCollections.Models
{
    public static class DBInitializer
    {
        public static void Initialize(MyCollectionsContext context)
        {
            context.Database.EnsureCreated();

            if (context.System.Any() || context.Store.Any())
            {
                return;
            }

            var System = new System[]
            {
                new System{Name="Undefined"},
                new System{Name="XBOX 360", Logo="https://banner2.kisspng.com/20180330/gbe/kisspng-black-xbox-360-xbox-one-logo-xbox-5abe7f19a78ce4.8747049115224338176863.jpg"},
                new System{Name="XBOX One", Logo="https://upload.wikimedia.org/wikipedia/commons/thumb/f/f9/Xbox_one_logo.svg/1024px-Xbox_one_logo.svg.png"},
                new System{Name="Playstation 3", Logo="https://upload.wikimedia.org/wikipedia/commons/c/c6/PS3.svg"},
                new System{Name="Playstation 4", Logo="https://cdn.worldvectorlogo.com/logos/playstation-4.svg"},
                new System{Name="PC", Logo="https://www.shareicon.net/download/2015/12/26/693394_monitor.svg"},
                new System{Name="Android", Logo="https://image.flaticon.com/icons/svg/61/61120.svg"},
                new System{Name="iOS", Logo="https://www.shareicon.net/download/2016/08/01/639906_apple.svg"},
                new System{Name="Wii", Logo="https://upload.wikimedia.org/wikipedia/commons/b/bc/Wii.svg"}
            };

            foreach (System s in System)
            {
                context.System.Add(s);
            }

            var Store = new Store[]
            {
                new Store{Name="Undefined"},
                new Store{Name="Steam", Logo="https://cdn.worldvectorlogo.com/logos/steam-1.svg"},
                new Store{Name="Origin", Logo="https://cdn.freebiesupply.com/logos/large/2x/origin-1-logo-svg-vector.svg"},
                new Store{Name="UPlay", Logo="https://banner2.kisspng.com/20180423/uyw/kisspng-uplay-ubisoft-computer-icons-video-game-wii-u-passport-vector-5ade665595bc49.3714998115245246296133.jpg"},
                new Store{Name="HumbleBundle", Logo="https://humblebundle-a.akamaihd.net/static/hashed/5d84c488c1df9afdfbcb0846f137e90a24c3f6cb.svg"},
                new Store{Name="GOG", Logo="https://upload.wikimedia.org/wikipedia/commons/3/3c/Gog-logo-2014.svg"},
                new Store{Name="Google Play", Logo="https://cdn4.iconfinder.com/data/icons/free-colorful-icons/360/google_play.png"},
                new Store{Name="Apple Store", Logo="https://upload.wikimedia.org/wikipedia/commons/f/fa/Apple_logo_black.svg"},
                new Store{Name="Amazon", Logo="https://vignette.wikia.nocookie.net/logopedia/images/6/62/Amazon.com_Favicon.svg/revision/latest?cb=20160808095333"},
                new Store{Name="PSN", Logo="https://logodownload.org/wp-content/uploads/2017/05/playstation-4-logo-ps4-7.png"},
                new Store{Name="Windows Store", Logo="https://image.flaticon.com/icons/svg/25/25412.svg"}
            };
            
            foreach (Store s in Store)
            {
                context.Store.Add(s);
            }

            //Salva as duas primeiras entidades para gerar as chaves primárias
            context.SaveChanges();

            var Game = new Game[]
            {
                new Game{Name="Teste", SystemID=1, StoreID=1}
            };

            foreach (Game g in Game)
            {
                context.Game.Add(g);
            }

            context.SaveChanges();

            var Param = new Param[]
            {
                new Param { key = "steam-key", value = ""},
                new Param { key = "steam-steamid", value = ""},
                new Param { key = "igdb-key", value = ""}
            };

            foreach (Param p in Param)
            {
                context.Param.Add(p);
            }

            context.SaveChanges();
        }
    }

}
