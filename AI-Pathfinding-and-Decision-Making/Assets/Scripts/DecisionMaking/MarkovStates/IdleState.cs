using UnityEngine;

namespace DecisionMaking.MarkovStates
{
    public sealed class MarkovIdleState : MarkovStateBase
    {

        public override void UpdateAgent() =>
            Owner.m_Wander.m_Weight = m_StartingWeight * (DegreeOfActivation / 100f);

        protected override void Start()
        {
            base.Start();
            m_StartingWeight = Owner.m_Wander.m_Weight;  
        }

        public override void Enter()
        {
            base.Enter();
            Owner.m_Wander.m_Active = IsActive;
        }

        public override void Exit()
        {
            base.Exit();
            Owner.m_Wander.m_Active = IsActive;
        }

        /// <summary>
        /// If no moving target ot pickup target wander.
        /// </summary>
        public override float CalculateActivation(MovingEntity movingTarget = null, Vector2 target = default)
        {
            DegreeOfActivation = !m_MovingTarget && !IsFinite(m_PickupTarget) ? 100f : 0.0f;
            return DegreeOfActivation;
        }
    }
}