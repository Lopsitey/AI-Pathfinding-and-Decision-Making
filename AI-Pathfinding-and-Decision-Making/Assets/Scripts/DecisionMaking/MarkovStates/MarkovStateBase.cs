#region

using UnityEngine;

#endregion

namespace DecisionMaking.MarkovStates
{
    public abstract class MarkovStateBase : MonoBehaviour
    {
        protected MarkovManager Owner { get; private set; }
        protected bool IsActive { get; private set; }
        protected float DegreeOfActivation { get; set; }

        protected Vector2 m_PickupTarget;
        protected MovingEntity m_MovingTarget;
        protected float m_StartingWeight;

        protected virtual void Start()
        {
            if (TryGetComponent(out MarkovManager markov))
                Owner = markov;
            else
                Debug.LogWarning("MarkovStateBase: MarkovManager not found");
        }

        /// <summary>
        /// Ends all steering behaviours when entering a state.
        /// </summary>
        public virtual void Enter() => IsActive = true;
        
        public abstract void UpdateAgent();

        public virtual void Exit() => IsActive = false;

        public abstract float CalculateActivation(MovingEntity movingTarget = null, Vector2 target = default);

        /// <summary>
        /// Creates an exponential curve based on distance between agent and target.
        /// </summary>
        /// <param name="agent">The origin point of the curve.</param>
        /// <param name="target">The target the agent is travelling towards.</param>
        /// <param name="isGrowth">The type of curve - true for growth - false for decay.</param>
        protected float ExponentialCurve(in Vector2 agent, in Vector2 target, in bool isGrowth = true)
        {
            float dist = Maths.Magnitude(target - agent);

            // A steeper curve means faster drop off in activation with distance
            const float curveSteepness = 0.1f;
            const float initialHeight = 100f;
            float curveType = isGrowth ? 1f : -1f;

            // Exponential decay curve for activation based on distance
            return initialHeight * Mathf.Exp((curveSteepness * curveType) * dist);
            // Curve steepness is negative for decay - positive would be growth.
        }

        /// <summary>
        /// Returns true if the vector isn't infinite, NaN.
        /// </summary>
        public static bool IsFinite(Vector2 v) =>
            float.IsFinite(v.x) && float.IsFinite(v.y);
    }
}