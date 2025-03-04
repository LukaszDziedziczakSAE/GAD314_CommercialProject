using NUnit.Framework.Interfaces;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_TopBar : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] TMP_Text moneyText;
    [SerializeField] TMP_Text ratingText;
    [SerializeField] Slider ratingSlider;
    [SerializeField] Button buildFloorButton;
    [SerializeField] Button placeObjectButton;

    private void OnEnable()
    {
        buildFloorButton.onClick.AddListener(OnBuildFloorButtonPress);
        placeObjectButton.onClick.AddListener(OnPlaceObjectButtonPress);
    }

    private void OnDisable()
    {
        buildFloorButton.onClick.RemoveListener(OnBuildFloorButtonPress);
        placeObjectButton.onClick.RemoveListener(OnPlaceObjectButtonPress);
    }

    private void OnBuildFloorButtonPress()
    {
        Game.Selection.DeselectRoom();
        UI.ShowRoomsMenu(!UI.IsRoomsMenuShowing);
    }

    private void OnPlaceObjectButtonPress()
    {
        Game.Selection.DeselectRoom();
        UI.ShowPlaceablesMenu(!UI.IsPlaceablesMenuShowing);
    }

    public void UpdateMoneyText()
    {
        moneyText.text = "$" + Station.Money.Amount.ToString();
    }

    public void UpdateRatingText()
    {
        ratingText.text = Station.Rating.Value.ToString("F0");
    }

    public void UpdateRatingVisual()
    {
        ratingSlider.value = Station.Rating.Value / Station.Rating.MAX_RATING;
        // can add some sort of smoothing when the value changes
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
