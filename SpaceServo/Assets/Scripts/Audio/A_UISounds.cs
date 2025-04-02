using UnityEngine;

public class A_UISounds : A_BASE
{
    [SerializeField] AudioClip buttonPress;
    [SerializeField] AudioClip buttonCancel;
    [SerializeField] AudioClip placeFloor;
    [SerializeField] AudioClip buildPlaceable;
    [SerializeField] AudioClip remove;

    public void PlayButtonPressSound()
    {
        PlayClip(buttonPress);
    }

    public void PlayButtonCancelSound()
    {
        PlayClip(buttonCancel);
    }

    public void PlayPlaceFloorSound()
    {
        PlayClip(placeFloor);
    }

    public void PlayBuildPlaceableSound()
    {
        PlayClip(buildPlaceable);
    }

    public void PlayRemoveSound()
    {
        PlayClip(remove);
    }
}
