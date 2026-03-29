using System;
using UnityEngine;
using UnityEngine.Assertions;

public class ApplyDamage : MonoBehaviour
{
    public Action OnDamageDealt;

    [SerializeField]
    int m_Damage;

    [SerializeField]
    bool m_DestroyOnApply = false;

    [SerializeField]
    LayerMask m_CanDamageLayer = 0;
    
    [SerializeField]
    string m_CanDamageTag;

#if DEBUG
    private void Start()
    {
        Assert.AreNotEqual(m_CanDamageLayer, 0);
    }
#endif

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if ((m_CanDamageLayer.value & 1 << collision.gameObject.layer) != 0)
        {
            if(m_CanDamageTag != string.Empty)
                if (!collision.gameObject.CompareTag(m_CanDamageTag))
                    return;
            
            Health mv = collision.gameObject.GetComponent<Health>();
            if (mv != null)
            {
                mv.ApplyDamage(m_Damage);
                OnDamageDealt?.Invoke();
            }
        }
        if (m_DestroyOnApply)
        {
            Destroy(gameObject);
        }
    }
}
