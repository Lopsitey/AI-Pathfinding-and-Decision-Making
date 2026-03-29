#region

using System.Collections.Generic;
using DecisionMaking.MarkovStates;
using SteeringBehaviours;
using UnityEngine;

#endregion

namespace DecisionMaking
{
    public class MarkovManager : DecisionMaking
    {
        public SteeringBehaviourWander m_Wander;
        public SteeringBehaviourArrive m_Arrive;
        public SteeringBehaviourEvade m_Evade;

        private Task13_DecisionMaking m_Task13;

        [SerializeField] private List<MarkovStateBase> m_AllStates;
        private List<MarkovStateBase> m_InactiveStates;
        private List<MarkovStateBase> m_ActiveStates;

        [SerializeField] private AnimationCurve m_CloseEnemiesCurve;
        [SerializeField] private AnimationCurve m_CurrentAmmoCurve;
        [SerializeField] private AnimationCurve m_CurrentHealthCurve;

        private Cannon m_Cannon;
        private Health m_Health;

        //For debugging - used to prevent spamming the console
        private float m_NextLogTime = 0f;

        protected override void Awake()
        {
            base.Awake();
            
            // So the debugs can be throttled
            Application.targetFrameRate = 120;

            if (TryGetComponent(out Task13_DecisionMaking decisionMaking))
                m_Task13 = decisionMaking;
            else
                Debug.LogError("Object doesn't have a Task13_DecisionMaking attached", this);

            m_Cannon = GetComponentInChildren<Cannon>();
            if (!m_Cannon)
                Debug.LogError("Object doesn't have a cannon", this);

            //Tried a new method to display the error message
            m_Health = TryGetComponent(out Health health)
                ? health
                : throw new MissingComponentException("Object doesn't have a health system attached");

            m_ActiveStates = new List<MarkovStateBase>(m_AllStates.Count);
            m_InactiveStates = new List<MarkovStateBase>(m_AllStates.Count);
        }

        // Subscribes here so the task 13 awake logic is run first.
        private void Start() => Pickup.PickUpCollected += ResetPickups;

        public override void CustomUpdate()
        {
            Vector2 myPos = transform.position;

            // Pickups have the highest priority
            Vector2 pickupTarget = Vector2.positiveInfinity;
            if (m_Task13) CheckPickups(out pickupTarget, in myPos);

            // Evades any enemies found in the scan range
            Task7_WanderingAgent closestEnemy = null;
            ScanEnemies(ref closestEnemy, in myPos);

            // Defaults to idle
            CheckStates(closestEnemy, in pickupTarget);
            UpdateStates();
        }

        /// <summary>
        /// Assesses the activation levels of all states to determine which should be active.
        /// </summary>
        private void CheckStates(MovingEntity closestEnemy, in Vector2 pickupTarget)
        {
            if (m_AllStates == null || m_AllStates.Count == 0) return;

            //Store previous active states for comparison
            List<MarkovStateBase> previousActiveStates = new List<MarkovStateBase>(m_ActiveStates);
            //Deactivate all states for reassessment
            if (m_ActiveStates != null)
                m_ActiveStates.Clear();

            if (m_InactiveStates != null)
                m_InactiveStates.Clear();

            foreach (var state in m_AllStates)
            {
                // Prevents iteration over null states
                if (!state) continue;

                if (state.CalculateActivation(closestEnemy, pickupTarget) > 0)
                    m_ActiveStates?.Add(state);
                else
                    m_InactiveStates?.Add(state);
            }

            // These are called here because some states will have just been decided inactive/active, after checking their activation.
            DeactivateStates(previousActiveStates);
            ActivateStates(previousActiveStates);
        }

        /// <summary>
        /// Exits any states that were active in the previous frame but are now inactive.
        /// Also moves states that were already inactive to the new inactive list.
        /// </summary>
        private void DeactivateStates(List<MarkovStateBase> previousActiveStates)
        {
            if (m_InactiveStates.Count == 0 || previousActiveStates.Count == 0) return;

            foreach (var inactiveState in m_InactiveStates)
            {
                foreach (var prevState in previousActiveStates)
                {
                    if (inactiveState == prevState)
                        inactiveState.Exit();
                }
            }
        }

        /// <summary>
        /// Enters any states that were inactive in the previous frame but should now be active.
        /// Also moves states that were already active to the new active list.
        /// </summary>
        private void ActivateStates(List<MarkovStateBase> previousActiveStates)
        {
            if (m_ActiveStates.Count == 0) return;
            foreach (var activeState in m_ActiveStates)
            {
                bool alreadyActive = false;
                foreach (var prevState in previousActiveStates)
                {
                    if (activeState == prevState)
                    {
                        alreadyActive = true;
                        break; //stops when a match is found
                    }
                }

                //Only enter states that haven't been activated yet
                if (!alreadyActive)
                    activeState.Enter();
            }
        }

