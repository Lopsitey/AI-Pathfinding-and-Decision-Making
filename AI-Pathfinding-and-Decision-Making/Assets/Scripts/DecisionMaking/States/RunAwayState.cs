namespace DecisionMaking.States
{
    public sealed class RunAwayState : StateBase
    {
        public RunAwayState(FSM_Manager owner) : base(owner) { }

        public override void UpdateAgent(MovingEntity movingTarget = null)
        {
            if (!movingTarget) return;

            Owner.m_Evade.m_Active = true;
            Owner.m_Evade.m_EvadedEntity = movingTarget;
        }

        public override void Exit() { }
    }
}