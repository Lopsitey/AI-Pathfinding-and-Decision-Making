using System.Collections;
using System.Collections.Generic;
using PathFinding;
using SteeringBehaviours;
using UnityEngine;

public class Task11_AStarPathfinding : MovingEntity
{
	SteeringBehaviourManager m_SteeringBehaviours;
	SteeringBehaviourSeek m_Seek;

	[SerializeField]
	Pathfinding_AStar m_AStar;

	[Header("Debug")]
	[Tooltip("Draws the path of the agent")]
	[SerializeField]
	bool m_Debug_DrawPath;

	protected override void Awake()
	{
		base.Awake();

		m_SteeringBehaviours = GetComponent<SteeringBehaviourManager>();

		if (!m_SteeringBehaviours)
			Debug.LogError("Object doesn't have a Steering Behaviour Manager attached", this);

		m_Seek = GetComponent<SteeringBehaviourSeek>();

		if (!m_Seek)
			Debug.LogError("Object doesn't have a Seek Steering Behaviour attached", this);

		m_AStar = new Pathfinding_AStar(m_AStar.m_AllowDiagonal, m_AStar.m_CanCutCorners, m_AStar.m_ObstacleLayer, m_AStar.m_Debug_ChangeTileColours);
	}

	protected override Vector2 GenerateVelocity()
	{
		return m_SteeringBehaviours.GenerateSteeringForce();
	}

	protected void Update()
	{
		if (m_AStar.m_Path.Count == 0)
		{
			Rect size = TileGrid.m_GridSize;
			float x1 = Random.Range(size.xMin, size.xMax);
			float y1 = Random.Range(size.yMin, size.yMax);

			m_AStar.GeneratePath(TileGrid.GetNodeClosestWalkableToLocation(transform.position), TileGrid.GetNodeClosestWalkableToLocation(new Vector2(x1, y1)));
		}
		else
		{
			if (m_AStar.m_Path.Count > 0)
			{
				Vector2 closestPoint = m_AStar.GetClosestPointOnPath(transform.position);

				if (Maths.Magnitude(closestPoint - (Vector2)transform.position) < 0.5f)
					closestPoint = m_AStar.GetNextPointOnPath(transform.position);

				m_Seek.m_TargetPosition = closestPoint;
			}
		}
	}

	void OnDrawGizmosSelected()
	{
		if (Application.isPlaying)
		{
			if (m_Debug_DrawPath)
			{
				Gizmos.DrawLine(transform.position, m_Seek.m_TargetPosition);

				if (m_AStar.m_Path.Count > 1)
				{
					for (int i = 0; i < m_AStar.m_Path.Count - 1; ++i)
					{
						Gizmos.DrawLine(m_AStar.m_Path[i], m_AStar.m_Path[i + 1]);
					}
				}
			}
		}
	}
}
