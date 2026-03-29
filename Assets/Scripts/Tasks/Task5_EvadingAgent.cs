using System.Collections;
using System.Collections.Generic;
using SteeringBehaviours;
using UnityEngine;

public class Task5_EvadingAgent : MovingEntity
{
	SteeringBehaviourManager m_SteeringBehaviours;
	SteeringBehaviourEvade m_Evade;

	protected override void Awake()
	{
		base.Awake();

		m_SteeringBehaviours = GetComponent<SteeringBehaviourManager>();

		if (!m_SteeringBehaviours)
			Debug.LogError("Object doesn't have a Steering Behaviour Manager attached", this);

		m_Evade = GetComponent<SteeringBehaviourEvade>();

		if (!m_Evade)
			Debug.LogError("Object doesn't have a Seek Steering Behaviour attached", this);
	}

	protected void Start()
	{
		m_Evade.m_EvadedEntity = GameObject.Find("Player").GetComponent<MovingEntity>();
	}

	protected override Vector2 GenerateVelocity()
	{
		return m_SteeringBehaviours.GenerateSteeringForce();
	}
}
