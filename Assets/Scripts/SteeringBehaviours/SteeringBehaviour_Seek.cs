#region

using UnityEngine;
using UnityEngine.Serialization;

#endregion

namespace SteeringBehaviours
{
    public class SteeringBehaviourSeek : SteeringBehaviour
    {
        [Header("Seek Properties")] [Header("Settings")]
        public Vector2 m_TargetPosition;

        [FormerlySerializedAs("m_Debug_TargetColour")] [Space(10)] [Header("Debugs")] [SerializeField]
        protected Color m_DebugTargetColour = Color.yellow;

        public override Vector2 CalculateForce()
        {
            Vector2 pos = transform.position;
            //calculates the vector pointing to the target position
            m_DesiredVelocity = m_TargetPosition - pos;

            //normalise it and scale it to the max speed
            m_DesiredVelocity = m_Manager.m_Entity.m_MaxSpeed * Maths.Normalise(m_DesiredVelocity);
            //normalisation makes it into a unit vector which gets rid of all the unnecessary information in the vector and makes it a length of 1
            //this is good for getting a direction, whilst disregarding magnitude (speed)
        
            //desired velocity - current velocity = steering/force
            m_Steering = m_DesiredVelocity - m_Manager.m_Entity.m_Velocity;
            return m_Steering * m_Weight;//force = steering * weight
        }

        protected override void OnDrawGizmosSelected()
        {
            if (Application.isPlaying)
            {
                if (m_Debug_ShowDebugLines && m_Active && m_Manager.m_Entity)
                {
                    Gizmos.color = m_DebugTargetColour;
                    Gizmos.DrawSphere(m_TargetPosition, 0.5f);


                    base.OnDrawGizmosSelected();
                }
            }
        }
    }
}