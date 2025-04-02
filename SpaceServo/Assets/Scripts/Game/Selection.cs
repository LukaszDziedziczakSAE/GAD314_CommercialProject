using System;
using UnityEngine;

public class Selection : MonoBehaviour
{
    [SerializeField] Camera customerCameraPrefab;
    [field: SerializeField, Header("DEBUG")] public RoomObject Room { get; private set; }
    [field: SerializeField] public Customer Customer { get; private set; }

    Camera customerCamera;

    private bool selectionOverride => Game.FloorBuilder.IsPlacing || UI.IsRoomsMenuShowing || UI.IsPlaceablesMenuShowing || Game.PlaceableBuilder.IsPlacing;

    private void OnEnable()
    {
        Game.Input.OnPrimaryPress += OnPrimaryPress;
        Station.CustomerManager.OnCustomerDeparted += CustomerDeparted;
    }

    private void OnDisable()
    {
        Game.Input.OnPrimaryPress -= OnPrimaryPress;
        Station.CustomerManager.OnCustomerDeparted -= CustomerDeparted;
    }

    public void SelectRoom(RoomObject room)
    {
        DeselectCustomer();
        if (Room == room) return;
        DeselectRoom();
        Room = room;
        Room.ShowSelectedMaterial(true);
        UI.ShowRoomInfo();
        Debug.Log("Selected " + room.Config.Name + "(Room)");
    }

    public void DeselectRoom()
    {
        if (Room == null) return;
        UI.ShowRoomInfo(false);
        Room.ShowSelectedMaterial(false);
        Debug.Log("Deselected " + Room.Config.Name + "(Room)");
        Room = null;
    }

    public void SelectCustomer(Customer customer)
    {
        DeselectRoom();
        if (Customer == customer) return;
        DeselectCustomer();
        Customer = customer;
        customerCamera = Instantiate(customerCameraPrefab, Customer.transform);
        UI.ShowCustomerInfo();
    }

    public void DeselectCustomer()
    {
        if (Customer == null) return;
        UI.ShowCustomerInfo(false);
        Destroy(customerCamera.gameObject);
        Customer = null;
    }

    private void OnPrimaryPress()
    {
        if (selectionOverride) return;

        if (Game.CameraController == null)
        {
            Debug.LogError("Game is missing Camera Controller reference");
            return;
        }
        if (UI.MouseOverUI)
        {
            //Debug.LogWarning("Mouse over UI");
            return;
        }

        //print("selection click");
        Ray ray = Game.CameraController.Camera.ScreenPointToRay(Game.Input.MousePosition);
        float rayCastDistance = Game.CameraController.DistanceToGround + 50;

        if (Physics.Raycast(ray, out RaycastHit hit, rayCastDistance))
        {
            //print("hit " + hit.collider.name);

            if (hit.collider.TryGetComponent<PlaceableObject>(out PlaceableObject placeableObject))
            {
                RoomObject room = placeableObject.GetComponentInParent<RoomObject>();
                if (room != null) SelectRoom(room);
            }

            else if (hit.collider.TryGetComponent<FloorTile>(out FloorTile tile))
            {
                SelectRoom(tile.Room);
            }

            else if (hit.collider.TryGetComponent<Customer>(out Customer customer))
            {
                SelectCustomer(customer);
            }

            else
            {
                if (Room != null)
                {
                    DeselectRoom();
                }

                if (Customer != null)
                {
                    DeselectCustomer();
                }
            }
        }
        else
        {
            //Debug.LogWarning("Did not select anything");
            if (Room != null)
            {
                DeselectRoom();
            }

            if (Customer != null)
            {
                DeselectCustomer();
            }
        }
    }

    private void CustomerDeparted(Customer departedCustomer)
    {
        if (departedCustomer == Customer)
        {
            DeselectCustomer();
        }
    }
}
