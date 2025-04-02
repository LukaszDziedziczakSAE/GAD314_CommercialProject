using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_RoomInfo : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] TMP_Text roomNameText;
    [SerializeField] Button closeButton;
    [SerializeField] Button addFloorButton;
    [SerializeField] Button addPlaceableButton;
    [SerializeField] Button destroyRoomButton;
    [SerializeField] Transform content;
    [SerializeField] UI_RoomInfo_Placeable placeablePrefab;

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
        UpdateUI();

        addFloorButton.interactable = false;
        addPlaceableButton.onClick.AddListener(OnAddPlaceableButtonPress);
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
    }

    private void OnDisable()
    {
        closeButton.onClick.RemoveAllListeners();
        addPlaceableButton.onClick.RemoveAllListeners();
        destroyRoomButton.onClick.RemoveListener(OnDestoryButtonPress);
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
        Game.Selection.DeselectRoom();
        UI.MouseOverUI = false;
    }

    private void OnAddPlaceableButtonPress()
    {
        UI.ShowPlaceablesMenu();
    }

    private void OnDestoryButtonPress()
    {
        UI.Sound.PLayRemoveSound();
        Destroy(room.gameObject);
        Game.Selection.DeselectRoom();
        UI.MouseOverUI = false;
    }
}
