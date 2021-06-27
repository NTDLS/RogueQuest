using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.Engine
{
    public class BinarySaveFile
    {
        public GameState State { get; set; }
        public List<byte[]> Bytes { get; set; }
    }
}
