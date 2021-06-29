using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.Engine
{
    public class SaveFile
    {
        public ScenarioMetaData Meta { get; set; }
        public GameState State { get; set; }
        public List<Level> Collection { get; set; }
    }
}
