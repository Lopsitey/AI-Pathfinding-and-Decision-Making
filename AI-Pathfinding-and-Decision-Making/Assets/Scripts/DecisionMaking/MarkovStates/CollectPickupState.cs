using UnityEngine;

namespace DecisionMaking.MarkovStates
{
    //Sealed prevents other classes from inheriting from this state.
    public sealed class MarkovCollectPickupState : MarkovStateBase
    {
        public override void UpdateAgent()
        {
            if (!IsFinite(m_PickupTarget)) return;
            
            Owner.m_Arrive.m_Weight = m_StartingWeight * (DegreeOfActivation / 100f);
            Owner.m_Arrive.m_TargetPosition = m_PickupTarget;
        }

        protected override void Start()
        {
            base.Start();
            m_StartingWeight = Owner.m_Arrive.m_Weight;
        }
        
        public override void Enter()
        {
            base.Enter();
            Owner.m_Arrive.m_Active = IsActive;
        }

        public override void Exit()
        {
            base.Exit();
            Owner.m_Arrive.m_Weight = m_StartingWeight;
            m_PickupTarget = Vector2.positiveInfinity;
            Owner.m_Arrive.m_Active = IsActive;
        }

        //This can only ever be active or inactive, so return 100 or 0.
        public override float CalculateActivation(MovingEntity movingTarget = null, Vector2 target = default)
        {
            if (!IsFinite(target)) return 0.0f;
            
            m_PickupTarget = target;
            Vector2 myPos = Owner.transform.position;
            
            DegreeOfActivation = ExponentialCurve(in myPos, in m_PickupTarget);
            //Debug.Log("The distance to the target is: " + DegreeOfActivation);
            return DegreeOfActivation;
        }
    }
}