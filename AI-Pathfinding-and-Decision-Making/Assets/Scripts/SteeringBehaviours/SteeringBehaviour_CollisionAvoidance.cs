#region

using System;
using UnityEngine;

#endregion

namespace SteeringBehaviours
{
    public class SteeringBehaviourCollisionAvoidance : SteeringBehaviour
    {
        [Serializable]
        public struct Feeler
        {
            [Range(0, 360)] public float m_Angle;
            public float m_MaxLength;
            public Color m_Colour;
        }

        public Feeler[] m_Feelers;
        Vector2[] m_FeelerVectors;
        float[] m_FeelersLength;

        [SerializeField] LayerMask m_FeelerLayerMask;

        private void Start()
        {
            m_FeelersLength = new float[m_Feelers.Length];
            m_FeelerVectors = new Vector2[m_Feelers.Length];
        }

        public override Vector2 CalculateForce()
        {
            //uses max value so the first hit is always stored since it will always be less than the highest possible float
            //this then allows the rest of the comparison chain to find the closest hit correctly
            //closestHit = hit.distance<closestHit ? hit.distance : closestHit 
            float closestHit = float.MaxValue;
            Vector2 obstacleToAvoid = Vector2.zero;
            float closestFeelerLength = 0; // We need this for the math later!
            bool obstacleFound = false;
            Vector2 currentPos = transform.position;
        
            UpdateFeelers();
        
            for (int i = 0; i < m_Feelers.Length; ++i)
            {
                Vector2 dir = Maths.Normalise(m_FeelerVectors[i]);
                float len = m_FeelersLength[i];
            
                RaycastHit2D hit = Physics2D.Raycast(currentPos, dir, len, m_FeelerLayerMask);
                if (hit.collider && hit.distance < closestHit)
                {
                    closestHit = hit.distance;
                    obstacleToAvoid = hit.point;
                    closestFeelerLength = len;
                    obstacleFound = true;
                } 
            }

            //stops if no obstacle to avoid was found
            if (!obstacleFound) return Vector2.zero;
        
            //essentially the flee behaviour - flees from the obstacle
            //the direction from the obstacle to you
            m_DesiredVelocity = currentPos - obstacleToAvoid;
            m_DesiredVelocity = m_Manager.m_Entity.m_MaxSpeed * Maths.Normalise(m_DesiredVelocity);

            m_Steering = m_DesiredVelocity - m_Manager.m_Entity.m_Velocity;

            //the force is increased the lower the distance is - because of the .min
            return m_Steering * Mathf.Lerp(m_Weight, 0, Mathf.Min(closestHit, closestFeelerLength) / closestFeelerLength);
            //if the object is far enough away (further than the closestHit) then less force is applied
        }

        void UpdateFeelers()
        {
            for (int i = 0; i < m_Feelers.Length; ++i)
            {
                m_FeelersLength[i] = Mathf.Lerp(1, m_Feelers[i].m_MaxLength,
                    Maths.Magnitude(m_Manager.m_Entity.m_Velocity) / m_Manager.m_Entity.m_MaxSpeed);
                m_FeelerVectors[i] =
                    Maths.RotateVector(Maths.Normalise(m_Manager.m_Entity.m_Velocity), m_Feelers[i].m_Angle) *
                    m_FeelersLength[i];
            }
        }

        protected override void OnDrawGizmosSelected()
        {
            if (Application.isPlaying)
            {
                if (m_Debug_ShowDebugLines && m_Active && m_Manager.m_Entity)
                {
                    for (int i = 0; i < m_Feelers.Length; ++i)
                    {
                        Gizmos.color = m_Feelers[i].m_Colour;
                        Gizmos.DrawLine(transform.position, (Vector2)transform.position + m_FeelerVectors[i]);
                    }

                    base.OnDrawGizmosSelected();
                }
            }
        }
    }
}