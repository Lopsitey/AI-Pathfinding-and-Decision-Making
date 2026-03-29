namespace DecisionMaking.States
{
    public abstract class StateBase
    {
        protected readonly FSM_Manager Owner;

        protected StateBase(FSM_Manager owner)
        {
            Owner = owner;
        }

        /// <summary>
        /// Ends all steering behaviours when entering a state.
        /// </summary>
        public virtual void Enter()
        {
            Owner.m_Evade.m_Active = false;
            Owner.m_Arrive.m_Active = false;
            Owner.m_Wander.m_Active = false;
        }

        public abstract void UpdateAgent(MovingEntity movingTarget = null);
        
        // Could add any cleanup logic needed when exiting the state here.
        public abstract void Exit();
    }
}