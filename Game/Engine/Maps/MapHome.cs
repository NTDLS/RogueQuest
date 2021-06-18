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

            //MapPersistence.Load(core, Assets.Constants.GetAssetPath(@"Maps\Test.rqm"), true);
            //var player = core.CreatePlayer();
            //player.X = 100;
            //player.Y = 150;
        }
    }
}

