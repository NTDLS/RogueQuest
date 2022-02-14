using Library.Engine.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.Engine
{
    public class ActorStates
    {
        public List<ActorState> StateDictonary { get; set; } = new List<ActorState>();

        public List<StateOfBeing> StatesOfBeing(Guid actorUID)
        {
            return StateDictonary.Where(o => o.ActorUID == actorUID).Select(o => o.State).ToList();
        }

        public ActorState UpsertState(Guid actorUID, StateOfBeing state)
        {
            ActorState actorState = StateDictonary.Where(o => o.ActorUID == actorUID && o.State == state).FirstOrDefault();

            if (actorState == null)
            {
                actorState = new ActorState()
                {
                    ActorUID = actorUID,
                    State = state
                };

                StateDictonary.Add(actorState);
            }

            return actorState;
        }

        public ActorState AddState(Guid actorUID, StateOfBeing state)
        {
            var actorState = new ActorState()
            {
                ActorUID = actorUID,
                State = state
            };

            StateDictonary.Add(actorState);

            return actorState;
        }

        public void RemoveState(Guid actorUID, StateOfBeing state)
        {
            StateDictonary.RemoveAll(o => o.ActorUID == actorUID && o.State == state);
        }

        public bool HasState(Guid actorUID, StateOfBeing state)
        {
            return StatesOfBeing(actorUID)?.Contains(StateOfBeing.Poisoned) ?? false;
        }
    }
}
