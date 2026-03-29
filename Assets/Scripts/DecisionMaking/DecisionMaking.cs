using System.Collections.Generic;
using UnityEngine;

namespace DecisionMaking
{
    public abstract class DecisionMaking : MonoBehaviour
    {
        protected List<Task7_WanderingAgent> Enemies { get; private set; }
        
        protected virtual void Awake() => Enemies = new List<Task7_WanderingAgent>();

        public abstract void CustomUpdate();

        /// <summary>
        /// Uses the decision-making entity to update a list of enemies in range.
        /// </summary>
        public void UpdateEnemyList(List<Task7_WanderingAgent> enemies)
        {
            Enemies.Clear();
            Enemies.AddRange(enemies);
        }
    }
}