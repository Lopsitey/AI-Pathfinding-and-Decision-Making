using System.Collections;
using System.Collections.Generic;
using SteeringBehaviours;
using UnityEngine;

public class SeekandFlee : MovingEntity
{
	SteeringBehaviourManager m_SteeringBehaviours;
	SteeringBehaviourFlee m_Flee;
	SteeringBehaviourSeek m_Seek;

    protected override void Awake()
	{
		base.Awake();

		m_Flee = GetComponent<SteeringBehaviourFlee>();

		m_SteeringBehaviours = GetComponent<SteeringBehaviourManager>();

		if (!m_SteeringBehaviours)
			Debug.LogError("Object doesn't have a Steering Behaviour Manager attached", this);

		if (!m_Flee)
			Debug.LogError("Object doesn't have a Seek Steering Behaviour attached", this);

        m_Seek = GetComponent<SteeringBehaviourSeek>();

        if (!m_Seek)
            Debug.LogError("Object doesn't have a Seek Steering Behaviour attached", this);
    }

	protected void Start()
	{
		m_Flee.m_FleeTarget = GameObject.Find("Player").transform;
	}

	protected override Vector2 GenerateVelocity()
	{
		return m_SteeringBehaviours.GenerateSteeringForce();
	}

    private void Update()
    {
        if (Maths.Magnitude((Vector2)transform.position - m_Seek.m_TargetPosition) < 0.5f)
        {
            m_Seek.m_TargetPosition = TileGrid.GetRandomWalkableTile(2).transform.position;
        }
    }
}
