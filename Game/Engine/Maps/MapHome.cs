using Library.Utility;
using Game.Engine;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Library.Engine;

namespace Game.Maps
{
    public class MapHome : MapBase
    {
        #region Public properties.

        #endregion

        public MapHome(EngineCore core)
            : base(core)
        {
            Core = core;
            //MapPersistence.Load(core, Assets.Constants.GetAssetPath(@"Maps\Home.rqm"), true);
        }
    }
}

