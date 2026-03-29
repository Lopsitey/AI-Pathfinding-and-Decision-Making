namespace DecisionMaking.States
{
    public sealed class IdleState : StateBase
    {
        public IdleState(FSM_Manager owner) : base(owner) { }

        public override void UpdateAgent(MovingEntity movingTarget = null)
            => Owner.m_Wander.m_Active = true;

        public override void Exit() { }
    }
}