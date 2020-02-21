using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Server;
using Server.Mobiles;

namespace Server.Commands
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
				e.Mobile.SendMessage("Usage: GenerateSpawners filename");
				return;
			}

			string fullPath = "Spawns/" + e.Arguments[0].ToLower() + ".json";

			if ( !File.Exists( fullPath ) )
			{
				e.Mobile.SendMessage( $"Cannot find file {fullPath}..." );
				return;
			}

			e.Mobile.SendMessage($"Generating spawners for {e.Arguments[0].ToLower()}");

			using JsonTextReader jtr = new JsonTextReader(new StreamReader(fullPath));
			JsonSerializer serializer = JsonSerializer.Create();

			SpawnDefinition[] data = serializer.Deserialize<SpawnDefinition[]>( jtr );

			int spawnersCreated = 0;

			Stopwatch sw = new Stopwatch();
			sw.Start();
			int stepSize = data.Length / 10;
			foreach ( SpawnDefinition definition in data )
			{
				Map map = definition.Map switch
				{
					SpawnDefinition.MapId.Felucca => Map.Felucca,
					SpawnDefinition.MapId.Trammel => Map.Trammel,
					SpawnDefinition.MapId.Ilshenar => Map.Ilshenar,
					SpawnDefinition.MapId.Malas => Map.Malas,
					SpawnDefinition.MapId.Tokuno => Map.Tokuno,
					SpawnDefinition.MapId.TerMur => Map.TerMur,
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

				if ((spawnersCreated % stepSize) == 0)
					e.Mobile.SendMessage($"generation is {(int)Math.Ceiling((spawnersCreated / (float)data.Length) * 100f)}% complete");
			}

			sw.Stop();

			e.Mobile.SendMessage( $"{spawnersCreated} spawners generated in {sw.Elapsed}." );
		}

        private static void CheckLocation( ref Point3D location, Map map )
        {
            IPooledEnumerable<Spawner> spawners = map.GetItemsInRange<Spawner>( location, 0 );

            if ( !spawners.Any() )
				return;

			for ( int x = 0; x > -10; x-- )
			{
				for ( int y = 0; y > -10; y-- )
				{
					location.X += x;
					location.Y += y;
					location.Z = map.GetAverageZ( location.X, location.Y );

					spawners = map.GetItemsInRange<Spawner>( location, 0 );

					if ( spawners.Any() )
						continue;

					return;
				}
			}
		}
	}
}
