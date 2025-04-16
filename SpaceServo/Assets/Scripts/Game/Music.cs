using Unity.VisualScripting;
using UnityEngine;

public class Music : MonoBehaviour
{
    [SerializeField] AudioSource musicSource;
    [SerializeField] AudioClip[] musicClips;
    int index = -1;

    private void Update()
    {
        if (musicSource == null || musicClips.Length == 0) return;

        if (!musicSource.isPlaying)
        {
            if (index >= musicClips.Length) index = 0;
            else index++;
            musicSource.clip = musicClips[index];
            musicSource.Play();
        }
    }
}
