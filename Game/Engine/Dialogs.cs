using Game.Actors;
using System;
using System.Drawing;

namespace Game.Engine
{
    public static class Dialogs
    {
        public static DateTime DialogOpenTime { get; set; }

        public static void DrawDialog(EngineCore core, string msg)
        {
            core.State.IsDialogActive = true;
            DialogOpenTime = DateTime.Now;

            var brush = new SolidBrush(Color.Black);

            ActorTextBlock textBlock = null;
            int textSize = 20;

            do
            {
                if (textBlock != null)
                {
                    textBlock.QueueForDelete();
                }

                textBlock = core.AddNewTextBlock("Arial", brush, textSize, 0, 0, true, msg);
                textSize -= 2;
            }
            while (textBlock.Size.Width > core.Display.VisibleSize.Width - 40 && textSize > 5);

            core.PurgeAllDeletedTiles();

            textBlock.X = (core.Display.VisibleSize.Width / 2) - (textBlock.Size.Width / 2);
            textBlock.Y = (core.Display.VisibleSize.Height / 2) - (textBlock.Size.Height / 2);
            textBlock.Invalidate();

            textBlock.DrawOrder = 1010;

            var dialogXWallE = textBlock.X + textBlock.Size.Width + 16;
            var dialogYWallE = textBlock.Y - 16;

            var dialogXWallW = textBlock.X - 16;
            var dialogYWallW = textBlock.Y - 16;

            var neCorner = core.Actors.AddNew<ActorDialog>(dialogXWallE, dialogYWallE, @"Tiles\Special\@UI\Dialog\Dialog NE");
            var nwCorner = core.Actors.AddNew<ActorDialog>(dialogXWallW, dialogYWallW, @"Tiles\Special\@UI\Dialog\Dialog NW");

            neCorner.DrawOrder = 1002;
            nwCorner.DrawOrder = 1002;

            ActorDialog lastEWall = null;
            ActorDialog lastWWall = null;

            for (double y = dialogYWallW + nwCorner.Size.Height; y < textBlock.Y + textBlock.Size.Height + 16;)
            {
                lastEWall = core.Actors.AddNew<ActorDialog>(dialogXWallE, y, @"Tiles\Special\@UI\Dialog\Dialog E");
                lastEWall.DrawOrder = 1001;
                lastWWall = core.Actors.AddNew<ActorDialog>(dialogXWallW, y, @"Tiles\Special\@UI\Dialog\Dialog W");
                lastWWall.DrawOrder = 1001;
                y += nwCorner.Size.Height;
            }

            var seCorner = core.Actors.AddNew<ActorDialog>(lastEWall.X, lastEWall.Y + lastEWall.Size.Height, @"Tiles\Special\@UI\Dialog\Dialog SE");
            var swCorner = core.Actors.AddNew<ActorDialog>(lastWWall.X, lastWWall.Y + lastWWall.Size.Height, @"Tiles\Special\@UI\Dialog\Dialog SW");

            seCorner.DrawOrder = 1002;
            swCorner.DrawOrder = 1002;

            for (double x = dialogXWallW + nwCorner.Size.Width; x < textBlock.X + textBlock.Size.Width + 16;)
            {
                var tile = core.Actors.AddNew<ActorDialog>(x, neCorner.Y, @"Tiles\Special\@UI\Dialog\Dialog N");
                tile.DrawOrder = 1001;
                tile = core.Actors.AddNew<ActorDialog>(x, swCorner.Y, @"Tiles\Special\@UI\Dialog\Dialog S");
                tile.DrawOrder = 1001;
                x += nwCorner.Size.Height;
            }

            for (double x = nwCorner.X + nwCorner.Size.Width; x < lastEWall.X;)
            {
                for (double y = nwCorner.Y + nwCorner.Size.Height; y < lastWWall.Y + lastWWall.Size.Height + 16;)
                {
                    var fillTile = core.Actors.AddNew<ActorDialog>(x, y, @"Tiles\Special\@UI\Dialog\Dialog Fill");

                    fillTile.DrawOrder = 1000;

                    y += nwCorner.Size.Height;
                }
                x += nwCorner.Size.Width;
            }


            core.Display.DrawingSurface.Invalidate();
        }
    }
}
