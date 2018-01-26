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
                new System{Name="XBOX 360"},
                new System{Name="XBOX One"},
                new System{Name="Playstation 3"},
                new System{Name="Playstation 4"},
                new System{Name="PC"},
                new System{Name="Android"},
                new System{Name="iOS"},
                new System{Name="Wii"}
            };

            foreach (System s in System)
            {
                context.System.Add(s);
            }

            var Store = new Store[]
            {
                new Store{Name="Undefined"},
                new Store{Name="Steam"},
                new Store{Name="Origin"},
                new Store{Name="UPlay"},
                new Store{Name="HumbleBundle"},
                new Store{Name="GOG"},
                new Store{Name="Google Play"},
                new Store{Name="Apple Store"},
                new Store{Name="Amazon"},
                new Store{Name="PSN"},
                new Store{Name="Windows Store"}
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
                new Param { key = "steam-steamid", value = ""}
            };

            foreach (Param p in Param)
            {
                context.Param.Add(p);
            }

            context.SaveChanges();
        }
    }

}
