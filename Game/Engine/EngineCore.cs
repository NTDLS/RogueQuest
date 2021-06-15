using Library.Types;
using Game.Actors;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Library.Engine;

namespace Game.Engine
{
    public class EngineCore : EngineCoreBase
    {
        public EngineCore(Control drawingSurface, Size visibleSize)
            : base (drawingSurface, visibleSize)
        {

        }

        public ActorTextBlock AddNewTextBlock(string font, Brush color, double size, double x, double y, bool isPositionStatic, string text = "")
        {
            lock (CollectionSemaphore)
            {
                var obj = new ActorTextBlock(this, font, color, size, new Point<double>(x, y), isPositionStatic)
                {
                    Text = text
                };
                Actors.Add(obj);
                return obj;
            }
        }
    }
}
