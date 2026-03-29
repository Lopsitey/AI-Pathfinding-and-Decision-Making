using UnityEngine;

public class TeleportingObject : MonoBehaviour
{
    float m_TeleportTimer = 0f;
    public float m_TeleportTime = 5f;

    // Update is called once per frame

    private void OnTriggerEnter2D(Collider2D collision)
    {
            transform.position = TileGrid.GetRandomWalkableTile(2).transform.position;
        m_TeleportTimer -= Time.deltaTime;

        if (m_TeleportTimer <= 0f)
        {
            m_TeleportTimer = m_TeleportTime;
        }
    }
}
