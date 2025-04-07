using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_FloorPlacementInfo : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] TMP_Text roomNameText;
    [SerializeField] TMP_Text roomSizeText;
    [SerializeField] TMP_Text roomCostText;
    [SerializeField] Button confirmButton;
    [SerializeField] Button cancelButton;

    StationFloorBuilder floorBuilder => Game.FloorBuilder;

    private void OnEnable()
    {
        confirmButton.onClick.AddListener(OnConfirmButtonPress);
        cancelButton.onClick.AddListener(OnCancelButtonPress);

        if (floorBuilder == null || !floorBuilder.IsPlacing)
        {
            UI.ShowFloorPlacementInfo(false);
            return;
        }

        UpdateRoomName();
    }

    private void OnDisable()
    {
        confirmButton.onClick.RemoveListener(OnConfirmButtonPress);
        cancelButton.onClick.RemoveListener(OnCancelButtonPress);
    }

    private void Update()
    {
        UpdateSizeText();
        UpdateCostText();

        if (!floorBuilder.IsPlacing)
        {
            UI.ShowFloorPlacementInfo(false);
            return;
        }

        confirmButton.interactable = floorBuilder.PlacementIsValid && floorBuilder.CanAffordPlacement;
    }

    public void UpdateRoomName()
    {
        roomNameText.text = floorBuilder.CurrentRoom.Config.Name;
    }

    public void UpdateSizeText()
    {
        roomSizeText.text = floorBuilder.CurrentRoomSize.x + " X " + floorBuilder.CurrentRoomSize.y;
    }

    public void UpdateCostText()
    {
        roomCostText.text = "$" + floorBuilder.CostOfPlacement;
    }

    private void OnConfirmButtonPress()
    {
        floorBuilder.CompletePlacement();
        UI.MouseOverUI = false;
    }

    private void OnCancelButtonPress()
    {
        floorBuilder.CancelPlacement();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        UI.MouseOverUI = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        UI.MouseOverUI = false;
    }
}
