using Library.Engine;
using System.Drawing;
using System.Windows.Forms;

namespace ScenarioEdit.Engine
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
