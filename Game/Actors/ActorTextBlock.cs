using Game.Engine;
using Library.Engine;
using Library.Types;
using System;
using System.Drawing;

namespace Game.Actors
{
    public class ActorTextBlock : ActorBase
    {
        private Rectangle? _prevRegion;
        private Font _font;
        private Graphics _genericDC; //Not used for drawing, only measuring.
        private Brush _color;

        public bool IsPositionStatic { get; set; }

        #region Properties.

        double _height = 0;
        public Double Height
        {
            get
            {
                if (_height == 0)
                {
                    _height = _genericDC.MeasureString("void", _font).Height;
                }
                return _height;
            }
        }

        private string _lastTextSizeCheck;
        private Size _size = Size.Empty;
        public override Size Size
        {
            get
            {
                if ((_size.IsEmpty || _text != _lastTextSizeCheck) && _text != null)
                {
                    var fSize = _genericDC.MeasureString(_text, _font);

                    _size = new Size((int)Math.Ceiling(fSize.Width), (int)Math.Ceiling(fSize.Height));
                    _lastTextSizeCheck = _text;
                }
                return _size;
            }
        }

        private string _text;
        public string Text
        {
            get
            {
                return _text;
            }
            set
            {
                _text = value;

                //Now that we have used _prevRegion to invaldate the previous region, set it to the new region coords.
                //And invalidate them for the new text.
                var stringSize = _genericDC.MeasureString(_text, _font);
                _prevRegion = new Rectangle((int)X, (int)Y, (int)stringSize.Width, (int)stringSize.Height);
                Invalidate();
            }
        }

        #endregion

        public ActorTextBlock(EngineCore core, string font, Brush color, double size, Point<double> location, bool isPositionStatic)
            : base(core)
        {
            IsPositionStatic = isPositionStatic;
            Location = new Point<double>(location);
            AlwaysRender = isPositionStatic;
            _color = color;
            _font = new Font(font, (float)size);
            _genericDC = core.Display.DrawingSurface.CreateGraphics();
            this.Meta.ActorClass = Library.Engine.Types.ActorClassName.ActorDialog;
        }

        public override void Render(Graphics dc)
        {
            if (Visible)
            {
                dc.DrawString(_text, _font, _color, (float)X, (float)Y);
            }
        }

        public override void Invalidate()
        {
            int slop = 3; //This i just to take care of the debuging highlighting rectangles.

            var rect = new Rectangle(
                (int)(X - slop),
                (int)(Y - slop),
                Size.Width + (slop * 2),
                Size.Height + (slop * 2)
                );

            Core.Display.DrawingSurface.Invalidate(rect);
        }
    }
}