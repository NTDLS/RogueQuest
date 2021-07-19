using Library.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.Engine
{
    public class Level
    {
        public Point<double> LastEditBackgroundOffset { get; set; }
        public string Name { get; set; }
        public byte[] Bytes { get; set; }

        public new string ToString()
        {
            return Name;
        }
    }
}
