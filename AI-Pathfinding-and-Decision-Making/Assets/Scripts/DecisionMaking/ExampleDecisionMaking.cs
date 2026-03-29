using DecisionMaking.States;
using SteeringBehaviours;
using UnityEngine;

namespace DecisionMaking
{
    public class ExampleDecisionMaking : DecisionMaking
    {

        SteeringBehaviourManager m_SteeringBehaviours;
        SteeringBehaviourWander m_Wander;
        SteeringBehaviourArrive m_Arrive;

        Task7_WanderingAgent m_Target;

        Cannon m_Cannon;

        protected override void Awake()
        {
            base.Awake();
            m_SteeringBehaviours = GetComponent<SteeringBehaviourManager>();

            if (!m_SteeringBehaviours)
                Debug.LogError("Object doesn't have a Steering Behaviour Manager attached", this);

            m_Wander = GetComponent<SteeringBehaviourWander>();

            if (!m_Wander)
                Debug.LogError("Object doesn't have a Wander Steering Behaviour attached", this);


            m_Arrive = GetComponent<SteeringBehaviourArrive>();

            if (!m_Arrive)
                Debug.LogError("Object doesn't have a Arrive Steering Behaviour attached", this);

            m_Cannon = GetComponentInChildren<Cannon>();

            if (!m_Cannon)
                Debug.LogError("Object doesn't have a Cannon attached", this);
            
            m_Arrive.m_Active = false;
            m_Wander.m_Active = true;
        }

        
        public override void CustomUpdate()
        {
            //m_Arrive.m_TargetPosition = TileGrid.GetRandomWalkableTile(2).transform.position;
            
            // Sets the target to the first enemy in the list
            if(Enemies.Count > 0)
            {
                m_Target = Enemies[0];
                m_Cannon.SetTarget(m_Target.transform);
            }
            else
            {// If no enemies are in range, clear the target
                m_Target = null;
                m_Cannon.SetTarget(null);
            }

            if (m_Target) 
            {// If there is a target, enable the Arrive behaviour and stop wandering
                m_Arrive.m_Active = true;
                m_Wander.m_Active = false;

                //seeks 3 units away from target
                Vector2 seekPoint = m_Target.transform.position - ((m_Target.transform.position - transform.position).normalized) * 3;
                m_Arrive.m_TargetPosition = seekPoint;
            }
            else
            {//If there is no target start wandering and disable the Arrive behaviour
                m_Arrive.m_Active = false;
                m_Wander.m_Active = true;
            }

            //Aims the cannon towards the target (if it exists) and fires if there is ammo
            m_Cannon.AimAndFire();
        }
    }
}
