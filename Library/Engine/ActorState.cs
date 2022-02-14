using Library.Engine.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.Engine
{
    public class ActorState
    {
        public Guid ActorUID { get; set; }
        public StateOfBeing State { get; set; }
    }
}
