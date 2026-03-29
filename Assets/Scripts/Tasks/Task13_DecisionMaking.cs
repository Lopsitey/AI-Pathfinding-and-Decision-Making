using System;
using System.Collections.Generic;
using System.Linq;
using SteeringBehaviours;
using UnityEngine;

public class Task13_DecisionMaking : MovingEntity
{
    SteeringBehaviourManager m_SteeringBehaviours;
    DecisionMaking.DecisionMaking m_DecisionMaking;

    float m_ScanRange = 5f;
    public int m_Team;

    public Vector2 m_HealthPickupLocation = Vector2.positiveInfinity;
    public Vector2 m_AmmoPickupLocation = Vector2.positiveInfinity;

    protected override void Awake()
    {
        base.Awake();

        m_SteeringBehaviours = GetComponent<SteeringBehaviourManager>();

        if (!m_SteeringBehaviours)
            Debug.LogError("Object doesn't have a Steering Behaviour Manager attached", this);

        m_DecisionMaking = GetComponent<DecisionMaking.DecisionMaking>();

        if (!m_DecisionMaking)
            Debug.LogError("Object doesn't have a decision-making system attached", this);

        PickupManager.OnPickUpSpawned += PickupSpawned;

        Health health = GetComponent<Health>();

        if (!health)
            Debug.LogError("Object doesn't have a health system attached", this);

        health.OnHealthDepleted += OnDeath;
    }

    protected override Vector2 GenerateVelocity()
    {
        return m_SteeringBehaviours.GenerateSteeringForce();
    }

    // Update is called once per frame
    public void Update()
    {
        Scan();
        if(m_DecisionMaking)
            m_DecisionMaking.CustomUpdate();
    }


    protected void Scan()
    {
        List<Task7_WanderingAgent> enemies = new List<Task7_WanderingAgent>();

        Collider2D[] entities = Physics2D.OverlapCircleAll(transform.position, m_ScanRange);

        if (entities.Length > 0)
        {
            foreach (var entity in entities)
            {
                // Ignores the player as they shouldn't have this component
                if (entity.TryGetComponent(out Task7_WanderingAgent enemy))
                    enemies.Add(enemy);
                
                /*
                Task13_DecisionMaking entity = entities[i].GetComponent<Task13_DecisionMaking>();
                if (entity != null && entity.m_Team != m_Team)
                {
                    enemies.Add(entity);
                }
                */
            }
        }

        if (m_DecisionMaking)
            m_DecisionMaking.UpdateEnemyList(enemies);
    }

    void PickupSpawned(Vector2 health, Vector2 ammo)
    {
        m_HealthPickupLocation = health;
        m_AmmoPickupLocation = ammo;
    }

    void OnDeath()
    {
        if (m_Team != 0)
            PlayerUI.OnScoreUpdate.Invoke();
    }
}