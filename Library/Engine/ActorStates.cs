using Library.Engine.Types;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Library.Engine
{
    public class ActorStates
    {
        public List<ActorState> StateDictonary { get; set; } = new List<ActorState>();

        public List<StateOfBeing> StatesOfBeing(Guid actorUID)
        {
            return StateDictonary.Where(o => o.ActorUID == actorUID).Select(o => o.State).ToList();
        }

        public List<ActorState> States()
        {
            return StateDictonary.ToList();
        }

        public List<ActorState> States(Guid actorUID)
        {
            return StateDictonary.Where(o => o.ActorUID == actorUID).ToList();
        }

        public void Remove(List<ActorState> toRemove)
        {
            foreach (var item in toRemove)
            {
                StateDictonary.Remove(item);
            }
        }

        public ActorState UpsertState(Guid actorUID, StateOfBeing state, int expireTime)
        {
            ActorState actorState = StateDictonary.Where(o => o.ActorUID == actorUID && o.State == state).FirstOrDefault();

            if (actorState == null)
            {
                actorState = new ActorState()
                {
                    ActorUID = actorUID,
                    State = state,
                    ExpireTime = expireTime
                };

                StateDictonary.Add(actorState);
            }
            else
            {
                actorState.ExpireTime += expireTime;
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

        public void RemoveState(ActorState state)
        {
            StateDictonary.Remove(state);
        }

        public void RemoveAll(Guid actorUID)
        {
            StateDictonary.RemoveAll(o => o.ActorUID == actorUID);
        }

        public void RemoveState(Guid actorUID, StateOfBeing state)
        {
            StateDictonary.RemoveAll(o => o.ActorUID == actorUID && o.State == state);
        }

        public bool HasState(Guid actorUID, StateOfBeing state)
        {
            return StatesOfBeing(actorUID)?.Contains(state) ?? false;
        }
    }
}
