using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_RoomInfo : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] TMP_Text roomNameText;
    [SerializeField] Button closeButton;
    [SerializeField] Button addPlaceableButton;
    [SerializeField] Button destroyRoomButton;
    [SerializeField] Button editRoomButton;
    [SerializeField] Transform content;
    [SerializeField] UI_RoomInfo_Placeable placeablePrefab;
    [SerializeField] Button purchaseSuppliesButton;
    [SerializeField] TMP_Text suppliesText;
    [SerializeField] RectTransform suppliesProgress;
    [SerializeField] GameObject suppliesBox;

    RoomObject room => Game.Selection.Room;
    List<GameObject> listItems = new List<GameObject>();

    private void OnEnable()
    {
        if (Game.Selection.Room == null)
        {
            UI.ShowRoomInfo(false);
            return;
        }

        closeButton.onClick.AddListener(OnCloseButtonPress);
        destroyRoomButton.onClick.AddListener(OnDestoryButtonPress);
        addPlaceableButton.onClick.AddListener(OnAddPlaceableButtonPress);
        editRoomButton.onClick.AddListener(OnEditRoomButonPress);
        purchaseSuppliesButton.onClick.AddListener(OnPurchaseSuppliesButtonPress);
        UpdateUI();
    }

    public void UpdateUI()
    {
        roomNameText.text = room.Config.Name;

        if (listItems.Count > 0)
        {
            foreach (GameObject item in listItems)
            {
                Destroy(item);
            }
            listItems.Clear();
        }

        foreach (PlaceableObject placeable in room.Placeables)
        {
            UI_RoomInfo_Placeable placeableInfoItem = Instantiate(placeablePrefab, content);
            listItems.Add(placeableInfoItem.gameObject);
            placeableInfoItem.Initilize(placeable);
        }

        suppliesBox.SetActive(room.Config.SupplyType != StationSupplies.ESupplyType.NoSupplies);
        purchaseSuppliesButton.gameObject.SetActive(room.Config.SupplyType != StationSupplies.ESupplyType.NoSupplies);
        if (room.Config.SupplyType != StationSupplies.ESupplyType.NoSupplies)
        {
            suppliesText.text = room.Config.SupplyType.ToString() + " " + room.AvailableSupplies + " / " + room.MaxAvailableSupplies;
            suppliesProgress.localScale = new Vector3(((float)room.AvailableSupplies / (float)room.MaxAvailableSupplies), 1, 1);
        }
    }

    private void OnDisable()
    {
        closeButton.onClick.RemoveAllListeners();
        addPlaceableButton.onClick.RemoveAllListeners();
        destroyRoomButton.onClick.RemoveListener(OnDestoryButtonPress);
        editRoomButton.onClick.RemoveListener(OnEditRoomButonPress);
        purchaseSuppliesButton.onClick.AddListener(OnPurchaseSuppliesButtonPress);
    }


    public void OnPointerEnter(PointerEventData eventData)
    {
        UI.MouseOverUI = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        UI.MouseOverUI = false;
    }

    private void OnCloseButtonPress()
    {
        UI.Sound.PlayButtonCancelSound();
        Game.Selection.DeselectRoom();
        UI.MouseOverUI = false;
    }

    private void OnAddPlaceableButtonPress()
    {
        UI.Sound.PlayButtonPressSound();
        UI.ShowPlaceablesMenu();
    }

    private void OnDestoryButtonPress()
    {
        UI.Sound.PlayRemoveSound();
        Destroy(room.gameObject);
        Game.Selection.DeselectRoom();
        UI.MouseOverUI = false;
    }

    private void OnEditRoomButonPress()
    {
        UI.Sound.PlayButtonPressSound();
        UI.MouseOverUI = false;
        Game.FloorBuilder.BeginEditMode(room);
        Game.Selection.DeselectRoom();
    }

    private void OnPurchaseSuppliesButtonPress()
    {
        UI.Sound.PlayButtonPressSound();
        UI.ShowResupply();
    }
}
