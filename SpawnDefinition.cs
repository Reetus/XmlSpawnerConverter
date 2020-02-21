using System;
using System.Collections.Generic;

namespace Server.Commands
{
	public class SpawnDefinition
	{
		public static class MapId
		{
			public const int Felucca = 0;
			public const int Trammel = 1;
			public const int Ilshenar = 2;
			public const int Malas = 3;
			public const int Tokuno = 4;
			public const int TerMur = 5;
		}
		public int Count { get; set; }
		public List<SpawnEntry> Entries { get; set; }
		public int HomeRange { get; set; }
		public int Map { get; set; }
		public TimeSpan MaxDelay { get; set; }
		public TimeSpan MinDelay { get; set; }
		public int X { get; set; }
		public int Y { get; set; }
		public int Z { get; set; }
	}

	public class SpawnEntry
	{
		public int MaxCount { get; set; }
		public string Name { get; set; }
		public int Probability { get; set; }
	}
}