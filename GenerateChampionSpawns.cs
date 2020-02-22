using System.Collections.Generic;
using System.Linq;
using Server.Engines.CannedEvil;

namespace Server.Commands
{
    public class GenerateChampionSpawns
    {
        private static readonly ChampionSpawnDefinition[] _definitions =
        {
            // Deceit
            new ChampionSpawnDefinition
            {
                Map = Map.Felucca,
                X = 5178,
                Y = 708,
                Z = 20,
                Radius = 60,
                Type = ChampionSpawnType.UnholyTerror,
                RandomizeType = false
            },
            // Despise
            new ChampionSpawnDefinition
            {
                Map = Map.Felucca,
                X = 5557,
                Y = 824,
                Z = 65,
                Radius = 65,
                Type = ChampionSpawnType.VerminHorde,
                RandomizeType = false
            },
            // Destard
            new ChampionSpawnDefinition
            {
                Map = Map.Felucca,
                X = 5259,
                Y = 837,
                Z = 61,
                Radius = 75,
                Type = ChampionSpawnType.ColdBlood,
                RandomizeType = false
            },
            // Semidar
            new ChampionSpawnDefinition
            {
                Map = Map.Felucca,
                X = 5814,
                Y = 1350,
                Z = 2,
                Radius = 50,
                Type = ChampionSpawnType.Abyss,
                RandomizeType = false
            },
            // Terra Keep
            new ChampionSpawnDefinition
            {
                Map = Map.Felucca,
                X = 5190,
                Y = 1605,
                Z = 20,
                Radius = 50,
                Type = ChampionSpawnType.Arachnid,
                RandomizeType = false
            },
            // Desert T2A
            new ChampionSpawnDefinition
            {
                Map = Map.Felucca,
                X = 5636,
                Y = 2916,
                Z = 37,
                Radius = 30,
                RandomizeType = true
            },
            // Tortoise T2A
            new ChampionSpawnDefinition
            {
                Map = Map.Felucca,
                X = 5724,
                Y = 3991,
                Z = 42,
                Radius = 20,
                RandomizeType = true
            },
            // Ice West T2A
            new ChampionSpawnDefinition
            {
                Map = Map.Felucca,
                X = 5511,
                Y = 2360,
                Z = 40,
                Radius = 35,
                RandomizeType = true
            },
            // Oasis T2A
            new ChampionSpawnDefinition
            {
                Map = Map.Felucca,
                X = 5549,
                Y = 2640,
                Z = 15,
                Radius = 25,
                RandomizeType = true
            },
            // Terra Sanctum T2A
            new ChampionSpawnDefinition
            {
                Map = Map.Felucca,
                X = 6035,
                Y = 2944,
                Z = 52,
                Radius = 35,
                RandomizeType = true
            },
            // Lord Oaks T2A
            new ChampionSpawnDefinition
            {
                Map = Map.Felucca,
                X = 5559,
                Y = 3757,
                Z = 21,
                Radius = 18,
                Type = ChampionSpawnType.ForestLord,
                RandomizeType = false
            },
            // Marble T2A
            new ChampionSpawnDefinition
            {
                Map = Map.Felucca,
                X = 5267,
                Y = 3171,
                Z = 104,
                Radius = 18,
                RandomizeType = true
            },
            // Hopper's Bog T2A
            new ChampionSpawnDefinition
            {
                Map = Map.Felucca,
                X = 5954,
                Y = 3475,
                Z = 25,
                Radius = 50,
                RandomizeType = true
            },
            // Khaldun T2A
            new ChampionSpawnDefinition
            {
                Map = Map.Felucca,
                X = 5982,
                Y = 3882,
                Z = 20,
                Radius = 35,
                RandomizeType = true
            },
            // Ice East T2A
            new ChampionSpawnDefinition
            {
                Map = Map.Felucca,
                X = 6038,
                Y = 2400,
                Z = 46,
                Radius = 20,
                RandomizeType = true
            },
            // Damwin T2A
            new ChampionSpawnDefinition
            {
                Map = Map.Felucca,
                X = 5281,
                Y = 3368,
                Z = 51,
                Radius = 25,
                RandomizeType = true
            },
            // City of Dead T2A
            new ChampionSpawnDefinition
            {
                Map = Map.Felucca,
                X = 5207,
                Y = 3637,
                Z = 20,
                Radius = 50,
                RandomizeType = true
            },
            // Valor Ilshenar
            new ChampionSpawnDefinition
            {
                Map = Map.Ilshenar,
                X = 382,
                Y = 328,
                Z = -30,
                Radius = 100,
                RandomizeType = true
            },
            // Humility Ilshenar
            new ChampionSpawnDefinition
            {
                Map = Map.Ilshenar,
                X = 462,
                Y = 926,
                Z = -67,
                Radius = 100,
                RandomizeType = true
            },
            // Spirituality Ilshenar
            new ChampionSpawnDefinition
            {
                Map = Map.Ilshenar,
                X = 1645,
                Y = 1107,
                Z = 8,
                Radius = 50,
                Type = ChampionSpawnType.ForestLord,
                RandomizeType = false
            },
            // Glade Ilshenar
            new ChampionSpawnDefinition
            {
                Map = Map.Ilshenar,
                X = 2212,
                Y = 1260,
                Z = 25,
                Radius = 25,
                Type = ChampionSpawnType.Glade,
                RandomizeType = false
            },
            // Sleeping Dragon Tokuno
            new ChampionSpawnDefinition
            {
                Map = Map.Tokuno,
                X = 948,
                Y = 434,
                Z = 29,
                Radius = 60,
                Type = ChampionSpawnType.SleepingDragon,
                RandomizeType = false
            }
        };

        public static void Initialize()
        {
            CommandSystem.Register( "GenerateChampionSpawns", AccessLevel.Administrator,
                GenerateChampionSpawns_OnCommand );
        }

        private static void GenerateChampionSpawns_OnCommand( CommandEventArgs e )
        {
            foreach ( ChampionSpawnDefinition definition in _definitions )
            {
                // Remove existing
                IEnumerable<Item> existing = definition.Map
                    .GetItemsInRange( new Point3D( definition.X, definition.Y, definition.Z ), 0 )
                    .Where( i => i is ChampionSpawn );

                foreach ( Item item in existing )
                {
                    item.Delete();
                }

                ChampionSpawn spawn = new ChampionSpawn
                {
                    Active = false,
                    Type = definition.RandomizeType
                        ? Utility.RandomList( new[]
                        {
                            ChampionSpawnType.VerminHorde, ChampionSpawnType.Abyss, ChampionSpawnType.Arachnid,
                            ChampionSpawnType.ColdBlood, ChampionSpawnType.UnholyTerror
                        } )
                        : definition.Type,
                    SpawnSzMod = 0,
                    SpawnArea = new Rectangle2D(
                        new Point2D( definition.X - definition.Radius / 2, definition.Y - definition.Radius / 2 ),
                        new Point2D( definition.X + definition.Radius / 2, definition.Y + definition.Radius / 2 ) ),
                    RandomizeType = definition.RandomizeType
                };

                spawn.MoveToWorld( new Point3D( definition.X, definition.Y, definition.Z ), definition.Map );
                spawn.Active = true;
            }
        }

        private class ChampionSpawnDefinition
        {
            public Map Map { get; set; }
            public int Radius { get; set; }
            public bool RandomizeType { get; set; }
            public ChampionSpawnType Type { get; set; }
            public int X { get; set; }
            public int Y { get; set; }
            public int Z { get; set; }
        }
    }
}