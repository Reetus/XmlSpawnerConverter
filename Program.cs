using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;
using Server.Mobiles;

namespace XmlSpawnerConverter
{
    internal class Program
    {
        public enum Map
        {
            Felucca,
            Trammel,
            Ilshenar,
            Malas,
            Tokuno,
            TerMur
        }

        private const string INPUT_DIRECTORY = @"..\Spawns\";
        private const string OUTPUT_DIRECTORY = @"..\JSON\";

        private static void Main( string[] args )
        {
            IEnumerable<string> files = Directory.EnumerateFiles( INPUT_DIRECTORY, "*.xml" );
            //string[] files = { "Malas.xml" };

            foreach ( string file in files )
            {
                if ( !File.Exists( file ) )
                {
                    continue;
                }

                Console.WriteLine( $"Processing {file}..." );
                using FileStream reader = new FileStream( file, FileMode.Open );
                ParseXmlSpawn( reader, Path.GetFileNameWithoutExtension( file ) );
            }
        }

        private static void ParseXmlSpawn( Stream reader, string baseFileName )
        {
            List<SpawnDefinition> spawnDefinitions = new List<SpawnDefinition>();

            DataSet ds = new DataSet( "Spawns" );

            ds.ReadXml( reader );

            if ( ds.Tables.Count == 0 )
            {
                return;
            }

            foreach ( DataRow row in ds.Tables["Points"].Rows )
            {
                SpawnDefinition spawn = new SpawnDefinition { Entries = new List<SpawnEntry>() };

                string mapString = (string) row["Map"];

                Map map = Enum.Parse<Map>( mapString );

                spawn.Map = (int) map;

                string objects2;

                if ( ( objects2 = (string) row["Objects2"] ) != null )
                {
                    string[] objectList = SplitString( objects2, ":OBJ=" );

                    foreach ( string s in objectList )
                    {
                        SpawnEntry spawnEntry = new SpawnEntry();

                        string[] objectDetails = SplitString( s, ":MX=" );

                        if ( string.IsNullOrEmpty( objectDetails[0] ) )
                        {
                            continue;
                        }

                        string name = ParseName( objectDetails[0] );

                        Type typeName = ResolveType( name );

                        if ( typeName == null )
                        {
                            Console.WriteLine( $"Cannot find type {name}..." );
                            continue;
                        }

                        spawnEntry.Name = typeName.Name;
                        spawnEntry.MaxCount = int.Parse( GetParm( s, ":MX=" ) );
                        spawnEntry.Probability = 100;

                        spawn.Entries.Add( spawnEntry );
                    }
                }

                if ( spawn.Entries.Count == 0 )
                {
                    continue;
                }

                spawn.X = int.Parse( (string) row["CentreX"] );
                spawn.Y = int.Parse( (string) row["CentreY"] );
                spawn.Z = int.Parse( (string) row["CentreZ"] );
                spawn.Count = int.Parse( (string) row["MaxCount"] );
                spawn.HomeRange = int.Parse( (string) row["Range"] );

                try
                {
                    int minDelay = int.Parse( (string) row["MinDelay"] );
                    int maxDelay = int.Parse( (string) row["MaxDelay"] );

                    bool isSeconds = bool.Parse( (string) row["DelayInSec"] );

                    spawn.MinDelay = isSeconds ? TimeSpan.FromSeconds( minDelay ) : TimeSpan.FromMinutes( minDelay );
                    spawn.MaxDelay = isSeconds ? TimeSpan.FromSeconds( maxDelay ) : TimeSpan.FromMinutes( maxDelay );
                }
                catch ( ArgumentException )
                {
                    spawn.MinDelay = TimeSpan.FromMinutes( 5 );
                    spawn.MaxDelay = TimeSpan.FromMinutes( 10 );
                }

                spawnDefinitions.Add( spawn );
            }

            if ( spawnDefinitions.Count <= 0 )
            {
                return;
            }

            if ( !Directory.Exists( OUTPUT_DIRECTORY ) )
            {
                Directory.CreateDirectory( OUTPUT_DIRECTORY );
            }

            using JsonTextWriter jtw =
                new JsonTextWriter( new StreamWriter( Path.Combine( OUTPUT_DIRECTORY, $"{baseFileName}.json" ) ) );

            JsonSerializer serializer =
                JsonSerializer.Create( new JsonSerializerSettings { Formatting = Formatting.Indented } );

            serializer.Serialize( jtw, spawnDefinitions );
        }

        private static string ParseName( string s )
        {
            return s.TakeWhile( char.IsLetterOrDigit ).Aggregate( "", ( current, l ) => current + l );
        }

        public static Type ResolveType( string name )
        {
            Type type = Assembly.GetAssembly( typeof( Bird ) ).GetTypes().FirstOrDefault( t =>
                t.Namespace != null &&
                ( t.Namespace.StartsWith( "Server.Mobiles" ) || t.Namespace.StartsWith( "Server.Items" ) ) &&
                t.Name.Equals( name, StringComparison.OrdinalIgnoreCase ) );

            return type != null ? type : null;
        }

        #region XmlSpawner2

        public static string[] SplitString( string str, string separator )
        {
            if ( str == null || separator == null )
            {
                return null;
            }

            int lastindex = 0;
            int index = 0;
            List<string> strargs = new List<string>();

            while ( index >= 0 )
            {
                // go through the string and find the first instance of the separator
                index = str.IndexOf( separator, StringComparison.Ordinal );

                if ( index < 0 )
                {
                    // no separator so its the end of the string
                    strargs.Add( str );
                    break;
                }

                string arg = str.Substring( lastindex, index );

                strargs.Add( arg );

                str = str.Substring( index + separator.Length, str.Length - ( index + separator.Length ) );
            }

            // now make the string args
            string[] args = new string[strargs.Count];

            for ( int i = 0; i < strargs.Count; i++ )
            {
                args[i] = strargs[i];
            }

            return args;
        }

        internal static string GetParm( string str, string separator )
        {
            // find the parm separator in the string
            // then look for the termination at the ':'  or end of string
            // and return the stuff between
            string[] arg = SplitString( str, separator );

            //should be 2 args
            if ( arg.Length > 1 )
            {
                // look for the end of parm terminator (could also be eol)
                string[] parm = arg[1].Split( ':' );

                if ( parm.Length > 0 )
                {
                    return parm[0];
                }
            }

            return null;
        }

        #endregion
    }
}