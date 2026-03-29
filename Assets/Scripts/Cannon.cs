using UnityEngine;

public class Cannon : MonoBehaviour
{
    int m_MaxAmmo = 10;
    public int m_Ammo { get; private set; }

    float m_FireTime = 5f;
    float m_FireTimer = 0f;
    float m_TurnRate = 40f;

    float m_FireDistance = 5f;
    float m_FireAngle = 3f;

    Transform m_Target;

    public GameObject m_CannonBall;
    private MovingEntity m_Entity;

    public void Start()
    {
        m_Ammo = m_MaxAmmo;
        m_Entity = GetComponentInParent<MovingEntity>();
        if (!m_Entity) 
            Debug.LogError("Cannon is missing a MovingEntity component in its parent hierarchy.", this);
    }

    public void AimAndFire()
    {
        Aim();
        Fire();
    }

    public void SetTarget(Transform target)
    { 
        m_Target = target; 
    }

    void Aim()
    {
        if (m_Target == null) return;
        
        m_Entity.m_Attacking = true;
        
        Vector2 lineBetween = (m_Target.position - transform.position).normalized;

        float currentAngle = Mathf.Atan2(transform.up.y, transform.up.x);
        float targetAngle = Mathf.Atan2(lineBetween.y, lineBetween.x);
        float deltaAngle = Mathf.DeltaAngle(Mathf.Rad2Deg * currentAngle, Mathf.Rad2Deg * targetAngle);

        float maxTurnPerFrame = m_TurnRate * Time.fixedDeltaTime;
        deltaAngle = Mathf.Clamp(deltaAngle, -maxTurnPerFrame, maxTurnPerFrame);

        float newAngle = (Mathf.Rad2Deg * currentAngle) +  deltaAngle;

        transform.rotation = Quaternion.Euler(0, 0, newAngle - 90);
    }

    void Fire()
    {
        if (m_FireTimer > 0)
        {
            m_FireTimer -= Time.deltaTime;
            m_Entity.m_Attacking = false;
            return;
        }

        if(m_Target != null && m_Ammo > 0)
        {
            Vector2 lineBetween = (m_Target.position - transform.position);

            if(lineBetween.magnitude <= m_FireDistance)
            {
                lineBetween.Normalize();
                float angle = Mathf.Acos(Vector2.Dot(transform.up, lineBetween));

                if (angle <= m_FireAngle)
                {
                    Rigidbody2D rb = Instantiate(m_CannonBall, (Vector2)transform.position, Quaternion.identity).GetComponent<Rigidbody2D>();
                    Physics2D.IgnoreCollision(rb.GetComponent<Collider2D>(), transform.root.GetComponent<Collider2D>(), true);
                    rb.linearVelocity = lineBetween * 10f;
                    m_FireTimer = m_FireTime;
                    m_Ammo--;
                    Debug.LogError(" Fired Shot!\n\t\tAmmo Remaining: " + m_Ammo);
                }
            }
        }
        m_Entity.m_Attacking = false;
    }

    public void Reload()
    {
        m_Ammo = m_MaxAmmo;
    }
}
