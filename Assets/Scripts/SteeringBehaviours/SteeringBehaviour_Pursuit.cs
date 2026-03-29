using UnityEngine;

namespace SteeringBehaviours
{
    public class SteeringBehaviourPursuit : SteeringBehaviour
    {

        [Header("Pursuit Properties")]
        [Header("Settings")]
        public MovingEntity m_PursuedEntity;

        public override Vector2 CalculateForce()
        {
            if (!m_PursuedEntity) return Vector2.zero;
        
            float dist = Maths.Magnitude(transform.position - m_PursuedEntity.transform.position);
        
            float currentSpeed = Maths.Magnitude(m_Manager.m_Entity.m_Velocity);
            float pursuerSpeed = Maths.Magnitude(m_PursuedEntity.m_Velocity);
        
            //calculated magnitude first since we want to add the speeds, not add the vectors and then convert it into a speed
            float combinedSpeed = currentSpeed + pursuerSpeed;
        
            //time to predict is the distance divided by the total speed
            //if something moves 3 blocks a frame, and they are 3 blocks away you have 1 frame to predict
            float predictTime = combinedSpeed > 0.001f ? dist / combinedSpeed : 0;//if the player and target are moving - predict
            //stops the AI from predicting like 30 seconds ahead if the player is miles away
            predictTime = Mathf.Clamp(predictTime, 0f, 5.0f);
        
            Vector2 targetPos = m_PursuedEntity.transform.position;
            Vector2 predictedTargetPosition = targetPos + (m_PursuedEntity.m_Velocity * predictTime);
        
            //seek code
            Vector2 pos = transform.position;
            //calculates the vector pointing to the target position
            m_DesiredVelocity = predictedTargetPosition - pos;
            //speed needs normal vectors
            m_DesiredVelocity = m_Manager.m_Entity.m_MaxSpeed * Maths.Normalise(m_DesiredVelocity);
            //desired velocity - current velocity = steering
            m_Steering = m_DesiredVelocity - m_Manager.m_Entity.m_Velocity;
            return m_Steering * m_Weight;//force = steering * weight - pursues the target
        }
    }
}
