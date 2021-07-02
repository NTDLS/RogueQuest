using Library.Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScenarioEdit
{
    public static class Singletons
    {
        public static List<ActorBase> ClipboardTiles { get; set; } = new List<ActorBase>();
    }
}
