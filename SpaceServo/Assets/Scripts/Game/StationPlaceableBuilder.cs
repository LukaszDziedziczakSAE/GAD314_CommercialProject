using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class StationPlaceableBuilder : MonoBehaviour
{
    [field: SerializeField] public Placeable[] Placeables { get; private set; }
    [SerializeField] private bool snapToGrid;
    [field: SerializeField] public bool DontPlaceOnEdge { get; private set; }

    Placeable currentPlaceable;
    PlaceableObject currentPlaceableObject;

    public bool IsPlacing => currentPlaceable != null;

    private void Update()
    {
        if (UI.MouseOverUI || currentPlaceable == null || currentPlaceable.Prefab == null) return; // don't execute update if we are not placing

        if (currentPlaceableObject == null)
        {
            currentPlaceableObject = Instantiate(currentPlaceable.Prefab, GroundLocationUnderMouse, Quaternion.identity);

            if (currentPlaceable != currentPlaceableObject.Config)
            {
                currentPlaceableObject.CorrectConfig(currentPlaceable);
            }
        }
        else
        {
            currentPlaceableObject.transform.position = GroundLocationUnderMouse;
        }
    }

    public void BeginPlacement(Placeable newPlaceable)
    {
        if (Game.FloorBuilder.IsPlacing) return;

        currentPlaceable = newPlaceable;
        if (Game.Tutorial.ListentingForPlacementStart) Game.Tutorial.PlacementStarted(currentPlaceable);

        Game.Input.OnPrimaryPress += CompletePlacement;
        Game.Input.OnSecondaryPress += CancelPlacement;
        Game.Input.OnRotatePress += RotatePlacement;
    }

    private Vector3 GroundLocationUnderMouse
    {
        get
        {
            if (Game.CameraController == null)
            {
                Debug.LogError("Game is missing Camera Controller reference");
                return Vector3.zero;
            }

            Ray ray = Game.CameraController.Camera.ScreenPointToRay(Game.Input.MousePosition);
            float rayCastDistance = Game.CameraController.DistanceToGround + 50;

            if (Physics.Raycast(ray, out RaycastHit hit, rayCastDistance, Game.StationLayer))
            {
                if (snapToGrid)
                {
                    return Game.FloorBuilder.RoundToNearestHalfGrid(hit.point);
                }

                else return hit.point;
            }

            if (UI.MouseOverUI) Debug.LogWarning("Mouse over UI");
            //else Debug.LogWarning("RayCast fail, no ground position under mouse");
            return new Vector3(0, 500, 0);
        }
    }

    private void CompletePlacement()
    {
        if (!currentPlaceableObject.HasValidPlacement)
        {
            Debug.LogWarning("Invlid placement");
            return;
        }

        Game.Input.OnPrimaryPress -= CompletePlacement;
        Game.Input.OnSecondaryPress -= CancelPlacement;
        Game.Input.OnRotatePress -= RotatePlacement;

        currentPlaceableObject.SetPlaced();
        Station.Money.Remove(currentPlaceable.Cost);

        if (Game.Debug.PlaceablesBuilt.ContainsKey(currentPlaceable))
            Game.Debug.PlaceablesBuilt[currentPlaceable]++;
        else
            Game.Debug.PlaceablesBuilt.Add(currentPlaceable, 1);


        UI.Sound.PlayBuildPlaceableSound();
        currentPlaceableObject = null;

        if (Game.Input.ContinueProcedure) BeginPlacement(currentPlaceable);
        else currentPlaceable = null;
    }

    private void CancelPlacement()
    {
        Game.Input.OnPrimaryPress -= CompletePlacement;
        Game.Input.OnSecondaryPress -= CancelPlacement;
        Game.Input.OnRotatePress -= RotatePlacement;

        UI.Sound.PlayButtonCancelSound();

        currentPlaceable = null;
        Destroy(currentPlaceableObject.gameObject);
        currentPlaceableObject = null;
    }

    public Placeable[] BuildablePlaceables
    {
        get
        {
            List<Placeable> list = new List<Placeable>();

            foreach (RoomObject room in Station.Rooms)
            {
                foreach (Placeable placeable in room.Config.Placeables)
                {
                    if (!list.Contains(placeable)) list.Add(placeable);
                }
            }

            foreach (Placeable globalPlaceable in Placeables)
            {
                if (!list.Contains(globalPlaceable)) list.Add(globalPlaceable);
            }

            return list.ToArray();
        }
    }

    private void RotatePlacement()
    {
        Vector3 rotation = currentPlaceableObject.transform.eulerAngles;
        if (rotation.y == 270) rotation.y = 0;
        else rotation.y += 90;
        currentPlaceableObject.transform.eulerAngles = rotation;
    }
}