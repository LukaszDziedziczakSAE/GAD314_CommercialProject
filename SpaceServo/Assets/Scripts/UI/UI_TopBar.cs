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
    [SerializeField] Button menuButton;

    public UI_FlashingButton BuildFloorButton => buildFloorButton.GetComponent<UI_FlashingButton>();
    public UI_FlashingButton BuildPlaceableButton => placeObjectButton.GetComponent<UI_FlashingButton>();

    private void OnEnable()
    {
        buildFloorButton.onClick.AddListener(OnBuildFloorButtonPress);
        placeObjectButton.onClick.AddListener(OnPlaceObjectButtonPress);
        menuButton.onClick.AddListener(OnMenuButtonPress);
    }

    private void OnDisable()
    {
        buildFloorButton.onClick.RemoveListener(OnBuildFloorButtonPress);
        placeObjectButton.onClick.RemoveListener(OnPlaceObjectButtonPress);
        menuButton.onClick.RemoveListener(OnMenuButtonPress);
    }

    private void OnBuildFloorButtonPress()
    {
        UI.Sound.PlayButtonPressSound();
        Game.Selection.DeselectCustomer();
        Game.Selection.DeselectRoom();
        UI.ShowRoomsMenu(!UI.IsRoomsMenuShowing);
    }

    private void OnPlaceObjectButtonPress()
    {
        UI.Sound.PlayButtonPressSound();
        Game.Selection.DeselectCustomer();
        //Game.Selection.DeselectRoom();
        UI.ShowPlaceablesMenu(!UI.IsPlaceablesMenuShowing);
    }

    private void OnMenuButtonPress()
    {
        Game.PauseGame(!Game.IsPaused);
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
