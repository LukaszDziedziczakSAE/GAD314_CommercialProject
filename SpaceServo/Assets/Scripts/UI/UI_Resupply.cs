using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_Resupply : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] TMP_Text resupplyTitle;
    [SerializeField] TMP_Text resupplyAmount;
    [SerializeField] TMP_Text resupplyCost;
    [SerializeField] Button acceptButton;
    [SerializeField] Button cancelButton;
    [SerializeField] Slider amountSlider;

    int tradeAmount;
    StationSupplies.ESupplyType supplyType => Game.Selection.Room.Config.SupplyType;

    private void OnEnable()
    {
        acceptButton.onClick.AddListener(OnAcceptButtonPress);
        cancelButton.onClick.AddListener(OnCancelButtonPress);
        amountSlider.onValueChanged.AddListener(OnAmountSliderChange);

        tradeAmount = Mathf.Min(Station.Supplies.AmountCanAfford(supplyType), Station.Supplies.RoomForSupplyOf(supplyType));

        if (tradeAmount > 2)
        {
            amountSlider.maxValue = tradeAmount;
            amountSlider.value = amountSlider.maxValue;
        }
        resupplyTitle.text = "Buy " + supplyType.ToString();
        amountSlider.interactable = tradeAmount > 0;
        acceptButton.interactable = tradeAmount > 0;
        UpdateText();
    }

    private void OnDisable()
    {
        acceptButton.onClick.RemoveListener(OnAcceptButtonPress);
        cancelButton.onClick.RemoveListener(OnCancelButtonPress);
        amountSlider.onValueChanged.RemoveListener(OnAmountSliderChange);
    }

    private void OnAcceptButtonPress()
    {
        UI.Sound.PlayButtonPressSound();
        Station.Supplies.TryPurchaseSupplyForStation(supplyType, tradeAmount);
        UI.ShowResupply(false);
    }

    private void OnCancelButtonPress()
    {
        UI.Sound.PlayButtonCancelSound();
        UI.ShowResupply(false);
    }

    private void OnAmountSliderChange(float newValue)
    {
        tradeAmount = (int)newValue;
        UpdateText();
        acceptButton.interactable = tradeAmount > 0;
    }

    private void UpdateText()
    {
        resupplyAmount.text = tradeAmount.ToString();
        resupplyCost.text = "$" + (Station.Supplies.CostForSupplies(supplyType) * tradeAmount).ToString(); 
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
