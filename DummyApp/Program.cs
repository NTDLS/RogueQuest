using Library.Engine;
using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Library.Engine.Types;
using System.Collections.Generic;
using System.Drawing.Imaging;

namespace DummyApp
{
    class Program
    {
        static void Main(string[] args)
        {
            CompileScenarioTerrain(@"C:\Users\Josh\AppData\Roaming\Rougue Quest\Scenario\Desolation.rqs");
        }

        static void CompileScenarioTerrain(string fileName)
        {
            var parentControl = new Control
            {
                Width = 1024,
                Height = 768,
            };

            var engine = new EngineCoreBase(parentControl, parentControl.Size);

            engine.Load(fileName);

            foreach (var level in engine.Levels.Collection)
            {
                engine.SelectLevel(level.Name);

                var levelTerrainTiles = engine.Actors.Tiles.Where(o => o.Meta.ActorClass == ActorClassName.ActorTerrain).ToList();
                if (levelTerrainTiles.Any())
                {
                    var width = levelTerrainTiles.Max(o => o.X) - levelTerrainTiles.Min(o => o.X);
                    var height = levelTerrainTiles.Max(o => o.Y) - levelTerrainTiles.Min(o => o.Y);

                    var size = new Size((int)Math.Ceiling(width), (int)Math.Ceiling(height));

                    CompileLevelTerrain(size, levelTerrainTiles);
                }
            }
        }

        static void CompileLevelTerrain(Size size, List<ActorBase> tiles)
        {
            var screenBitmap = new Bitmap(size.Width, size.Height);

            var screenDC = Graphics.FromImage(screenBitmap);
            screenDC.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            screenDC.CompositingMode = System.Drawing.Drawing2D.CompositingMode.SourceOver;
            screenDC.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
            screenDC.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            screenDC.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;

            screenDC.Clear(EngineCoreBase.BackgroundColor);

            foreach (var tile in tiles)
            {
                Library.Native.Types.DynamicCast(tile, tile.GetType()).Render(screenDC);
            }

            screenBitmap.Save("C:\\test.png", ImageFormat.Png);
        }

        static void MakeCursedEnchantedAssetCopies()
        {
            var files = Directory.EnumerateFiles(@"C:\NTDLS\RougueQuest\Assets\Tiles\Items\@inTheWorks\Weapons\Ranged", "*.png", SearchOption.AllDirectories);

            foreach (var file in files)
            {
                File.Copy(file, Path.GetDirectoryName(file) + @"\" + Path.GetFileNameWithoutExtension(file) + ".Enchanted.png");
                File.Copy(file, Path.GetDirectoryName(file) + @"\" + Path.GetFileNameWithoutExtension(file) + ".Cursed.png");
            }
        }
    }
}
