#region

using UnityEngine;

#endregion

namespace SteeringBehaviours
{
    public class SteeringBehaviourAlignment : SteeringBehaviour
    {
        [Range(0, 360)] public float m_FOV = 360;
        public float m_AlignmentRange;

        public override Vector2 CalculateForce()
        {
            //the current position
            Vector2 pos = transform.position;
            //current look direction
            Vector2 lookDir = Maths.Normalise(m_Manager.m_Entity.m_Velocity);
            //sets the default if not moving yet
            lookDir = lookDir == Vector2.zero ? Vector2.up : lookDir;
            //holds the total average direction of all neighbours
            Vector2 alignedDir = Vector2.zero;

            //halves the value so input angles make more sense
            float halfAngle = m_FOV / 2f;
            //Cos converts radians into a ratio the dot product can use
            float dotThreshold = Mathf.Cos(halfAngle * Mathf.Deg2Rad);
            int neighborCount = 0;

            Collider2D[] entitiesInRange = Physics2D.OverlapCircleAll(pos, m_AlignmentRange);

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
                    if (entityCol.gameObject.TryGetComponent(out SteeringBehaviourManager neighbor))
                    {
                        //the direction used to gradually align the current one 
                        m_DesiredVelocity = Maths.Normalise(neighbor.m_Entity.m_Velocity);
                        //tallies the total alignment and neighbours
                        alignedDir += m_DesiredVelocity; 
                        ++neighborCount;
                    }
                    //no else needed because the next statement will stop the function if there are no neighbours anyway
                }
            }
            
            //stops if there are no neighbours
            if (neighborCount == 0) return Vector2.zero;
        
            //finishes calculating the average alignment
            alignedDir /= neighborCount;
    
            //scales the force to the max speed by default
            alignedDir = Maths.Normalise(alignedDir) * m_Manager.m_Entity.m_MaxSpeed;
            //returns the direction and speed of the re-alignment, with reference to the priority/weight
            m_Steering = alignedDir - m_Manager.m_Entity.m_Velocity;
            return m_Steering * m_Weight;
        }
    }
}