using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Server;
using Server.Mobiles;

namespace XmlSpawnerConverter
{
    public class GenerateSpawners
    {
        public static void Initialize()
        {
            CommandSystem.Register( "GenerateSpawners", AccessLevel.Administrator, GenenerateSpawners_OnCommand );
        }

        private static void GenenerateSpawners_OnCommand( CommandEventArgs e )
        {
            if ( e.Arguments.Length == 0 )
            {
                e.Mobile.SendMessage( "Usage: GenerateSpawners filename.json" );
                return;
            }

            string fullPath = e.Arguments[0];

            if ( !File.Exists( fullPath ) )
            {
                e.Mobile.SendMessage( $"Cannot find file {fullPath}..." );
                return;
            }

            using JsonTextReader jtr = new JsonTextReader( new StreamReader( fullPath ) );

            JsonSerializer serializer = JsonSerializer.Create();

            SpawnDefinition[] data = serializer.Deserialize<SpawnDefinition[]>( jtr );

            int spawnersCreated = 0;

            Stopwatch sw = new Stopwatch();
            sw.Start();

            foreach ( SpawnDefinition definition in data )
            {
                Map map = definition.Map switch
                {
                    0 => Map.Felucca,
                    1 => Map.Trammel,
                    2 => Map.Ilshenar,
                    3 => Map.Malas,
                    4 => Map.Tokuno,
                    5 => Map.TerMur,
                    _ => throw new IndexOutOfRangeException()
                };

                Point3D location = new Point3D( definition.X, definition.Y, definition.Z );

                CheckLocation( ref location, map );

                Spawner spawner = new Spawner
                {
                    HomeRange = definition.HomeRange,
                    MinDelay = definition.MinDelay,
                    MaxDelay = definition.MaxDelay,
                    Running = false
                };

                foreach ( SpawnEntry entry in definition.Entries )
                {
                    spawner.AddEntry( entry.Name, entry.Probability, entry.MaxCount );
                }

                spawner.MoveToWorld( location, map );
                spawner.Count = definition.Count;
                spawner.Running = true;
                spawner.Respawn();

                spawnersCreated++;
            }

            sw.Stop();

            e.Mobile.SendMessage( $"{spawnersCreated} spawners generated in {sw.Elapsed}." );
        }

        private static void CheckLocation( ref Point3D location, Map map )
        {
            IPooledEnumerable<Spawner> spawners = map.GetItemsInRange<Spawner>( location, 0 );

            if ( !spawners.Any() )
            {
                return;
            }

            for ( int x = 0; x > -10; x-- )
            {
                for ( int y = 0; y > -10; y-- )
                {
                    location.X += x;
                    location.Y += y;
                    location.Z = map.GetAverageZ( location.X, location.Y );

                    spawners = map.GetItemsInRange<Spawner>( location, 0 );

                    if ( spawners.Any() )
                    {
                        continue;
                    }

                    return;
                }
            }
        }
    }
}