using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MovingEntity
{
    public float m_Acceleration = 100;

    //input
    float m_Horizontal;
    float m_Vertical;

    void Update()
    {
        m_Horizontal = Input.GetAxis("Horizontal");
        m_Vertical = Input.GetAxis("Vertical");
    }

    public void StopAttack()
	{
        m_Attacking = false;
	}

	protected override Vector2 GenerateVelocity()
	{
        return new Vector2(m_Horizontal, m_Vertical) * m_Acceleration;
    }
}
