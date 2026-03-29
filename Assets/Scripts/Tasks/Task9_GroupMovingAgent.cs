using System.Collections;
using System.Collections.Generic;
using SteeringBehaviours;
using UnityEngine;

public class Task9_GroupMovingAgent : MovingEntity
{
	SteeringBehaviourManager m_SteeringBehaviours;
	SteeringBehaviourSeek m_Seek;
	SteeringBehaviourSeparation m_Separation;
	SteeringBehaviourCohesion m_Cohesion;
	SteeringBehaviourAlignment m_Alignment;

	GameObject m_SeekingPosition;

	protected override void Awake()
	{
		base.Awake();

		m_SteeringBehaviours = GetComponent<SteeringBehaviourManager>();

		if (!m_SteeringBehaviours)
			Debug.LogError("Object doesn't have a Steering Behaviour Manager attached", this);

		m_Seek = GetComponent<SteeringBehaviourSeek>();

		if (!m_Seek)
			Debug.LogError("Object doesn't have a Seek Steering Behaviour attached", this);

		m_Separation = GetComponent<SteeringBehaviourSeparation>();

		if (!m_Separation)
			Debug.LogError("Object doesn't have a Seperation Steering Behaviour attached", this);

		m_Cohesion = GetComponent<SteeringBehaviourCohesion>();

		if (!m_Cohesion)
			Debug.LogError("Object doesn't have a Cohesion Steering Behaviour attached", this);

		m_Alignment = GetComponent<SteeringBehaviourAlignment>();

		if (!m_Alignment)
			Debug.LogError("Object doesn't have a Alignment Steering Behaviour attached", this);
	}

	protected override Vector2 GenerateVelocity()
	{
		return m_SteeringBehaviours.GenerateSteeringForce();
	}

    private void Start()
    {
		m_SeekingPosition = GameObject.Find("Seeking Position");
    }

    private void Update()
    {
		m_Seek.m_TargetPosition = m_SeekingPosition.transform.position;
    }
}
