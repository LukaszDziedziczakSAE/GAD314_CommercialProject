using UnityEngine;

public class A_FuelPurchase : A_BASE
{
    [SerializeField] AudioClip purchaseSound;

    public void PlayPurchaseSound()
    {
        PlayClip(purchaseSound);
    }
}
