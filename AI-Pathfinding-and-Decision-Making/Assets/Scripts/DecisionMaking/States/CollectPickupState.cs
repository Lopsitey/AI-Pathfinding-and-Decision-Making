namespace DecisionMaking.States
{
    //Sealed prevents other classes from inheriting from this state.
    public sealed class CollectPickupState : StateBase
    {
        // This constructor initializes the reference to the owner (FSM_Manager).
        // Ensures the state has access to the owner's properties etc. when instantiated.
        public CollectPickupState(FSM_Manager owner) : base(owner) { }
        public override void UpdateAgent(MovingEntity movingTarget = null)
        {
            Owner.m_Arrive.m_Active = true;
            Owner.m_Arrive.m_TargetPosition = Owner.m_PickupTarget;
        }

        public override void Exit() { }
    }
}