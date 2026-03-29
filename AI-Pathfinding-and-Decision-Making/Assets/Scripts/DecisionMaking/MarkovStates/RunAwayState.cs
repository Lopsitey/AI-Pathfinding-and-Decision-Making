#region

using UnityEngine;

#endregion

namespace DecisionMaking.MarkovStates
{
    public sealed class MarkovRunAwayState : MarkovStateBase
    {
        public override void UpdateAgent()
        {
            if (!m_MovingTarget) return;

            Owner.m_Evade.m_Weight = m_StartingWeight * (DegreeOfActivation / 100f);
            Owner.m_Evade.m_EvadedEntity = m_MovingTarget;
        }

        protected override void Start()
        {
            base.Start();
            m_StartingWeight = Owner.m_Evade.m_Weight;
        }

        public override void Enter()
        {
            base.Enter();
            Owner.m_Evade.m_Active = IsActive;
        }

        public override void Exit()
        {
            base.Exit();
            Owner.m_Evade.m_Weight = m_StartingWeight;
            m_MovingTarget = null;
            Owner.m_Evade.m_Active = IsActive;
        }

        public override float CalculateActivation(MovingEntity movingTarget = null, Vector2 target = default)
        {
            if (!movingTarget) return 0.0f;

            m_MovingTarget = movingTarget;
            Vector2 myPos = Owner.transform.position;
            Vector2 targetPos = m_MovingTarget.transform.position;

            DegreeOfActivation = ExponentialCurve(in myPos, in targetPos, false);

            // Throttles the amount of debug messages being output to the console 
            if(Owner.Has120FramesPassed())
                Debug.Log($"Distance to zombie changed Evade's degree of activation to: {(int)DegreeOfActivation}%");
            return DegreeOfActivation;
        }
    }
}