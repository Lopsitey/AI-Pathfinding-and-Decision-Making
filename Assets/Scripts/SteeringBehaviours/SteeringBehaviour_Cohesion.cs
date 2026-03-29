using UnityEngine;

namespace SteeringBehaviours
{
    public class SteeringBehaviourCohesion : SteeringBehaviour
    {
        public float m_CohesionRange;
    
        [Range(0,360)]
        public float m_FOV = 360;
        public override Vector2 CalculateForce()
        {
            //the current position
            Vector2 pos = transform.position;
            //current look direction
            Vector2 lookDir = Maths.Normalise(m_Manager.m_Entity.m_Velocity);
            //sets the default if not moving yet
            lookDir = lookDir == Vector2.zero ? Vector2.up : lookDir;
            //holds the total average position of all neighbours
            Vector2 centerOfMass = Vector2.zero;
            
            //halves the value so input angles make more sense
            float halfAngle = m_FOV / 2f;
            //Cos converts radians into a ratio the dot product can use
            float dotThreshold = Mathf.Cos(halfAngle * Mathf.Deg2Rad);
            int neighborCount = 0;
            
            Collider2D[] entitiesInRange = Physics2D.OverlapCircleAll(pos, m_CohesionRange);

            foreach (var entityCol in entitiesInRange)
            {
                //filters self-detection
                if (entityCol.gameObject == gameObject) continue;
                
                //the direction the neighbour is facing 
                Vector2 neighborPos = entityCol.transform.position;
                Vector2 toNeighbor = neighborPos - pos; 
                
                //FOV determines how close these values need to be - higher=closer - lower=further
                if (Vector2.Dot(lookDir, Maths.Normalise(toNeighbor)) > dotThreshold)
                {
                    //tallies all the neighbour's positions
                    centerOfMass += neighborPos;
                    neighborCount++;
                }
            }
            
            //essentially the seek code - seeks to the average centre
            //stops if there are no neighbours
            if (neighborCount == 0) return Vector2.zero;
            
            //finishes calculating the average
            centerOfMass /= neighborCount;
            //calculates the vec to the centre seeking point
            m_DesiredVelocity = centerOfMass - pos;

            //normalise it and scale it to the max speed
            m_DesiredVelocity = m_Manager.m_Entity.m_MaxSpeed * Maths.Normalise(m_DesiredVelocity);
        
            //desired velocity - current velocity = steering/force
            m_Steering = m_DesiredVelocity - m_Manager.m_Entity.m_Velocity;
            return m_Steering * m_Weight;
        }
    }
}
