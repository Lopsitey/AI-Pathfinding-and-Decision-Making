using UnityEngine;
using UnityEngine.Serialization;

namespace SteeringBehaviours
{
	public class SteeringBehaviourWander : SteeringBehaviour
	{
		[Header("Wander Properties")]
		[Header("Settings")]
		public float m_WanderRadius = 2; 
		public float m_WanderOffset = 2;
		public float m_AngleDisplacement = 15;

		Vector2 m_CirclePosition;
		Vector2 m_PointOnCircle;
		private float m_Angle;

		[FormerlySerializedAs("m_Debug_RadiusColour")]
		[Space(10)]

		[Header("Debugs")]
		[SerializeField]
		protected Color m_DebugRadiusColour = Color.yellow;
		[SerializeField]
		protected Color m_DebugEntityToCentreColour = Color.cyan;
		[SerializeField]
		protected Color m_DebugCentreToPointColour = Color.green;
		[SerializeField]
		protected Color m_DebugPointToEntityColour = Color.magenta;
    
		public override Vector2 CalculateForce()
		{
			//sets the circle position to slightly in front of the object
			Vector2 currentPosition = transform.position;
			m_CirclePosition = currentPosition + (Maths.Normalise(m_Manager.m_Entity.m_Velocity) * m_WanderOffset);
	    
			//gets a random angle between the lowest and highest angle displacement values 
			m_Angle += Random.Range(-m_AngleDisplacement, m_AngleDisplacement);
			float targetAngleRadians = m_Angle * Mathf.Deg2Rad;//converts to radians
		
			//uses trig to get the points on a circle
			float x = Mathf.Cos(targetAngleRadians) * m_WanderRadius;
			float y = Mathf.Sin(targetAngleRadians) * m_WanderRadius;
			//Adds the points onto the circle using a Vector2
			m_PointOnCircle = m_CirclePosition + new Vector2(x, y);
		
			//travel towards the point on the circle
			m_DesiredVelocity = m_PointOnCircle - currentPosition;

			//normalise for just the direction and then scale that to the max speed
			//essentially makes them move at max speed in the correct direction by default
			m_DesiredVelocity = m_Manager.m_Entity.m_MaxSpeed * Maths.Normalise(m_DesiredVelocity);
        
			//reducing the desired velocity by the current velocity gives us the direction to move in
			m_Steering = m_DesiredVelocity - m_Manager.m_Entity.m_Velocity;
			return m_Steering * m_Weight;//applies weight to the force - good for priority and making the behaviour more powerful
		}

		protected override void OnDrawGizmosSelected()
		{
			if (Application.isPlaying)
			{
				if (m_Debug_ShowDebugLines && m_Active && m_Manager.m_Entity)
				{
					Gizmos.color = m_DebugRadiusColour;
					Gizmos.DrawWireSphere(m_CirclePosition, m_WanderRadius);

					Gizmos.color = m_DebugEntityToCentreColour;
					Gizmos.DrawLine(transform.position, m_CirclePosition);

					Gizmos.color = m_DebugCentreToPointColour;
					Gizmos.DrawLine(m_CirclePosition, m_PointOnCircle);

					Gizmos.color = m_DebugPointToEntityColour;
					Gizmos.DrawLine(transform.position, m_PointOnCircle);

					base.OnDrawGizmosSelected();
				}
			}
		}
	}
}
