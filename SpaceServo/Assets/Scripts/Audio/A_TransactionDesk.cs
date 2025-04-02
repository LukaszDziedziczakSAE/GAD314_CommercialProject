using UnityEngine;

public class A_TransactionDesk : A_BASE
{
    [SerializeField] AudioClip purchaseSound;

    public void PlayPurchaseSound()
    {
        PlayClip(purchaseSound);
    }
}
