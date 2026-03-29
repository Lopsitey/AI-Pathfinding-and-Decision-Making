using System.Collections;
using System.Collections.Generic;
using SteeringBehaviours;
using UnityEngine;

public class Task4_PursuitingAgent : MovingEntity
{
	SteeringBehaviourManager m_SteeringBehaviours;
	SteeringBehaviourPursuit m_Pursuit;

	protected override void Awake()
	{
		base.Awake();

		m_SteeringBehaviours = GetComponent<SteeringBehaviourManager>();

		if (!m_SteeringBehaviours)
			Debug.LogError("Object doesn't have a Steering Behaviour Manager attached", this);

		m_Pursuit = GetComponent<SteeringBehaviourPursuit>();

		if (!m_Pursuit)
			Debug.LogError("Object doesn't have a Seek Steering Behaviour attached", this);
	}

	protected void Start()
	{
		m_Pursuit.m_PursuedEntity = GameObject.Find("Player").GetComponent<MovingEntity>();
	}

	protected override Vector2 GenerateVelocity()
	{
		return m_SteeringBehaviours.GenerateSteeringForce();
	}
}
