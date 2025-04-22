using System;
using UnityEngine;

public class Tutorial : MonoBehaviour
{
    [System.Serializable]
    public class TutorialPart
    {
        [field: SerializeField] public string TutorialText { get; private set; }
        [field: SerializeField] public EType Type { get; private set; }
        [field: SerializeField] public Objective Objective { get; private set; }
        [field: SerializeField] public EMenuType MenuType { get; private set; }
        [field: SerializeField] public ScriptableObject Config { get; private set; }
        [field: SerializeField] public bool ShowNextObjective { get; private set; }
        [field: SerializeField] public AudioClip VoiceOver { get; private set; }

        public enum EType
        {
            ClickThrough,
            Objective,
            OpenMenu,
            StartRoomBuild,
            StartPlacement
        }

        public enum EMenuType
        {
            None,
            RoomMenu,
            PlaceablesMenu
        }
    }

    [SerializeField] bool TurnOffTutorial;
    [field: SerializeField] public TutorialPart[] Parts { get; private set; }
    [SerializeField] AudioSource audioSource;

    public TutorialPart CurrentPart => Parts[index];
    public TutorialPart NextPart => Parts[index+1];

    [SerializeField] int index = -1;
    public bool IsRunning => index >= 0 && index < Parts.Length;
    public bool HasNextPart => index + 1 < Parts.Length;

    private void Start()
    {
        if (!TurnOffTutorial && Parts.Length > 0)
        {
            PartComplete();
            UI.ShowTutorial(true);
            Debug.Log("Starting tutorial");
        }
    }

    public void PartComplete()
    {
        //if (!IsRunning) return;

        if (index >= 0) TurnOffButtonFlashing();
        index++;
        //Debug.Log("Starting tutorial part " + index);
        if (index < Parts.Length)
        {
            TurnOnButtonFlashing();
            PlayVoiceOver();
        }
        
        UI.UpdateTutorial();
        UI.UpdateObjectives();
    }

    private void PlayVoiceOver()
    {
        if (CurrentPart.VoiceOver != null && audioSource != null)
        {
            if (audioSource.isPlaying && audioSource.clip != CurrentPart.VoiceOver) audioSource.Stop();
            audioSource.clip = CurrentPart.VoiceOver;
            audioSource.Play();
        }
    }

    public void ExitTutorial()
    {
        Debug.Log("Exiting tutorial");

        index = -1;
        UI.ShowTutorial(false);
        UI.UpdateObjectives();

        audioSource.Stop();
    }

    public Objective NextObjective
    {
        get
        {
            if (index >= 0 && index < Parts.Length)
            {
                for (int i = index; i < Parts.Length; i++)
                {
                    if (Parts[i].Type == TutorialPart.EType.Objective)
                    {
                        return Parts[i].Objective;
                    }
                }
            }
            return null;
        }
    }

    public bool ListeningForRoomMenu => IsRunning && CurrentPart.Type == TutorialPart.EType.OpenMenu && CurrentPart.MenuType == TutorialPart.EMenuType.RoomMenu;

    public bool ListentingForPlacablesMenu => IsRunning && CurrentPart.Type == TutorialPart.EType.OpenMenu && CurrentPart.MenuType == TutorialPart.EMenuType.PlaceablesMenu;

    public bool ListentingForFloorBuildStart => IsRunning && CurrentPart.Type == TutorialPart.EType.StartRoomBuild;

    public bool ListentingForPlacementStart => IsRunning && CurrentPart.Type == TutorialPart.EType.StartPlacement;

    public void RoomBuiltStarted(Room roomConfig)
    {
        if ((Room)CurrentPart.Config == roomConfig) PartComplete();
    }

    public void PlacementStarted(Placeable placeable)
    {
        if ((Placeable)CurrentPart.Config == placeable) PartComplete();
    }

    private void TurnOnButtonFlashing()
    {
        if (CurrentPart.Type == TutorialPart.EType.OpenMenu)
        {
            if (CurrentPart.MenuType == TutorialPart.EMenuType.RoomMenu)
                UI.TopBar.BuildFloorButton.StartFlashing();

            if (CurrentPart.MenuType == TutorialPart.EMenuType.PlaceablesMenu)
                UI.TopBar.BuildPlaceableButton.StartFlashing();
        }
    }

    private void TurnOffButtonFlashing()
    {
        if (CurrentPart.Type == TutorialPart.EType.OpenMenu)
        {
            if (CurrentPart.MenuType == TutorialPart.EMenuType.RoomMenu)
                UI.TopBar.BuildFloorButton.StopFlashing();

            if (CurrentPart.MenuType == TutorialPart.EMenuType.PlaceablesMenu)
                UI.TopBar.BuildPlaceableButton.StopFlashing();
        }
    }
}
