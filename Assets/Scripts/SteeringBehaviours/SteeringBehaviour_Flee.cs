using UnityEngine;
using UnityEngine.Serialization;

namespace SteeringBehaviours
{
    public class SteeringBehaviourFlee : SteeringBehaviour
    {
        [Header("Flee Properties")]
        [Header("Settings")]
        public Transform m_FleeTarget;
        public float m_FleeRadius;

        [FormerlySerializedAs("m_Debug_RadiusColour")]
        [Space(10)]

        [Header("Debugs")]
        [SerializeField]
        protected Color m_DebugRadiusColour = Color.yellow;

        public override Vector2 CalculateForce()
        {
            if (!m_FleeTarget) return Vector2.zero;//only runs if there is a flee target
        
            float dist = Maths.Magnitude(transform.position - m_FleeTarget.position);
            if (dist < m_FleeRadius)
            {
                //the direction is reversed from the seek func.
                //this creates a direction vector from target pos -> your pos as opposed to vice versa (seek)
                m_DesiredVelocity = transform.position - m_FleeTarget.position;
                m_DesiredVelocity = m_Manager.m_Entity.m_MaxSpeed * Maths.Normalise(m_DesiredVelocity);
            
                //calculates steering/force
                m_Steering = m_DesiredVelocity - m_Manager.m_Entity.m_Velocity;
                //applies a smooth weight to the fleeing
                return m_Steering * Mathf.Lerp(m_Weight, 0, Mathf.Min(dist, m_FleeRadius) / m_FleeRadius);
                //the force is increased the lower the distance is - because of the .min
                //if the flee radius is higher than the distance (players is out of the radius) less force is applied
            }
            return Vector2.zero;
        }

        protected override void OnDrawGizmosSelected()
        {
            if (Application.isPlaying)
            {
                if (m_Debug_ShowDebugLines && m_Active && m_Manager.m_Entity)
                {
                    Gizmos.color = m_DebugRadiusColour;
                    Gizmos.DrawWireSphere(transform.position, m_FleeRadius);

                    base.OnDrawGizmosSelected();
                }
            }
        }
    }
}
