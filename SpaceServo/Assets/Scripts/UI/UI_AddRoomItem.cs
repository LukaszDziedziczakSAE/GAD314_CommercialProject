using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_AddRoomItem : MonoBehaviour
{
    [SerializeField] Button button;
    [SerializeField] TMP_Text nameText;
    [SerializeField] TMP_Text costText;
    [SerializeField] RawImage icon;
    [SerializeField] UI_FlashingButton flashingButton;

    Room config;

    public void Initilize(Room newConfig)
    {
        config = newConfig;
        button.onClick.AddListener(OnButtonPress);

        icon.texture = config.Icon;
        nameText.text = config.Name;
        costText.text = "$" + config.BasePrice + " + " + config.CostPerTile + " Per/Tile";

        if (Game.Tutorial.IsRunning)
        {
            if (Game.Tutorial.CurrentPart.Type == Tutorial.TutorialPart.EType.StartRoomBuild &&
                config == (Room)Game.Tutorial.CurrentPart.Config &&
                flashingButton != null)
                    flashingButton.StartFlashing();

            if (Game.Tutorial.HasNextPart && Game.Tutorial.NextPart.Type == Tutorial.TutorialPart.EType.StartRoomBuild &&
                config == (Room)Game.Tutorial.NextPart.Config && flashingButton != null)
                    flashingButton.StartFlashing();
        }
    }

    private void OnButtonPress()
    {
        UI.Sound.PlayButtonPressSound();

        Game.FloorBuilder.StartPlacingFloor(config);

        button.onClick.RemoveListener(OnButtonPress);
        UI.ShowRoomsMenu(false);
        UI.MouseOverUI = false;
    }
}
