using UnityEngine;

public class A_BASE : MonoBehaviour
{
    [SerializeField] AudioSource audioSource;

    protected void PlayClip(AudioClip audioClip)
    {
        if (audioSource == null || audioClip == null) return;

        if (audioSource.isPlaying) audioSource.Stop();
        audioSource.clip = audioClip;
        audioSource.Play();
    }

    public void Stop()
    {
        if (audioSource == null) return;
        audioSource.Stop();
    }
}
