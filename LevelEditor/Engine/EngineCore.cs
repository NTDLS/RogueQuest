using Assets;
using Library.Engine;
using Library.Types;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace LevelEditor.Engine
{
    public class EngineCore : EngineCoreBase
    {
        public EngineCore(Control drawingSurface, Size visibleSize)
            : base(drawingSurface, visibleSize)
        {
        }

        public void LoadLevlesAndPopCurrent(string fileName)
        {
            Levels.Load(fileName);
            PopCurrentLevel();        
        }

        public void PushLevelAndSave(string fileName)
        {
            PushCurrentLevel();
            Levels.Save(fileName);
        }
    }
}
