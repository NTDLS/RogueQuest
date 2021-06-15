using Assets;
using Library.Engine;
using Library.Types;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace LevelEditor.Engine
{
    public class EngineCore : EngineCoreBase
    {
        public EngineCore(Control drawingSurface, Size visibleSize)
            : base(drawingSurface, visibleSize)
        {

        }


        public T AddNewTerrain<T>(double x, double y, string tileTypeKey) where T : TerrainEditorTile
        {
            lock (CollectionSemaphore)
            {
                var bitmap = SpriteCache.GetBitmapCached($"{tileTypeKey}.png");
                object[] param = { this, tileTypeKey, bitmap };
                var obj = (TerrainBase)Activator.CreateInstance(typeof(T), param);
                obj.X = x;
                obj.Y = y;
                Terrains.Add(obj);
                return (T)obj;
            }
        }

    }
}