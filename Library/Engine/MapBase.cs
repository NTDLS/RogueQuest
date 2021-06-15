using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.Engine
{
    public class MapBase
    {
        #region Public properties.
        public string Tag { get; set; }
        public Guid UID { get; private set; } = Guid.NewGuid();

        #endregion

        public MapBase()
        {
            UID = Guid.NewGuid();
        }
    }
}