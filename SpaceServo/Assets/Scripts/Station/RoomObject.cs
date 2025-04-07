using System.Collections.Generic;
using UnityEngine;
using static StationFloorBuilder;

public class RoomObject : MonoBehaviour
{
    [field: SerializeField] public Room Config {  get; private set; }

    public List<FloorTile> Floor { get; private set; } = new List<FloorTile>();
    public List<FloorTile> DoorTiles { get; private set; } = new List<FloorTile>();
    public List<PlaceableObject> Placeables { get; private set; } = new List<PlaceableObject>();
    public int Cost => Config.Cost(Floor.Count);
    public List<Customer> IncomingCustomers = new List<Customer>();
    public Vector2 RoomSize { get; private set; }

    public void Initialize(Room config)
    {
        Config = config;
        name = config.Name;
    }

    public void AddFloorTile(FloorTile tile)
    {
        Floor.Add(tile);
        tile.transform.parent = transform;
        tile.SetRoom(this);
    }

    public void AddWalls()
    {
        foreach(FloorTile tile in Floor)
        {
            tile.AddWalls();
        }
    }

    public void RemoveWalls()
    {
        foreach (FloorTile tile in Floor)
        {
            tile.RemoveWalls();
        }
    }

    public void AddDoorTile(FloorTile doorTile)
    {
        DoorTiles.Add(doorTile);
        doorTile.AddWalls();
    }

    public void RemoveTile(FloorTile tile)
    {
        Floor.Remove(tile);
    }

    public void AddPlaceable(PlaceableObject placeableObject)
    {
        Placeables.Add(placeableObject);
        placeableObject.transform.parent = transform;
    }

    public void RemovePlaceable(PlaceableObject placeableObject)
    {
        Placeables.Remove(placeableObject);
    }

    public int NumberOfFreeCustomerSlots
    {
        get
        {
            int freeSlots = 0;
            bool hasInteractables = false;

            foreach (PlaceableObject placeableObject in Placeables)
            {
                if (placeableObject.TryGetComponent<InteractablePlaceableObject>(out InteractablePlaceableObject interactable))
                {
                    freeSlots += interactable.AvailableSlots;
                    hasInteractables = true;
                }
            }

            if (hasInteractables)
            {
                freeSlots -= IncomingCustomers.Count;
                return freeSlots;
            }

            else
            {
                foreach (PlaceableObject placeableObject in Placeables)
                {
                    if (placeableObject.TryGetComponent<TransactionDesk>(out TransactionDesk transactionDesk)
                        && transactionDesk.StaffMember != null)
                    {
                        freeSlots += transactionDesk.FreeCustomerSlots;
                    }
                }

                freeSlots -= IncomingCustomers.Count;

                return freeSlots;
            }
 
        }
    }

    public bool TryGetAvailableDesk(out TransactionDesk transactionDesk)
    {
        List<TransactionDesk> desks = new List<TransactionDesk>();

        foreach (PlaceableObject placeableObject in Placeables)
        {
            if (placeableObject.TryGetComponent<TransactionDesk>(out TransactionDesk desk) 
                && desk.StaffMember != null && desk.FreeCustomerSlots > 0)
            {
                desks.Add(desk);
            }
        }

        if (desks.Count > 0)
        {
            transactionDesk = desks[Random.Range(0, desks.Count)];
            return true;
        }

        transactionDesk = null;
        return false;
    }

    public bool TryGetAvailableElement(Placeable config, out InteractionElement intElement)
    {
        List<InteractionElement> available = new List<InteractionElement>();

        foreach (PlaceableObject placeableObject in Placeables)
        {
            if (placeableObject.Config == config 
                && placeableObject.TryGetComponent<InteractablePlaceableObject>(out InteractablePlaceableObject intObject)
                && intObject.TryGetFreeInteractionElement(out InteractionElement element))
            {
                available.Add(element);
            }
        }

        if (available.Count > 0)
        {
            intElement = available[Random.Range(0, available.Count)];
            return true;
        }


        intElement = null;
        return false;
    }

    public bool TryGetAvailableInteractable(Placeable config, out InteractablePlaceableObject interactable)
    {
        List<InteractablePlaceableObject> available = new List<InteractablePlaceableObject>();

        foreach (PlaceableObject placeableObject in Placeables)
        {
            if (placeableObject.Config == config
                && placeableObject.TryGetComponent<InteractablePlaceableObject>(out InteractablePlaceableObject intObject))
            {
                available.Add(intObject);
            }
        }

        if (available.Count > 0)
        {
            interactable = available[Random.Range(0, available.Count)];
            return true;
        }


        interactable = null;
        return false;
    }

    public bool HasFreeSlots => NumberOfFreeCustomerSlots > 0;

    private void OnDestroy()
    {
        foreach (PlaceableObject placeableObject in Placeables)
        {
            Destroy(placeableObject);
        }

        Station.RemoveRoom(this);
    }

    public void ShowSelectedMaterial(bool selected)
    {
        if (selected)
        {
            foreach (FloorTile tile in Floor)
            {
                tile.SwitchToSelectedMaterial();
            }
        }
        else
        {
            foreach (FloorTile tile in Floor)
            {
                tile.SwitchToBuitMaterial();
            }
        }
            
    }

    public void ShowEditingMaterial()
    {
        foreach (FloorTile tile in Floor)
        {
            tile.SwitchToBuildingValidMaterial();
        }
    }

    public void SetRoomSize(Vector2 Size)
    {
        RoomSize = Size;
    }

    public Vector2 LocalTilePosition(FloorTile tile)
    {
        return new Vector2(tile.x - Floor[0].x, tile.z - Floor[0].z);
    }

    public EWallSide TileWallSide(FloorTile tile)
    {
        Vector2 localPosition = LocalTilePosition(tile);

        if (localPosition.x == 0) return EWallSide.LeftWall;
        else if (localPosition.x == RoomSize.x) return EWallSide.RightWall;
        else if (localPosition.y == RoomSize.y) return EWallSide.BottomWall;
        else if (localPosition.y == 0) return EWallSide.TopWall;
        
        return EWallSide.NoWall;
    }

    public void ClearFloor()
    {
        foreach (FloorTile tile in Floor)
        {
            Destroy(tile.gameObject);
        }
        Floor.Clear();
    }
}
