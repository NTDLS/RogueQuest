using Game.Engine;
using Library.Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game.Actors
{
    public class ActorPlayer: ActorBase
    {

        public ActorPlayer(EngineCore core)
            : base(core)
        {
            this.SetImage(Assets.SpriteCache.GetBitmapCached(Assets.Constants.GetAssetPath(@"Players\1\Front 1.png")));
        }
    }
}
