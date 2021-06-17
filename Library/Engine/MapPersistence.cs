using Newtonsoft.Json;
using System.Linq;

namespace Library.Engine
{
    public static class MapPersistence
    {
        public static void Save(EngineCoreBase core, string fileName)
        {
            var map = new PersistMap();

            foreach (var obj in core.Terrain.Tiles.Where(o => o.Visible))
            {
                map.Chunks.Add(new PersistMapEntity
                {
                    TileTypeKey = obj.TileTypeKey,
                    X = obj.X,
                    Y = obj.Y,
                    Angle = obj.Angle.Degrees,
                    DrawOrder = obj.DrawOrder
                });
            }

            var json = JsonConvert.SerializeObject(map);

            var compressed = Utility.Compress.Zip(json);

            System.IO.File.WriteAllBytes(fileName, compressed);
        }

        public static void Load(EngineCoreBase core, string fileName)
        {
            var compressed = System.IO.File.ReadAllBytes(fileName);

            var json = Utility.Compress.Unzip(compressed);

            var map = JsonConvert.DeserializeObject<PersistMap>(json);

            core.QueueAllForDelete();

            foreach (var chunk in map.Chunks)
            {
                var obj = core.Terrain.AddNew<TerrainBase>(chunk.X, chunk.Y, chunk.TileTypeKey);

                obj.Angle.Degrees = chunk.Angle;
                obj.DrawOrder = chunk.DrawOrder;
            }
        }
    }
}