        /// <summary>
        /// If any states are active, update all of them. 
        /// </summary>
        private void UpdateStates()
        {
            if (m_ActiveStates.Count <= 0) return;
            foreach (var state in m_ActiveStates)
                state.UpdateAgent();
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

            if (!m_Task13.m_Attacking && closestEnemy && m_Cannon.m_Ammo > 0)
            {
                m_Cannon.SetTarget(closestEnemy.transform);
                m_Cannon.AimAndFire();
            }


            //Doesn't directly affect decision-making but is good for debugging
            float nearbyEnemies = m_CloseEnemiesCurve.Evaluate(Enemies.Count / 10.0f) * 100f;
            if (nearbyEnemies > 0)
                LogWithGlobalDelay($"There is {Enemies.Count} {(Enemies.Count == 1 ? "enemy" : "enemies")}" +
                             $" nearby, evade should be about {nearbyEnemies}% more powerful.");
        }

        /// <summary>
        /// Checks for available pickups and sets the closest one as the target.
        /// </summary>
        /// <param name="pickupTarget"></param>
        /// <param name="myPos"></param>
        private void CheckPickups(out Vector2 pickupTarget, in Vector2 myPos)
        {
            pickupTarget = Vector2.positiveInfinity;
            float healthDist = float.MaxValue;
            float ammoDist = float.MaxValue;

            // Checks if pickups are available by seeing if their location is still set to the default value of positive infinity
            bool hasHealth = MarkovStateBase.IsFinite(m_Task13.m_HealthPickupLocation);
            bool hasAmmo = MarkovStateBase.IsFinite(m_Task13.m_AmmoPickupLocation);

            //Exits early if there are no pickups to be had
            if (!hasHealth && !hasAmmo) return;

            if (hasHealth)
                healthDist = (m_Task13.m_HealthPickupLocation - myPos).sqrMagnitude;
            if (hasAmmo)
                ammoDist = (m_Task13.m_AmmoPickupLocation - myPos).sqrMagnitude;

            float ammoUrgency = 1.0f - m_CurrentAmmoCurve.Evaluate(m_Cannon.m_Ammo / 10.0f);
            //Health ratio is already between 0 and 1, so no need to normalise it like ammo
            float healthUrgency = 1.0f - m_CurrentHealthCurve.Evaluate(m_Health.HealthRatio);

            // If they are a similar level of urgency choose the closest pickup
            // Checks if the values are within 0.2 of each-other
            if (Mathf.Abs(healthUrgency - ammoUrgency) <= 0.125f)
            {
                // Prefers health if both are equally distant
                pickupTarget = healthDist <= ammoDist
                    ? m_Task13.m_HealthPickupLocation
                    : m_Task13.m_AmmoPickupLocation;

                LogWithGlobalDelay("Going for the closest pickup as both are a similar level of urgency", 1);
            }
            // Otherwise, choose the more urgently required pickup
            else if (healthUrgency > ammoUrgency)
                pickupTarget = m_Task13.m_HealthPickupLocation;
            else if (ammoUrgency > healthUrgency)
                pickupTarget = m_Task13.m_AmmoPickupLocation;

            //float pickupExists = pickupTarget != Vector2.positiveInfinity ? 1.0f : 0.0f;
            
            // Throttles the amount of debug messages being output to the console
            if(Has120FramesPassed())
                Debug.LogWarning($"Collecting The {(pickupTarget == m_Task13.m_HealthPickupLocation ? "Health" : "Ammo")} Pickup");
        }

        /// <summary>
        /// Ensures that pickup locations are reset when a pickup is collected.
        /// Positive infinity is used as a default value because Vector2.zero is a valid location
        /// </summary>
        private void ResetPickups()
        {
            m_Task13.m_HealthPickupLocation = Vector2.positiveInfinity;
            m_Task13.m_AmmoPickupLocation = Vector2.positiveInfinity;
        }

        /// <summary>
        /// For delaying debug messages to make the console more readable.
        /// </summary>
        /// <param name="message">The message to be displayed.</param>
        /// <param name="logType">The type of debug message to be displayed, 1 for warning, 2 for error</param>
        /// <param name="delay">The delay before the message is displayed. One second by default.</param>
        /// <returns>True when the log was successful, false if the delay hasn't hit yet.</returns>
        public void LogWithGlobalDelay(string message, int logType = 0, float delay = 1)
        {
            // Only allows a message to be logged once a second
            if (Time.time <= m_NextLogTime) return;
            m_NextLogTime = Time.time + delay;
            switch (logType)
            {
                case 1:
                    Debug.LogWarning(message);
                    break;
                case 2:
                    Debug.LogError(message);
                    break;
                default:
                    Debug.Log(message);
                    break;
            }
        }

        /// <summary>
        /// Returns true once every 120 frames (approximately every 2 seconds at 60fps).
        /// Good for throttling the amount of debug messages being output to the console 
        /// </summary>
        public bool Has120FramesPassed() => Time.frameCount % 120 == 0;
        
    }
}