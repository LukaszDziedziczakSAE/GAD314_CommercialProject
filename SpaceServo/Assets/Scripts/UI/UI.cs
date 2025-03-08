using UnityEngine;

// central UI script, accessing the UI should go through this script from all other scripts

// please use the "UI_" prefix for all UI related scripts

public class UI : MonoBehaviour
{
    public static UI Instance;
    [SerializeField] private UI_TopBar topBar;
    [SerializeField] private UI_AddPlaceablesMenu placeablesMenu;
    [SerializeField] private UI_AddRoomMenu roomsMenu;
    [SerializeField] private UI_RoomInfo roomInfo;
    [SerializeField] private UI_PauseMenu pauseMenu;
    [SerializeField] private UI_CustomerInfo customerInfo;
    [SerializeField] private UI_Tutorial tutorial;
    [SerializeField] private UI_DebugOutput debugOutput;
    [SerializeField] private UI_Settings settings;
    [SerializeField] private UI_Objectives objectives;

    public static bool MouseOverUI;


    public static bool IsPlaceablesMenuShowing => Instance.placeablesMenu.gameObject.activeSelf;
    public static bool IsRoomsMenuShowing => Instance.roomsMenu.gameObject.activeSelf;

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

    private void OnEnable()
    {
        Station.Money.OnAmountChange += UpdateMoneyText;
        Station.Rating.OnRatingChange += UpdateRatingVisual;
    }

    private void OnDisable()
    {
        Station.Money.OnAmountChange -= UpdateMoneyText;
        Station.Rating.OnRatingChange -= UpdateRatingVisual;
    }

    public static void UpdateMoneyText()
    {
        Instance.topBar.UpdateMoneyText();
    }

    public static void UpdateRatingText()
    {
        Instance.topBar.UpdateRatingText();
    }

    public static void UpdateRatingVisual()
    {
        Instance.topBar.UpdateRatingVisual();
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

    public static void ShowRoomInfo(bool show = true)
    {
        Instance.roomInfo.gameObject.SetActive(show);
    }

    public static void ShowCustomerInfo(bool show = true)
    {
        Instance.customerInfo.gameObject.SetActive(show);
    }

    public static void ShowTutorial(bool show = true)
    {
        Instance.tutorial.gameObject.SetActive(show);
    }

    public static void ShowDebugInfo(bool show = true)
    {
        Instance.debugOutput.gameObject.SetActive(show);
    }

    public static void ShowSettings(bool show = true)
    {
        Instance.settings.gameObject.SetActive(show);
    }

    public static void UpdateRoomInfo()
    {
        if (Instance.roomInfo.gameObject.activeSelf) Instance.roomInfo.UpdateUI();
    }

    public static void ShowPauseMenu(bool show = true)
    {
        Instance.pauseMenu.gameObject.SetActive(show);
    }

    public static void UpdateObjectives()
    {
        Instance.objectives.UpdateObjectiveUI();
    }

    public static void UpdateTutorial()
    {
        Instance.tutorial.UpdateTutorialUI();
    }
}
