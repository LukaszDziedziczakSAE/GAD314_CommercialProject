using TMPro;
using UnityEngine;

public class UI_FloorPlacementInfo : MonoBehaviour
{
    [SerializeField] TMP_Text roomNameText;
    [SerializeField] TMP_Text roomSizeText;
    [SerializeField] TMP_Text roomCostText;

    StationFloorBuilder floorBuilder => Game.FloorBuilder;

    private void OnEnable()
    {
        if (floorBuilder == null || !floorBuilder.IsPlacing)
        {
            UI.ShowFloorPlacementInfo(false);
            return;
        }

        UpdateRoomName();
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
}
