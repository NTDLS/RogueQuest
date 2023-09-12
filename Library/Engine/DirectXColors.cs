using SharpDX.Direct2D1;
using SharpDX.Mathematics.Interop;

namespace Library.Engine
{
    internal class DirectXColors
    {
        internal class RawColors
        {
            public RawColor4 Red { get; private set; } = new(1, 0, 0, 1);
            public RawColor4 Green { get; private set; } = new(0, 1, 0, 1);
            public RawColor4 Blue { get; private set; } = new(0, 0, 1, 1);
            public RawColor4 Black { get; private set; } = new(0, 0, 0, 1);
            public RawColor4 White { get; private set; } = new(1, 1, 1, 1);
            public RawColor4 Gray { get; private set; } = new(0.25f, 0.25f, 0.25f, 1);
            public RawColor4 WhiteSmoke { get; private set; } = new(0.9608f, 0.9608f, 0.9608f, 1);
            public RawColor4 Cyan { get; private set; } = new(0, 1f, 1f, 1f);
            public RawColor4 OrangeRed { get; private set; } = new(1f, 0.2706f, 0.0000f, 1);
            public RawColor4 LawnGreen { get; private set; } = new(0.4863f, 0.9882f, 0f, 1);
        }

        internal class ColorBrushes
        {
            public SolidColorBrush Red { get; internal set; }
            public SolidColorBrush Green { get; internal set; }
            public SolidColorBrush Blue { get; internal set; }
            public SolidColorBrush Black { get; internal set; }
            public SolidColorBrush White { get; internal set; }
            public SolidColorBrush Gray { get; internal set; }
            public SolidColorBrush WhiteSmoke { get; internal set; }
            public SolidColorBrush Cyan { get; internal set; }
            public SolidColorBrush OrangeRed { get; internal set; }
            public SolidColorBrush LawnGreen { get; internal set; }
        }

        internal ColorBrushes Brushes { get; set; } = new();
        internal RawColors Raw { get; set; } = new();

        internal DirectXColors(RenderTarget renterTarget)
        {
            Brushes.Red = new SolidColorBrush(renterTarget, Raw.Red);
            Brushes.Green = new SolidColorBrush(renterTarget, Raw.Green);
            Brushes.Blue = new SolidColorBrush(renterTarget, Raw.Blue);
            Brushes.Black = new SolidColorBrush(renterTarget, Raw.Black);
            Brushes.White = new SolidColorBrush(renterTarget, Raw.White);
            Brushes.Gray = new SolidColorBrush(renterTarget, Raw.Gray);
            Brushes.WhiteSmoke = new SolidColorBrush(renterTarget, Raw.WhiteSmoke);
            Brushes.Cyan = new SolidColorBrush(renterTarget, Raw.Cyan);
            Brushes.OrangeRed = new SolidColorBrush(renterTarget, Raw.OrangeRed);
            Brushes.LawnGreen = new SolidColorBrush(renterTarget, Raw.LawnGreen);
        }
    }
}
