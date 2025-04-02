using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_RoomInfo_Placeable : MonoBehaviour
{
    [SerializeField] TMP_Text placeableName;
    [SerializeField] TMP_Text staffMemberName;
    [SerializeField] Button removeButton;
    [SerializeField] Button hireStaffButton;

    PlaceableObject placeable;

    public void Initilize(PlaceableObject placeableObject)
    {
        placeable = placeableObject;
        placeableName.text = placeable.Config.Name;

        if (placeable.TryGetComponent<TransactionDesk>(out TransactionDesk desk))
        {
            if (desk.StaffMember != null)
            {
                staffMemberName.text = desk.StaffMember.Name;
                hireStaffButton.gameObject.SetActive(false);
            }
            else
            {
                staffMemberName.gameObject.SetActive(false);
                hireStaffButton.onClick.AddListener(OnHireStaffButtonPress);
            }
        }
        else if (TryGetComponent<RectTransform>(out RectTransform rectTransform))
        {
            hireStaffButton.gameObject.SetActive(false);
            staffMemberName.gameObject.SetActive(false);
            rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x, rectTransform.sizeDelta.y / 2);
        }

        removeButton.onClick.AddListener(OnRemoveButtonPress);
    }

    private void OnRemoveButtonPress()
    {
        UI.Sound.PlayRemoveSound();
        Destroy(placeable.gameObject);
        UI.UpdateRoomInfo();
    }

    private void OnHireStaffButtonPress()
    {
        UI.Sound.PlayButtonPressSound();
        if (placeable.TryGetComponent<TransactionDesk>(out TransactionDesk desk))
        {
            desk.HireStaffMember();
        }
        UI.UpdateRoomInfo();
    }

    private void OnDisable()
    {
        hireStaffButton.onClick.RemoveAllListeners();
        removeButton.onClick.RemoveAllListeners();
    }
}
