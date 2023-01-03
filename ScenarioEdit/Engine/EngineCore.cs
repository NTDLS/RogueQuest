using Library.Engine;
using System.Collections.Generic;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Windows.Forms;
using Library.Engine.Types;
using System.IO;

namespace ScenarioEdit.Engine
{
    public class EngineCore : EngineCoreBase
    {
        public EngineCore(Control drawingSurface, Size visibleSize)
            : base(drawingSurface, visibleSize)
        {
        }

        public void LoadLevlesAndPopCurrent(string scenarioFileName)
        {
            Levels.Load(scenarioFileName);
            PopCurrentLevel();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="scenarioFileName"></param>
        /// <param name="compileTerrain">Used only by the scenario editor, saves the terrain bitmap to a file for quick access.</param>
        public void PushLevelAndSave(string scenarioFileName)
        {
            PushCurrentLevel();
            Levels.Save(scenarioFileName);
        }
    }
}
