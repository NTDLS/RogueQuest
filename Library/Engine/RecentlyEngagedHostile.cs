namespace Library.Engine
{
    public class RecentlyEngagedHostile
    {
        public ActorBase Hostile { get; set; }
        /// <summary>
        /// Interaction time in Core.State.TimePassed
        /// </summary>
        public int InteractionTime { get; set; }
    }
}
