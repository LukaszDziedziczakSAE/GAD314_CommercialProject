using UnityEngine;

public class A_Ship : A_BASE
{
    [SerializeField] AudioClip runningSound;
    [SerializeField] AudioClip takeoff;
    [SerializeField] AudioClip landing;

    public void PlayRunningSound()
    {
        PlayClip(runningSound);
    }

    public void PlayTakeOffSound()
    {
        PlayClip(takeoff);
    }

    public void PlayLandingSound()
    {
        PlayClip(landing);
    }
}
