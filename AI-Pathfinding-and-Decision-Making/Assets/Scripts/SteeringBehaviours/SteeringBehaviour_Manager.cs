using System.Collections.Generic;
using UnityEngine;

namespace SteeringBehaviours
{
    public class SteeringBehaviourManager : MonoBehaviour
    {
        public MovingEntity m_Entity { get; private set; }
        public float m_MaxForce = 500;
        public float m_RemainingForce;
        public List<SteeringBehaviour> m_SteeringBehaviours;

        private void Awake()
        {
            m_Entity = GetComponent<MovingEntity>();

            if(!m_Entity)
                Debug.LogError("Steering Behaviours only working on type moving entity", this);
        }

        public Vector2 GenerateSteeringForce()
        {
            Vector2 totalForce = Vector2.zero;
            foreach (SteeringBehaviour steeringBehaviour in m_SteeringBehaviours)
            {
                //If the steering behaviour is disabled stop
                if (!steeringBehaviour.m_Active) continue;
            
                //returns the correct force for the specific steering behaviour
                Vector2 force = steeringBehaviour.CalculateForce();         
                //the amount of force left available to be applied to the object
                float remainingForce = m_MaxForce - totalForce.magnitude; 
                //stops if there is no more room for any extra force on the object
                if (remainingForce <= 0) return totalForce;
            
                //if there is enough room left to fit the entire force
                if (force.magnitude < remainingForce)
                {
                    totalForce += force;
                }
                else//if not try to use just the direction (normalised force)
                {
                    //Normalises the direction and multiplies it by the remaining force
                    //This is only done if it's too big too fit into the max force normally but there is still some room left
                    totalForce += force.normalized * remainingForce;
                    return totalForce;
                }
            }
            return totalForce;
        }
    
        public void EnableExclusive(SteeringBehaviour behaviour)
        {
            if(m_SteeringBehaviours.Contains(behaviour))
            {
                foreach(SteeringBehaviour sb in m_SteeringBehaviours)
                {
                    sb.m_Active = false;
                }

                behaviour.m_Active = true;
            }
            else
            {
                Debug.Log(behaviour + " does not exist on object", this);
            }
        }
        public void DisableAllSteeringBehaviours()
        {
            foreach (SteeringBehaviour sb in m_SteeringBehaviours)
            {
                sb.m_Active = false;
            }
        }

        public void AddSteeringBehaviour(SteeringBehaviour behaviour) 
        {
            m_SteeringBehaviours.Add(behaviour);
        }

        public void RemoveSteeringBehaviour(SteeringBehaviour behaviour)
        {
            m_SteeringBehaviours.Remove(behaviour);
        }
    }
}
