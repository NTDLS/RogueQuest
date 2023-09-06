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
