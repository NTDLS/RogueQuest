using Library.Engine;
using Library.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using static ScenarioEdit.UndoItem;

namespace ScenarioEdit
{
    public class UndoBuffer
    {
        private enum Direction
        {
            Forward,
            Backward
        }

        public EngineCoreBase Core { get; set; }

        public UndoBuffer(EngineCoreBase core)
        {
            Core = core;
        }

        private int RollingIndex { get; set; }
        private List<UndoItem> Items { get; set; } = new List<UndoItem>();

        public void Record(List<ActorBase> tiles, ActionPerformed action, Point<double> offset = null)
        {
            if (RollingIndex != Items.Count)
            {
                Items.Clear();
                RollingIndex = 0;
            }

            var item = new UndoItem()
            {
                Tiles = tiles,
                Action = action,
                Offset = offset
            };

            Items.Add(item);

            RollingIndex++;
        }

        public void Record(ActorBase tile, ActionPerformed action)
        {
            if (RollingIndex != Items.Count)
            {
                Items.Clear();
                RollingIndex = 0;
            }

            var tiles = new List<ActorBase>();

            tiles.Add(tile);

            Items.Add(new UndoItem()
            {
                Tiles = tiles,
                Action = action
            });

            RollingIndex++;
        }

        public void Rollforward()
        {
            if (RollingIndex == Items.Count)
            {
                return;
            }

            PerformAction(Items[RollingIndex], Direction.Forward);

            RollingIndex++;
        }

        public void Rollback()
        {
            if (RollingIndex == 0)
            {
                return;
            }

            RollingIndex--;

            PerformAction(Items[RollingIndex], Direction.Backward);
        }

        private void PerformAction(UndoItem item, Direction direction)
        {
            if ((direction == Direction.Backward && item.Action == ActionPerformed.Created)
                || (direction == Direction.Forward && item.Action == ActionPerformed.Deleted))
            {
                item.Tiles.ForEach(o => o.QueueForDelete());
                Core.PurgeAllDeletedTiles();
            }
            else if ((direction == Direction.Backward && item.Action == ActionPerformed.Deleted)
                || (direction == Direction.Forward && item.Action == ActionPerformed.Created))
            {
                Core.Actors.Tiles.Where(o => o.SelectedHighlight == true)
                    .ToList().ForEach(o => o.SelectedHighlight = false);

                foreach (var tile in item.Tiles)
                {
                    tile.ReadyForDeletion = false;
                    tile.Visible = true;
                    tile.SelectedHighlight = true;
                    Core.Actors.Add(tile);
                }
            }
            else if (item.Action == ActionPerformed.Moved)
            {
                Core.Actors.Tiles.Where(o => o.SelectedHighlight == true)
                   .ToList().ForEach(o => o.SelectedHighlight = false);

                foreach (var tile in item.Tiles)
                {
                    if (direction == Direction.Backward)
                    {
                        tile.X += item.Offset.X;
                        tile.Y += item.Offset.Y;
                    }
                    else if (direction == Direction.Forward)
                    {
                        tile.X -= item.Offset.X;
                        tile.Y -= item.Offset.Y;
                    }

                    tile.SelectedHighlight = true;
                }
            }
            else
            {
                throw new NotFiniteNumberException();
            }
        }
    }
}
