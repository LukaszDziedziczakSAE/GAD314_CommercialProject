using System;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;

public class Station : MonoBehaviour
{
    public static Station Instance;

    [SerializeField] private StationMoney stationMoney;
    [SerializeField] private StationRating stationRating;
    [SerializeField] private ShipManager shipManager;
    [SerializeField] private StationStaffManager staffManager;
    [SerializeField] private CustomerManager customerManager;
    [SerializeField] private StationSupplies stationSupplies;
    [SerializeField] private Transform roomTransform;

    [Header("DEBUG")]
    [SerializeField] List<PlaceableObject> placeableObjects;
    [SerializeField] List<LandingPad> landingPads;
    [SerializeField] List<TransactionDesk> transactionDesks;
    [SerializeField] List<RoomObject> rooms = new List<RoomObject>();
    
    NavMeshSurface navMeshSurface;

    public static event Action<RoomObject> OnRoomAdded;
    public static event Action<PlaceableObject> OnPlaceableAdded;

    public static GameObject Object => Instance.gameObject;
    public static StationMoney Money => Instance.stationMoney;
    public static StationRating Rating => Instance.stationRating;
    public static StationStaffManager Staff => Instance.staffManager;
    public static List<LandingPad> LandingPads => Instance.landingPads;
    public static List<TransactionDesk> TransactionDesks => Instance.transactionDesks;
    public static NavMeshSurface NavMeshSurface => Instance.navMeshSurface;
    public static List<RoomObject> Rooms => Instance.rooms;
    public static ShipManager ShipManager => Instance.shipManager;
    public static CustomerManager CustomerManager => Instance.customerManager;
    public static bool HasRooms => Instance.rooms.Count > 0;
    public static StationSupplies Supplies => Instance.stationSupplies;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(this);
    }

    public static void AddPlaceable(PlaceableObject newobject)
    {
        if (newobject.TryGetComponent<LandingPad>(out LandingPad landingPad))
        {
            Instance.landingPads.Add(landingPad);
        }
        else if (newobject.TryGetComponent<TransactionDesk>(out TransactionDesk transactionDesk))
        {
            Instance.transactionDesks.Add(transactionDesk);
        }
        else
        {
            Instance.placeableObjects.Add(newobject);
        }

        //newobject.transform.parent = Instance.transform;

        OnPlaceableAdded?.Invoke(newobject);
    }

    public static void RemovePlaceable(PlaceableObject newobject)
    {
        if (newobject.TryGetComponent<LandingPad>(out LandingPad landingPad))
        {
            Instance.landingPads.Remove(landingPad);
        }
        else if (newobject.TryGetComponent<TransactionDesk>(out TransactionDesk transactionDesk))
        {
            Instance.transactionDesks.Remove(transactionDesk);
        }
        else
        {
            Instance.placeableObjects.Remove(newobject);
        }
    }

    public static void SetNavMeshSurface(NavMeshSurface navMesh)
    {
        Instance.navMeshSurface = navMesh;
    }

    public static bool TryGetAvialableTransactionDesk(out TransactionDesk transactionDesk)
    {
        foreach (TransactionDesk desk in Instance.transactionDesks)
        {
            if (desk.Room.Config.Type != Room.EType.FuelPurchase && desk.IsAvailable)
            {
                transactionDesk = desk;
                return true;
            }
        }

        transactionDesk = null;
        return false;
    }

    public static bool TryGetAvialableFuelDesk(out TransactionDesk transactionDesk)
    {
        foreach (TransactionDesk desk in Instance.transactionDesks)
        {
            if (desk.Room.Config.Type == Room.EType.FuelPurchase && desk.IsAvailable)
            {
                transactionDesk = desk;
                return true;
            }
        }

        transactionDesk = null;
        return false;
    }

    public static bool TryGetFuelPurchaseRoom(out RoomObject fuelPurchaseRoom)
    {
        foreach (RoomObject room in Rooms)
        {
            if (room.Config.Type == Room.EType.FuelPurchase
                && room.HasFreeSlots)
            {
                fuelPurchaseRoom = room;
                return true;
            }
        }

        fuelPurchaseRoom = null;
        return false;
    }

    public static bool TryGetInteractableRoom(out RoomObject interactableRoom)
    {
        foreach (RoomObject room in Rooms)
        {
            if (room.Config.Type == Room.EType.None
                && room.HasFreeSlots)
            {
                interactableRoom = room;
                return true;
            }
        }

        interactableRoom = null;
        return false;
    }

    public static void AddRoom(RoomObject room)
    {
        Instance.rooms.Add(room);
        room.transform.parent = Instance.roomTransform.transform;
        OnRoomAdded?.Invoke(room);
    }

    public static void RemoveRoom(RoomObject room)
    {
        Instance.rooms.Remove(room);
    }

    public static FloorTile TileAtLocation(Vector3 location)
    {
        foreach(RoomObject room in Instance.rooms)
        {
            foreach(FloorTile tile in room.Floor)
            {
                if (tile.transform.position == location)
                {
                    return tile;
                }
            }
        }

        return null;
    }

    public static RoomObject RandomHallwayOrHanger
    {
        get
        {
            List<RoomObject> rooms = new List<RoomObject>();

            foreach (RoomObject room in Instance.rooms)
            {
                if (room.Config.Type == Room.EType.Hallway || room.Config.Type == Room.EType.Hanger)
                {
                    rooms.Add(room);
                }
            }

            if (rooms.Count > 0)
                return rooms[UnityEngine.Random.Range(0, rooms.Count)];

            return null;
        }
    }
}
