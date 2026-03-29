using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity : MonoBehaviour
{
    protected Animator m_Animator;
    protected SpriteRenderer m_Renderer;
    public Health m_Health { get; private set; }
    public GameObject m_DeathExplosion;

    protected virtual void Awake()
    {
        m_Animator = GetComponent<Animator>();
        m_Renderer = GetComponent<SpriteRenderer>();

       m_Health = GetComponent<Health>();
        if (m_Health != null)
        {
            m_Health.OnHealthDepleted += TriggerDeath;
            m_Health.OnHealthChanged += UpdateHealthAnimation;
        }
    }

    protected virtual void TriggerDeath()
    {
        if (m_DeathExplosion)
        {
            Instantiate(m_DeathExplosion, transform.position, Quaternion.identity);
        }

        DestroyEntity();
    }

    public virtual void DestroyEntity()
    {
        Destroy(gameObject);
    }

    void UpdateHealthAnimation(int max, int current)
    {
        if (m_Animator)
        {
            float healthRatio = (float)current / (float)max;
            m_Animator.SetFloat("health", healthRatio);
        }
    }
}
