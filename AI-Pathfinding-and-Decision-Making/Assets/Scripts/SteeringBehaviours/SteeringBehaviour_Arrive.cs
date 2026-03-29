using UnityEngine;
using UnityEngine.Serialization;

namespace SteeringBehaviours
{
    public class SteeringBehaviourArrive : SteeringBehaviour
    {
        [Header("Arrive Properties")]
        [Header("Settings")]
        public Vector2 m_TargetPosition;
        public float m_SlowingRadius; 

        [FormerlySerializedAs("m_Debug_RadiusColour")]
        [Space(10)]

        [Header("Debugs")]
        [SerializeField]
        protected Color m_DebugRadiusColour = Color.yellow;
        [FormerlySerializedAs("m_Debug_TargetColour")] [SerializeField]
        protected Color m_DebugTargetColour = Color.cyan;


        public override Vector2 CalculateForce()
        {
            //Current position
            Vector2 pos = transform.position;
            //Direction to target
            m_DesiredVelocity = m_TargetPosition - pos;
            //Initial distance
            float dist = Maths.Magnitude(m_DesiredVelocity);
            //Normalise for scaling with initial speed application
            m_DesiredVelocity = m_Manager.m_Entity.m_MaxSpeed * Maths.Normalise(m_DesiredVelocity);
        
            //If the arriver is within range of the object to arrive at 
            if (dist < m_SlowingRadius)
            { 
                //Decrease speed as a percentage of the max using distance into slowing radius
                m_DesiredVelocity = m_Manager.m_Entity.m_MaxSpeed * (dist/m_SlowingRadius) * Maths.Normalise(m_DesiredVelocity);
            } 
        
            //Steering direction / force applied = desired velocity - current velocity
            m_Steering = m_DesiredVelocity - m_Manager.m_Entity.m_Velocity;
            //Return total force with weight applied
            return m_Steering * m_Weight;
        }

        protected override void OnDrawGizmosSelected()
        {
            if (Application.isPlaying)
            {
                if (m_Debug_ShowDebugLines && m_Active && m_Manager.m_Entity)
                {
                    Gizmos.color = m_DebugTargetColour;
                    Gizmos.DrawSphere(m_TargetPosition, 0.5f);

                    Gizmos.color = m_DebugRadiusColour;
                    Gizmos.DrawWireSphere(transform.position, m_SlowingRadius);

                    base.OnDrawGizmosSelected();
                }
            }
        }
    }
}
