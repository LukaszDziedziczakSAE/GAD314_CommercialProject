using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_AddPlaceablesItem : MonoBehaviour
{
    [SerializeField] Button button;
    [SerializeField] TMP_Text nameText;
    [SerializeField] TMP_Text costText;
    [SerializeField] RawImage icon;
    [SerializeField] UI_FlashingButton flashingButton;

    Placeable config;

    public void Initilize(Placeable newConfig)
    {
        config = newConfig;
        button.onClick.AddListener(OnButtonPress);

        icon.texture = config.Icon;
        nameText.text = config.Name;
        costText.text = "$" + config.Cost;

        button.interactable = Station.Money.CanAfford(config.Cost);

        if (Game.Tutorial.IsRunning)
        {
            if (Game.Tutorial.CurrentPart.Type == Tutorial.TutorialPart.EType.StartPlacement &&
                config == (Placeable)Game.Tutorial.CurrentPart.Config &&
                flashingButton != null)
                flashingButton.StartFlashing();

            if (Game.Tutorial.HasNextPart && Game.Tutorial.NextPart.Type == Tutorial.TutorialPart.EType.StartPlacement &&
                config == (Placeable)Game.Tutorial.NextPart.Config && flashingButton != null)
                flashingButton.StartFlashing();
        }
    }

    private void OnButtonPress()
    {
        UI.Sound.PlayButtonPressSound();

        Game.PlaceableBuilder.BeginPlacement(config);

        button.onClick.RemoveListener(OnButtonPress);
        UI.ShowPlaceablesMenu(false);
        UI.MouseOverUI = false;
    }
}
