using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.Engine
{
    public class GameState
    {
        public PlayerState Character { get; set; }
        public string CurrentMap { get; set; }
    }
}
