#region

using UnityEngine;

#endregion

namespace SteeringBehaviours
{
    public class SteeringBehaviourSeparation : SteeringBehaviour
    {
        [Range(0, 360)] public float m_FOV = 152;
        public float m_SeparationRange = 2;
        private Vector2 m_TotalForce;

        public override Vector2 CalculateForce()
        {
            //resets the force for each calculation
            m_TotalForce = Vector2.zero;
            Vector2 pos = transform.position;
            Vector2 lookDir = Maths.Normalise(m_Manager.m_Entity.m_Velocity);
            lookDir = lookDir == Vector2.zero ? Vector2.up : lookDir; //gives a default value if not moving yet

            //halves the input angle so 180 is half a circle
            float halfAngle = m_FOV / 2f;
            //Cos converts radians into a ratio the dot product can use
            //It would be 1 if looking at the target whereas, 0 would be off to the side.

            float dotThreshold = Mathf.Cos(halfAngle * Mathf.Deg2Rad);
            int neighborCount = 0;

            //Detects which AI are within the radius of the object
            Collider2D[] entitiesInRange = Physics2D.OverlapCircleAll(pos, m_SeparationRange);

            foreach (var entityCol in entitiesInRange)
            {
                //filters self-detection
                if (entityCol.gameObject == gameObject) continue;

                Vector2 neighborPos = entityCol.transform.position;
                Vector2 toNeighbor = neighborPos - pos;

                //This checks if the neighbour is in front of the object
                //Input the facing direction and the direction to the neighbour so if both are similar then you are looking at the neighbour
                //FOV determines how close these values need to be - higher=closer - lower=further
                if (Vector2.Dot(lookDir, Maths.Normalise(toNeighbor)) > dotThreshold)
                {
                    //push is from the neighbour to the object
                    m_DesiredVelocity = pos - neighborPos;
                    //distance - the quantity of the direction vector
                    float dist = Maths.Magnitude(m_DesiredVelocity);
                    if (dist > 0)
                    {
                        //distance is divided so that closer people push harder
                        m_TotalForce += Maths.Normalise(m_DesiredVelocity) / dist; //Normalized to get purely direction
                        ++neighborCount;
                    }
                }
            }
            
            //essentially the flee code - flees from the average centre 
            //stops if there are no neighbours
            if (neighborCount == 0) return Vector2.zero;

            //this finishes averaging the force - add up then divide by how many there are
            m_TotalForce /= neighborCount;
            //scaled to the max speed by default
            m_TotalForce = Maths.Normalise(m_TotalForce) * m_Manager.m_Entity.m_MaxSpeed;
            //returns the move direction (desired - current)
            m_Steering = m_TotalForce - m_Manager.m_Entity.m_Velocity;
            return m_Steering * m_Weight;
        }
    }
}