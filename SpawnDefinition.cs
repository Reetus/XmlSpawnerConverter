using System;
using System.Collections.Generic;

namespace XmlSpawnerConverter
{
    public class SpawnDefinition
    {
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