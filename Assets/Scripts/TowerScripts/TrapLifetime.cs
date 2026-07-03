using UnityEngine;

public class TrapLifetime : MonoBehaviour
{
    private TrapperTower owner;

    public void Initialize(TrapperTower newOwner)
    {
        owner = newOwner;
    }

    void OnDestroy()
    {
        if (owner != null)
        {
            owner.NotifyTrapRemoved();
        }
    }
}