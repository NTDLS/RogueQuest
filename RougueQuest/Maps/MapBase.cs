using RougueQuest.Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RougueQuest.Maps
{
    public class MapBase
    {
        #region Public properties.
        public string Tag { get; set; }
        public Guid UID { get; private set; } = Guid.NewGuid();
        public EngineCore Core { get; set; }

        #endregion

        public MapBase(EngineCore core)
        {
            this.Core = core;
            UID = Guid.NewGuid();
        }
    }
}