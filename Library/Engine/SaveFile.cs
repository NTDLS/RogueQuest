using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.Engine
{
    public class SaveFile
    {
        public GameState State { get; set; }
        public List<Level> Collection { get; set; }
    }
}
