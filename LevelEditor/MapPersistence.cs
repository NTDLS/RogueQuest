using LevelEditor.Engine;
using Library.Engine;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LevelEditor
{
    public static class MapPersistence
    {
        public static void Save(EngineCore core, string fileName)
        {
            var map = new PersistMap();

            foreach (var obj in core.Terrains.Where(o => o.Visible))
            {

                map.Chunks.Add(new PersistMapEntity
                {
                    TileTypeKey = (obj as TerrainEditorTile).TileTypeKey,
                    X = obj.X,
                    Y = obj.Y,
                });
            }

            var json = JsonConvert.SerializeObject(map);

            System.IO.File.WriteAllText(fileName, json);
        }

        public static void Load(EngineCore core, string fileName)
        {
            var json = System.IO.File.ReadAllText(fileName);

            var map = JsonConvert.DeserializeObject<PersistMap>(json);

            foreach (var chunk in map.Chunks)
            {
                core.AddNewTerrain<TerrainEditorTile>(chunk.X, chunk.Y, chunk.TileTypeKey);
            }
        }
    }
}