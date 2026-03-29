#region

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

#endregion

namespace PathFinding
{
    [Serializable]
    public class Pathfinding_AStar : PathFinding
    {
        [Serializable]
        class NodeInformation
        {
            public GridNode node;
            public NodeInformation parent;
            public float gCost; // Distance from Start
            public float hCost; // Distance to End (Heuristic)
            public float fCost; // Total (G + H)

            public NodeInformation(GridNode node, NodeInformation parent, float gCost, float hCost)
            {
                this.node = node;
                this.parent = parent;
                this.gCost = gCost;
                this.hCost = hCost;
                fCost = gCost + hCost;
            }

            public void UpdateNodeInformation(NodeInformation parent, float gCost, float hCost)
            {
                this.parent = parent;
                this.gCost = gCost;
                this.hCost = hCost;
                fCost = gCost + hCost;
            }
        }

        public LayerMask m_ObstacleLayer;
        public ThetaStar m_ThetaStar;

        public Pathfinding_AStar(bool allowDiagonal, bool cutCorners, LayerMask obstacleLayer,
            bool debug_ChangeTileColours = false) :
            base(allowDiagonal, cutCorners, debug_ChangeTileColours)
        {
            m_ObstacleLayer = obstacleLayer;
            m_ThetaStar = new ThetaStar(m_ObstacleLayer);
        }

        /// <summary>
        /// Gets the correct heuristic algorithm depending on whether diagonal movement is allowed
        /// </summary>
        /// <param name="a">The agent start point</param>
        /// <param name="b">The target</param>
        /// <returns>Octile for diagonal movement, Manhattan for straight only</returns>
        private float GetHeuristic(GridNode a, GridNode b) =>
            m_AllowDiagonal ? Heuristic_Octile(a, b) : Heuristic_Manhattan(a, b);

