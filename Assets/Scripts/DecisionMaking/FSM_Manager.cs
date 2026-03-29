#region

using DecisionMaking.States;
using SteeringBehaviours;
using UnityEngine;

#endregion

namespace DecisionMaking
{
    public class FSM_Manager : DecisionMaking
    {
        public SteeringBehaviourWander m_Wander;
        public SteeringBehaviourArrive m_Arrive;
        public SteeringBehaviourEvade m_Evade;

        public Vector2 m_PickupTarget;

        private RunAwayState m_Run;
        private CollectPickupState m_Collect;
        private IdleState m_Idle;

        private StateBase m_CurrentState;

        private Task13_DecisionMaking m_Task13;

        protected override void Awake()
        {
            base.Awake();

            # region Get Components

            m_Wander = GetComponent<SteeringBehaviourWander>();

            if (!m_Wander)
                Debug.LogError("Object doesn't have a Wander Steering Behaviour attached", this);

            m_Arrive = GetComponent<SteeringBehaviourArrive>();

            if (!m_Arrive)
                Debug.LogError("Object doesn't have an Arrive Steering Behaviour attached", this);

            m_Evade = GetComponent<SteeringBehaviourEvade>();

            if (!m_Evade)
                Debug.LogError("Object doesn't have an Evade Steering Behaviour attached", this);

            m_Task13 = GetComponent<Task13_DecisionMaking>();

            if (!m_Task13)
                Debug.LogError("Object doesn't have a Task13_DecisionMaking attached", this);

            #endregion

            // States are plain C# classes that use a ref to this script - no extra comps needed.
            m_Idle = new IdleState(this);
            m_Collect = new CollectPickupState(this);
            m_Run = new RunAwayState(this);
        }

        // Needed because SwitchState only works after everything has been initialised in Awake.
        private void Start()
        {
            Pickup.PickUpCollected += ResetPickups;
            SwitchState(m_Idle);
        }

        private void SwitchState(StateBase newState)
        {
            // Ignores invalid states and prevents swapping to the same state.
            if (newState == null || m_CurrentState == newState)
                return;

            m_CurrentState = newState;
            m_CurrentState.Enter();
        }

        public override void CustomUpdate()
        {
            Vector2 myPos = transform.position;

            bool hasPickup = false;
            if (m_Task13) CheckPickups(ref hasPickup, in myPos);

            Task7_WanderingAgent closestEnemy = null;
            if (!hasPickup) ScanEnemies(ref closestEnemy, in myPos);

            StateBase desiredState;
            // Pickups have the highest priority
            if (hasPickup)
                desiredState = m_Collect;
            // Evades any enemies found in the scan range
            else if (closestEnemy)
                desiredState = m_Run;
            else
                desiredState = m_Idle;

            SwitchState(desiredState);
            // Only pass the enemy target when evading - ensures the closest enemy stays updated.
            m_CurrentState?.UpdateAgent(m_CurrentState == m_Run ? closestEnemy : null);
        }

        /// <summary>
        /// Find the closest enemy among scanned enemies - scan range is pre-defined in Task13_DecisionMaking.
        /// </summary>
        /// <param name="closestEnemy"></param>
        /// <param name="myPos"></param>
        private void ScanEnemies(ref Task7_WanderingAgent closestEnemy, in Vector2 myPos)
        {
            float shortestDistance = float.MaxValue;

            foreach (var enemy in Enemies)
            {
                //If the enemy has been destroyed, skip it
                if (!enemy) continue;

                Vector2 enemyPos = enemy.transform.position;

                float enemyDist = (enemyPos - myPos).sqrMagnitude; //More efficient than Maths.Magnitude()

                //If the enemy is closer than the previous closest, set it as the closest
                if (enemyDist < shortestDistance)
                {
                    shortestDistance = enemyDist;
                    closestEnemy = enemy;
                }
            }
        }

        /// <summary>
        /// Checks for available pickups and sets the closest one as the target.
        /// </summary>
        /// <param name="foundPickup"></param>
        /// <param name="myPos"></param>
        private void CheckPickups(ref bool foundPickup, in Vector2 myPos)
        {
            float healthDist = float.MaxValue;
            float ammoDist = float.MaxValue;

            bool hasHealth = m_Task13.m_HealthPickupLocation != Vector2.zero;
            bool hasAmmo = m_Task13.m_AmmoPickupLocation != Vector2.zero;

            if (hasHealth)
                healthDist = (m_Task13.m_HealthPickupLocation - myPos).sqrMagnitude;
            if (hasAmmo)
                ammoDist = (m_Task13.m_AmmoPickupLocation - myPos).sqrMagnitude;

            if (hasHealth || hasAmmo)
            {
                foundPickup = true;
                // Chooses the closest pickup - prefers health if both are equally distant
                m_PickupTarget = healthDist <= ammoDist
                    ? m_Task13.m_HealthPickupLocation
                    : m_Task13.m_AmmoPickupLocation;
            }
        }

        /// <summary>
        /// Ensures that pickup locations are reset when a pickup is collected.
        /// </summary>
        private void ResetPickups()
        {
            m_Task13.m_HealthPickupLocation = Vector2.zero;
            m_Task13.m_AmmoPickupLocation = Vector2.zero;
        }
    }
}