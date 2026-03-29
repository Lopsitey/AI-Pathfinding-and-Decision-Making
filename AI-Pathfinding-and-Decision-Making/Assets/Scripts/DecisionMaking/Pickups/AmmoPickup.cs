using UnityEngine;

[CreateAssetMenu(fileName = "PickupData", menuName = "ScriptableObjects/AmmoPickup", order = 1)]
public class AmmoPickUp : BasePickupEffect
{
    public override void ApplyEffect(GameObject target)
    {
        Cannon[] weapons = target.GetComponentsInChildren<Cannon>();

        foreach (Cannon weapon in weapons)
        {
            weapon.Reload();
        }
    }
}
