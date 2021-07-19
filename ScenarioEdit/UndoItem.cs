using Library.Engine;
using Library.Types;
using System.Collections.Generic;

namespace ScenarioEdit
{
    public class UndoItem
    {
        public enum ActionPerformed
        {
            Moved,
            Deleted,
            Created,
        }

        public ActionPerformed Action { get; set; }

        public List<ActorBase> Tiles { get; set; } = new List<ActorBase>();

        public Point<double> Offset { get; set; }
    }
}
