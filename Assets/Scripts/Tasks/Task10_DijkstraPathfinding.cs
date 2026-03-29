using SteeringBehaviours;
using UnityEngine;

public class Task10_DijkstraPathfinding : MovingEntity
{
	SteeringBehaviourManager m_SteeringBehaviours;
	SteeringBehaviourSeek m_Seek;
	
	[SerializeField]
	PathfindingDijkstra m_Dijkstra;
	
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

		m_Dijkstra = new PathfindingDijkstra(m_Dijkstra.m_AllowDiagonal, m_Dijkstra.m_CanCutCorners, m_Dijkstra.m_Debug_ChangeTileColours);
	}

	protected override Vector2 GenerateVelocity()
	{
		return m_SteeringBehaviours.GenerateSteeringForce();
	}

	protected void Update()
	{
		if (m_Dijkstra.m_Path.Count == 0)
		{
			GridNode destination =  TileGrid.GetRandomWalkableTile(2);

			m_Dijkstra.GeneratePath(TileGrid.GetNodeClosestWalkableToLocation(transform.position), destination);
		}
		else
		{
			if (m_Dijkstra.m_Path.Count > 0)
			{
				Vector2 closestPoint = m_Dijkstra.GetClosestPointOnPath(transform.position);

				if (Maths.Magnitude(closestPoint - (Vector2)transform.position) < 0.5f)
					closestPoint = m_Dijkstra.GetNextPointOnPath(transform.position);

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

				if (m_Dijkstra.m_Path.Count > 1)
				{
					for (int i = 0; i < m_Dijkstra.m_Path.Count - 1; ++i)
					{
						Gizmos.DrawLine(m_Dijkstra.m_Path[i], m_Dijkstra.m_Path[i + 1]);
					}
				}
			}
		}
	}
}
