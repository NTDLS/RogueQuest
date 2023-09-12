using SharpDX.DirectWrite;

namespace Library.Engine
{
    internal class DirectXTextFormats
    {
        public TextFormat MenuGeneral { get; private set; }
        public TextFormat MenuTitle { get; private set; }
        public TextFormat MenuItem { get; private set; }
        public TextFormat LargeBlocker { get; private set; }
        public TextFormat RadarPositionIndicator { get; private set; }
        public TextFormat RealtimePlayerStats { get; private set; }

        public DirectXTextFormats(Factory factory)
        {
            LargeBlocker = new TextFormat(factory, "Consolas", 50);
            MenuGeneral = new TextFormat(factory, "Consolas", 20);
            MenuTitle = new TextFormat(factory, "Consolas", 32);
            MenuItem = new TextFormat(factory, "Consolas", 20);
            RadarPositionIndicator = new TextFormat(factory, "Consolas", 16);
            RealtimePlayerStats = new TextFormat(factory, "Consolas", 16);
        }
    }
}
