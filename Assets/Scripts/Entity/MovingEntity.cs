using UnityEngine;

public abstract class MovingEntity : Entity
{
    Rigidbody2D m_Rigidbody;
    public float m_MaxSpeed = 15.0f;

    public bool m_PerfectTurning = false;
    public float m_MaxTurnDegrees = 90f;
    
    public bool m_Attacking;
    public bool m_CanMoveWhileAttacking;

    public Vector2 m_Velocity { get { return m_Rigidbody.linearVelocity; } }

	protected override void Awake()
	{
		base.Awake();

        m_Rigidbody = GetComponent<Rigidbody2D>();
	}

	protected abstract Vector2 GenerateVelocity();

    protected virtual void FixedUpdate()
	{
        MoveAndRotate();
	}

	protected void MoveAndRotate()
	{
		//Prevent movement if attacking and not allowed to move while attacking.
		if(!m_CanMoveWhileAttacking && m_Attacking) return;
		
        Vector2 force = GenerateVelocity();

        if(force == Vector2.zero) { return; }

        Vector2 normalisedForce = force.normalized;

        if (!m_PerfectTurning)
        {
            float currentAngle = Mathf.Atan2(transform.up.y, transform.up.x);
            float targetAngle = Mathf.Atan2(normalisedForce.y, normalisedForce.x);
            float deltaAngle = Mathf.DeltaAngle(Mathf.Rad2Deg * currentAngle, Mathf.Rad2Deg * targetAngle);

            float maxTurnPerFrame = m_MaxTurnDegrees * Time.fixedDeltaTime;
            deltaAngle = Mathf.Clamp(deltaAngle, -maxTurnPerFrame, maxTurnPerFrame);

            float newAngle = currentAngle + Mathf.Deg2Rad * deltaAngle;
            Vector2 newDirection = new Vector2(Mathf.Cos(newAngle), Mathf.Sin(newAngle));

            transform.rotation = Quaternion.Euler(0, 0, Mathf.Rad2Deg * newAngle - 90);

            force = newDirection * force.magnitude;
        }
        
        Vector2 acceleration = force / m_Rigidbody.mass;

        m_Rigidbody.linearVelocity += acceleration * Time.fixedDeltaTime;
        m_Rigidbody.linearVelocity = Vector2.ClampMagnitude(m_Rigidbody.linearVelocity, m_MaxSpeed);

        if(m_PerfectTurning)
        {
            transform.up = m_Rigidbody.linearVelocity.normalized;
        }
    }
}
