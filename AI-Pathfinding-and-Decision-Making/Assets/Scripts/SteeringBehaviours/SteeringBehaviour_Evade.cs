using UnityEngine;
using UnityEngine.Serialization;

namespace SteeringBehaviours
{
    public class SteeringBehaviourEvade : SteeringBehaviour
    {
        [Header("Evade Properties")]
        [Header("Settings")]
        public MovingEntity m_EvadedEntity;
        public float m_EvadeRadius = 6.0f;

        [FormerlySerializedAs("m_Debug_RadiusColour")]
        [Space(10)]

        [Header("Debugs")]
        [SerializeField]
        protected Color m_DebugRadiusColour = Color.yellow;

        public override Vector2 CalculateForce()
        {
            if (!m_EvadedEntity) return Vector2.zero;
        
            float dist = Maths.Magnitude(transform.position - m_EvadedEntity.transform.position);
            if (dist < m_EvadeRadius)
            {
                float currentSpeed = Maths.Magnitude(m_Manager.m_Entity.m_Velocity);
                float pursuerSpeed = Maths.Magnitude(m_EvadedEntity.m_Velocity);
            
                float combinedSpeed = currentSpeed + pursuerSpeed;
        
                //time to predict is the distance divided by the total speed
                float predictTime = combinedSpeed > 0.001f ? dist / combinedSpeed : 0;
            
                predictTime = Mathf.Clamp(predictTime, 0f, 5.0f);
        
                Vector2 targetPos = m_EvadedEntity.transform.position;
                Vector2 predictedTargetPosition = targetPos + (m_EvadedEntity.m_Velocity * predictTime);
        
                //reversed seek code
                Vector2 pos = transform.position;
                //calculates the vector pointing to your (the evader's) position
                m_DesiredVelocity = pos - predictedTargetPosition;
            
                m_DesiredVelocity = m_Manager.m_Entity.m_MaxSpeed * Maths.Normalise(m_DesiredVelocity);
                //calculates steering/force
                m_Steering = m_DesiredVelocity - m_Manager.m_Entity.m_Velocity;
            
                //smooths out the evading force - makes it less jittery
                return m_Steering * Mathf.Lerp(m_Weight, 0, Mathf.Min(dist, m_EvadeRadius) / m_EvadeRadius);
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
                    Gizmos.DrawWireSphere(transform.position, m_EvadeRadius);

                    base.OnDrawGizmosSelected();
                }
            }
        }
    }
}
