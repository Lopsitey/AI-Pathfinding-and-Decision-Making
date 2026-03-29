using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

[System.Serializable]
public class Pathfinding_JPS : PathFinding.PathFinding
{
	[System.Serializable]
	class NodeInformation
	{
		public GridNode node;
		public NodeInformation parent;
		public float gCost;
		public float hCost;
		public float fCost;

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

	public Pathfinding_JPS(bool allowDiagonal, bool cutCorners, bool debug_ChangeTileColours = false) : base(allowDiagonal, cutCorners, debug_ChangeTileColours) { }

	public override void GeneratePath(GridNode start, GridNode end)
	{
		//clears the current path
		m_Path.Clear();

		//lists to track the open and closed nodes
		List<NodeInformation> openList = new List<NodeInformation>();
		List<NodeInformation> closedList = new List<NodeInformation>();

		NodeInformation startingNode = new NodeInformation(start, null, 0, Heuristic_Manhattan(start, end));
		openList.Add(startingNode);

		NodeInformation current = startingNode;

		int maxIternation = 0;

		//loop while there is a node selected
		while (current != null)
		{
			maxIternation++;
			if (maxIternation > m_MaxPathCount)
			{
				Debug.LogError("Max Iteration Reached");
				break;
			}

            //delete me
            //Write JPS Algorithm here.
        }

        Debug.LogError("No path found, start pos = " + start.transform.position + " - end pos = " + end.transform.position);
	}


	private List<NodeInformation> FindSuccessors(NodeInformation node, GridNode start, GridNode end, ref List<NodeInformation> openList, ref List<NodeInformation> closedList)
	{

		#pragma warning disable CS0219 // Variable is assigned but its value is never used
		GridNode jumpNode = null;
		#pragma warning restore CS0219 // Variable is assigned but its value is never used
		List<NodeInformation> successors = new List<NodeInformation>();

		//delete me
		//Write this function.

		return successors;
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
		return nodes.OrderBy(n => n.fCost).First();
	}

	/// <summary>
	/// Changest the colour of the grid based on the values passed in
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

