using Game.Engine;
using Library.Engine;

namespace Game.Actors
{
    public class ActorDialog : ActorBase
    {
        public ActorDialog(EngineCore core)
            : base(core)
        {
            DrawRealitiveToBackgroundOffset = false;
            this.Meta.ActorClass = Library.Engine.Types.ActorClassName.ActorDialog;
        }
    }
}
