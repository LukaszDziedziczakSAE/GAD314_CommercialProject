using UnityEngine;

// central UI script, accessing the UI should go through this script from all other scripts

// please use the "UI_" prefix for all UI related scripts

public class UI : MonoBehaviour
{
    public static UI Instance;
    [SerializeField] private UI_TopBar topBar;
    [SerializeField] private UI_AddPlaceablesMenu placeablesMenu;
    [SerializeField] private UI_AddRoomMenu roomsMenu;

    public static bool MouseOverUI;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(this);
    }

    private void Start()
    {
        if (topBar == null) Debug.LogError("UI is missing referance to top bar");

        ShowPlaceablesMenu(false);
    }

    public static void UpdateMoneyText()
    {
        Instance.topBar.UpdateMoneyText();
    }

    public static void UpdateRatingText()
    {
        Instance.topBar.UpdateRaitingText();
    }

    public static void ShowPlaceablesMenu(bool show = true)
    {
        if (show) CloseMenus();
        Instance.placeablesMenu.gameObject.SetActive(show);
    }

    public static void ShowRoomsMenu(bool show = true)
    {
        if (show) CloseMenus();
        Instance.roomsMenu.gameObject.SetActive(show);
    }

    public static void CloseMenus()
    {
        Instance.placeablesMenu.gameObject.SetActive(false);
        Instance.roomsMenu.gameObject.SetActive(false);
    }
}
