using System.Collections;
using System.Collections.Generic;
using SteeringBehaviours;
using UnityEngine;

public class Task3_FleeingAgent : MovingEntity
{
	SteeringBehaviourManager m_SteeringBehaviours;
	SteeringBehaviourFlee m_Flee;

	protected override void Awake()
	{
		base.Awake();

		m_Flee = GetComponent<SteeringBehaviourFlee>();

		m_SteeringBehaviours = GetComponent<SteeringBehaviourManager>();

		if (!m_SteeringBehaviours)
			Debug.LogError("Object doesn't have a Steering Behaviour Manager attached", this);

		if (!m_Flee)
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
}
