#region

using System.Collections.Generic;
using UnityEngine;

#endregion

namespace PathFinding
{
    public abstract class PathFinding
    {
        protected GridNode[] m_GridNodes;

        [Header("Settings")]
        [Tooltip("Sets whether the agent can move diagonally, can only be set in the editor during runtime")]
        public bool m_AllowDiagonal;

        [Tooltip(
            "Sets whether the agent can cut corners when moving diagonally, can only be set in the editor during runtime")]
        public bool m_CanCutCorners;

        /// <summary>
        /// Caps the max iteration count when calculating paths. Useful to stop infinite loops
        /// </summary>
        protected int m_MaxPathCount = 10000;

        [Tooltip("Changes the colour of the grid to show pathing")] [SerializeField]
        public bool m_Debug_ChangeTileColours;
        
        public List<Vector2> m_Path { get; protected set; }

        public PathFinding(bool allowDiagonal, bool cutCorners, bool debug_ChangeTileColours = false)
        {
            m_Path = new List<Vector2>();
            m_AllowDiagonal = allowDiagonal;
            m_CanCutCorners = cutCorners;
            m_GridNodes = TileGrid.GridNodes;
            m_Debug_ChangeTileColours |= debug_ChangeTileColours;
        }

        public abstract void GeneratePath(GridNode start, GridNode end);

        public Vector2 GetClosestPointOnPath(Vector2 position)
        {
            float distance = float.MaxValue;
            int closestPoint = int.MaxValue;

            for (int i = 0; i < m_Path.Count; ++i)
            {
                float tempDistance = Maths.Magnitude(m_Path[i] - position);
                if (tempDistance < distance)
                {
                    closestPoint = i;
                    distance = tempDistance;
                }
            }

            for (int j = 0; j <= closestPoint - 1; ++j)
            {
                m_Path.RemoveAt(0);
            }

            return m_Path[0];
        }

        public Vector2 GetNextPointOnPath(Vector2 position)
        {
            Vector2 pos = position;
            if (m_Path.Count > 0)
            {
                m_Path.RemoveAt(0);

                if (m_Path.Count > 0)
                    pos = m_Path[0];
            }

            return pos;
        }
    
        // Heuristic Algorithms
        // These all return the estimated cost for going in the ideal direction from start to end
        // The lower the cost, the more ideal the direction is
        // These all just calculate the ideal direction in different ways, some factor in diagonal, others don't etc.
        protected float Heuristic_Manhattan(GridNode start, GridNode end)
        {
            //distance between the player and target
            //don't care about z because 2D
            float totalDist = Mathf.Abs(end.transform.position.x - start.transform.position.x)
                              + Mathf.Abs(end.transform.position.y - start.transform.position.y);
            return totalDist;
        }

        protected float Heuristic_Euclidean(GridNode start, GridNode end)
        {
            //Pythagoras theorem states C^2 = a^2 + b^2 
            float a = end.transform.position.x - start.transform.position.x;
            float b = end.transform.position.y - start.transform.position.y;
            float c = Mathf.Pow(a, 2) + Mathf.Pow(b, 2);
            return Mathf.Sqrt(c);
        }

        protected float Heuristic_Octile(GridNode start, GridNode end)
        {
            // Essentially works like Manhattan but factors in diagonal movement
            //this is done by taking the shortest length of x or y (since you can't go diagonal for more than the shortest distance)
            //and multiplying it by the cost to go from straight to diagonal (sqrt(2) - 1)
            //that then returns the total diagonal distance cost 
            // The full cost can then be calculated by adding the larger of the two distances (x or y) (essentially the straight movement remaining)
            // to the total diagonal cost, which returns the full octile distance
        
            //Octile = max(dx, dy) + (sqrt(2) - 1) * min(dx, dy)
        
            //magnitude of diagonal movement
            float diagonalCost = Mathf.Sqrt(2);
            //magnitude of straight movement
            float straightCost = 1.0f;
            //x direction distance
            float xDist = Mathf.Abs(end.transform.position.x - start.transform.position.x);
            //y direction distance
            float yDist = Mathf.Abs(end.transform.position.y - start.transform.position.y);
        
            float octileDist = Mathf.Max(xDist,yDist) + (diagonalCost - straightCost) * Mathf.Min(xDist,yDist);
            return octileDist;
        }

        protected float Heuristic_Chebyshev(GridNode start, GridNode end)
        {
            // Assumes you can move in any of the 8 directions for a cost of 1.
            // Therefore, the cost is simply the length of the longest axis.
            // Essentially works like octile but diagonal costs the same as straight
            //Chebyshev = max(dx, dy)
        
            float xDist = Mathf.Abs(end.transform.position.x - start.transform.position.x);
            //y direction distance
            float yDist = Mathf.Abs(end.transform.position.y - start.transform.position.y);
        
            float chebyshevDist = Mathf.Max(xDist, yDist);
            return chebyshevDist;
        }
    }
}