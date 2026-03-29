using UnityEngine;

namespace PathFinding
{
    public class ThetaStar
    {
        private LayerMask m_ObstacleLayer;
        public ThetaStar(LayerMask obstacleLayer)
        {
            m_ObstacleLayer = obstacleLayer;
        }
        
        /// <summary>
        /// Checks if there is are no obstacles between the two nodes.
        /// </summary>
        public bool HasLineOfSight(GridNode nodeA, GridNode nodeB)
        {
            Vector2 start = nodeA.transform.position;
            Vector2 end = nodeB.transform.position;
            Vector2 direction = end - start;
            float distance = Maths.Magnitude(direction);
            
            float agentRadius = 0.4f;
            
            // A simple circle-cast from centre to centre, works like a raycast, just thicker
            // Ensures the path is wide enough for the agent
            RaycastHit2D hit = Physics2D.CircleCast(
                start, 
                agentRadius, 
                direction, 
                distance, 
                m_ObstacleLayer
            );


            Color debugColour = hit.collider ? Color.purple : Color.hotPink;
            Debug.DrawLine(start, end, debugColour, 5f);

            //Clear path if no obstacles were hit
            return hit.collider == null;
            //May need to make the ray start/end slightly smaller
            //to avoid hitting the floor or the nodes themselves if they have colliders.
        }
    }
}