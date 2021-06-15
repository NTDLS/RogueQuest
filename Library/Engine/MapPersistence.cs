using Newtonsoft.Json;
using System.Linq;

namespace Library.Engine
{
    public static class MapPersistence
    {
        public static void Save(EngineCoreBase core, string fileName)
        {
            var map = new PersistMap();

            foreach (var obj in core.TerrainTiles.Where(o => o.Visible))
            {
                map.Chunks.Add(new PersistMapEntity
                {
                    TileTypeKey = obj.TileTypeKey,
                    X = obj.X,
                    Y = obj.Y,
                });
            }

            var json = JsonConvert.SerializeObject(map);

            System.IO.File.WriteAllText(fileName, json);
        }

        public static void Load(EngineCoreBase core, string fileName)
        {
            var json = System.IO.File.ReadAllText(fileName);

            var map = JsonConvert.DeserializeObject<PersistMap>(json);

            foreach (var chunk in map.Chunks)
            {
                core.AddNewTerrain<TerrainBase>(chunk.X, chunk.Y, chunk.TileTypeKey);
            }
        }
    }
}
