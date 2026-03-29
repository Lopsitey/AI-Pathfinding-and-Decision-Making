using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class PathfindingDijkstra : PathFinding.PathFinding
{
	[System.Serializable]
	class NodeInformation
	{
		public GridNode Node { get; private set; }
		public NodeInformation Parent { get; private set; }
		public float Cost { get; private set; }

		public NodeInformation(GridNode node, NodeInformation parent, float cost)
		{
			this.Node = node;
			this.Parent = parent;
			this.Cost = cost;
		}

		public void UpdateNodeInformation(NodeInformation parent, float cost)
		{
			this.Parent = parent;
			this.Cost = cost;
		}
	}

	public PathfindingDijkstra(bool allowDiagonal, bool cutCorners, bool debugChangeTileColours = false) : base(allowDiagonal, cutCorners, debugChangeTileColours) { }

	public override void GeneratePath(GridNode start, GridNode end)
	{
		//clears the current path
		m_Path.Clear();
		
		//lists to track visited and none visited nodes
		List<NodeInformation> visited = new List<NodeInformation>();
		List<NodeInformation> notVisited = new List<NodeInformation>();

		NodeInformation startingNode = new NodeInformation(start, null, 0);
		notVisited.Add(startingNode);

		NodeInformation current = startingNode;

		int maxIteration = 0;

		//loop while there is a node selected
		while (current != null)//current node is set to null if there is an error
		{
			maxIteration++;
			if (maxIteration > m_MaxPathCount)
			{
				Debug.LogError("Max Iteration Reached");
				break;
			}
			
			//should only be running if there are unvisited nodes
			if (notVisited.Count > 0)
			{
				//makes the cheapest unvisited node the current node
				current = GetCheapestNode(notVisited);
				notVisited.Remove(current);
				visited.Add(current);
			}
			else
			{
				//no path found - set current to null to exit loop
				current = null;
				break;
				//realistically, could just run the loop on true and only use break here
				//setting current is just the more verbose version
			}
			
			//if the target has been found
			if (current.Node == end)
			{
				Debug.Log($"Path found, start pos = {start.transform.position} - end pos = {end.transform.position}");
				//pass the final node so the path can be retraced in reverse
				SetPath(current);
				
				//Colours the path in using all the visited and unvisited nodes
				DrawPath(visited, notVisited);
				return;
			}

			//checks all 8 neighbours of the current tile, starting upwards (0) and then going clockwise
			for (int i = 0; i < 8; ++i)
			{
				//if moving diagonally - diagonal neighbours are always odd
				if (i % 2 != 0)
				{
					//skips diagonal neighbours
					if (!m_AllowDiagonal)
						continue;

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
						GridNode cardinalA = current.Node.Neighbours[cardinalA_Index];
						GridNode cardinalB = current.Node.Neighbours[cardinalB_Index];

						// If either is an obstacle then moving diagonally past them is corner clipping
						// Null may as well be obstacle as it should be void map that doesn't exist
						bool blockedA = (cardinalA == null || !cardinalA.m_Walkable);
						bool blockedB = (cardinalB == null || !cardinalB.m_Walkable);
						if (blockedA || blockedB)
							continue;
					}
				}
				
				GridNode neighbour = current.Node.Neighbours[i];
				
				// Ensures the neighbour is valid and hasn't already been visited - skips if invalid
				if(!neighbour || !neighbour.m_Walkable || visited.Any(x=> x.Node == neighbour))
					continue;
					
				//distance from the current node to the neighbour
				Vector3 distance = neighbour.transform.position - current.Node.transform.position;
				float totalCost = current.Cost + Maths.Magnitude(distance);
				
				//checks if neighbour has been found but not checked yet
				NodeInformation existingNode = notVisited.Find(x => x.Node == neighbour);
				if (existingNode == null)//if the node is completely unknown
				{
					//create one and add it to the checked list
					NodeInformation newNode = new NodeInformation(neighbour, current, totalCost);
					notVisited.Add(newNode);
				}
				else
				{//Already checked
					// Check again to see if the new path is cheaper than the old path
					if (totalCost < existingNode.Cost)
					{
						// Update the node with the new path information
						existingNode.UpdateNodeInformation(current, totalCost);
					}
				}
			}
        }
		
		//Runs if the while loop wasn't entered - means there was likely no start node
        Debug.LogError("No path found, start pos = " + start.transform.position + " - end pos = " + end.transform.position);
	}

	/// <summary>
	/// Passes the final node information and sets m_Path
	/// </summary>
	private void SetPath(NodeInformation end)
	{
		NodeInformation current = end;
		//while the start hasn't been added to the path yet
		while (current != null)
		{
			//add the current node found
			m_Path.Add(current.Node.transform.position);
			//add the parent
			current = current.Parent;
			//this retraces the path in reverse
		}

		//reverse the reversed path to get the forward path 
		m_Path.Reverse();
	}

	/// <summary>
	/// Returns the cheapest node in the list calculated by cost
	/// </summary>
	private NodeInformation GetCheapestNode(List<NodeInformation> nodes)
	{
		return nodes.OrderBy(n => n.Cost).First();
	}

	/// <summary>
	/// Changest the colour of the grid based on the values passed in
	/// </summary>
	void DrawPath(List<NodeInformation> visited, List<NodeInformation> notVisited)
	{
		//drawPath
		if (m_Debug_ChangeTileColours)
		{
			//Sets all the node grid colours to the new walkable colour
			TileGrid.ResetGridNodeColours();

			foreach (NodeInformation node in notVisited)
			{
				//applies the open colour to any not visited nodes
				node.Node.SetOpenInPathFinding();
			}

			foreach (NodeInformation node in visited)
			{
				//applies the closed colour to any visited nodes
				node.Node.SetClosedInPathFinding();
			}
		}
	}
}