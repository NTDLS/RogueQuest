using Library.Engine.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.Engine
{
    public class PlayMode
    {
        private ReplayMode _replay;

        public ReplayMode Replay
        {
            get
            {
                return _replay;
            }
            set
            {
                if (value == ReplayMode.LoopedPlay)
                {
                    DeleteActorAfterPlay = false;
                }
                _replay = value;
            }
        }

        public TimeSpan ReplayDelay;
        public bool DeleteActorAfterPlay;
    }
}