        public override void GeneratePath(GridNode start, GridNode end)
        {
            // Clears the current path
            m_Path.Clear();

            // Lists to track the open (to-do) and closed (done) nodes
            List<NodeInformation> openList = new List<NodeInformation>();
            List<NodeInformation> closedList = new List<NodeInformation>();

            float initialH = GetHeuristic(start, end);
            NodeInformation startingNode = new NodeInformation(start, null, 0, initialH);
            openList.Add(startingNode);

            NodeInformation current = startingNode;

            int maxIteration = 0;

            // Loop while there are nodes to investigate
            while (current != null)
            {
                maxIteration++;
                if (maxIteration > m_MaxPathCount)
                {
                    Debug.LogError("Max Iteration Reached");
                    break;
                }

                // Checks if there are nodes to process
                if (openList.Count > 0)
                {
                    // Get the node with the LOWEST F-Cost
                    current = GetCheapestNode(openList);
                    //F - cost used because it includes the total cost and the ideal heuristic direction cost
                    openList.Remove(current);
                    closedList.Add(current);
                }
                else
                {
                    // Open list is empty but target not found = Path Impossible
                    current = null;
                    break;
                }

                // Stops if the target has been found
                if (current.node == end)
                {
                    Debug.Log(
                        $"Path found, start pos = {start.transform.position} - end pos = {end.transform.position}");
                    SetPath(current);
                    DrawPath(openList, closedList);
                    return;
                }

                //Check neighbours
                for (int i = 0; i < 8; ++i)
                {
                    //starts identical to Dijkstra
                    //if moving diagonally - diagonal neighbours are always odd
                    if (i % 2 != 0)
                    {
                        //skips diagonal neighbours
                        if (!m_AllowDiagonal) continue;

                        if (!m_CanCutCorners)
                        {
                            // The neighbours which would be directly above/below or left/right of the current node
                            // Makes even for the cardinal directions - n/s and e/w
                            int cardinalA_Index = i - 1;
                            int cardinalB_Index = i + 1;
                            // You can't clip  a corner if the corner is diagonal to you (when moving diagonally)

                            // Handles wrap-around for the final odd diagonal - converts 8 which doesn't exist to 0
                            if (cardinalB_Index == 8) cardinalB_Index = 0;

                            // Gets the actual nodes from the cardinal directions
                            GridNode cardinalA = current.node.Neighbours[cardinalA_Index];
                            GridNode cardinalB = current.node.Neighbours[cardinalB_Index];

                            // If either is an obstacle then moving diagonally past them is corner clipping
                            // Null may as well be obstacle as it should be void map that doesn't exist
                            bool blockedA = (cardinalA == null || !cardinalA.m_Walkable);
                            bool blockedB = (cardinalB == null || !cardinalB.m_Walkable);
                            if (blockedA || blockedB)
                                continue;
                        }
                    }

                    GridNode neighbour = current.node.Neighbours[i];

                    // Ensures the neighbour is valid and hasn't already been checked - skips if invalid
                    if (!neighbour || !neighbour.m_Walkable || closedList.Any(x => x.node == neighbour))
                        continue;

                    float newGCost;
                    //Could be current or grandparent
                    NodeInformation potentialParent = current;

                    // The previous node (current) can be skipped if it's parent has a line of sight to the neighbour being checked
                    if (current.parent != null && m_ThetaStar.HasLineOfSight(current.parent.node, neighbour))
                    {
                        //Skips the prior node entirely
                        potentialParent = current.parent;

                        //The distance from the grandparent to the neighbour
                        Vector3 grandparentDist =
                            neighbour.transform.position - potentialParent.node.transform.position;
                        //Adds it to the total G cost (Distance from start to the grandparent)
                        newGCost = potentialParent.gCost + Maths.Magnitude(grandparentDist);
                    }
                    else
                    {
                        // Distance from the current node to the neighbour
                        Vector3 distanceVector = neighbour.transform.position - current.node.transform.position;
                        // Adds it to the total G cost (Distance from start to this neighbour)
                        newGCost = current.gCost + Maths.Magnitude(distanceVector);
                    }

                    // Calculates H-Cost (Heuristic distance from neighbour to end)
                    float neighbourHCost = current.hCost = GetHeuristic(neighbour, end);

                    // Checks if this node has been found but not checked yet
                    NodeInformation existingNode = openList.Find(x => x.node == neighbour);
                    if (existingNode == null) //if the node is completely unknown
                    {
                        // Create new node with the POTENTIAL parent (could be current, or grandparent)
                        //create one and add it to the checked list
                        NodeInformation newNode =
                            new NodeInformation(neighbour, potentialParent, newGCost, neighbourHCost);
                        openList.Add(newNode);
                    }
                    else
                    {
                        //Already checked
                        // Check again to see if the new path is cheaper than the old path
                        // Compares the G-Cost (the actual distance travelled so far)
                        if (newGCost < existingNode.gCost) //update the node if cheaper
                            existingNode.UpdateNodeInformation(potentialParent, newGCost, neighbourHCost);
                    }
                }
            }

            //Runs if the while loop wasn't entered - means there was likely no start node
            Debug.LogError("No path found, start pos = " + start.transform.position + " - end pos = " +
                           end.transform.position);
        }

        /// <summary>
        /// pass in the final node information and sets m_Path
        /// </summary>
        private void SetPath(NodeInformation end)
        {
            NodeInformation current = end;
            while (current != null)
            {
                m_Path.Add(current.node.transform.position);
                current = current.parent;
            }

            m_Path.Reverse();
        }

        /// <summary>
        /// Returns the cheapest node in the list calculated by cost
        /// </summary>
        private NodeInformation GetCheapestNode(List<NodeInformation> nodes)
        {
            //gets the node with the cheapest fCost (total cost including heuristic)
            return nodes.OrderBy(n => n.fCost).First();
        }

        /// <summary>
        /// Changes the colour of the grid based on the values passed in
        /// </summary>
        void DrawPath(List<NodeInformation> open, List<NodeInformation> closed)
        {
            //drawPath
            if (m_Debug_ChangeTileColours)
            {
                TileGrid.ResetGridNodeColours();

                foreach (NodeInformation node in closed)
                {
                    node.node.SetOpenInPathFinding();
                }

                foreach (NodeInformation node in open)
                {
                    node.node.SetClosedInPathFinding();
                }
            }
        }
    }
}