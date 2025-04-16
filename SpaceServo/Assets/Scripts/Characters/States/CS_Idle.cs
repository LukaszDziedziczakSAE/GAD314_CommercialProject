using UnityEngine;

// this is a state the customer can return to and it will work out what do to next
// customer should start at this state

public class CS_Idle : CustomerState
{
    float CrossFadeTime = 0.1f;

    public CS_Idle(Customer customer) : base(customer)
    {
        this.customer = customer;
    }

    public override void StateStart()
    {
        if (customer == null)
        {
            Debug.LogError("Customer State missing reference to Customer");
            return;
        }

        customer.Animator.CrossFade("Idle", CrossFadeTime);
    }

    public override void StateTick()
    {
        if (!customer.HasRefueled && !customer.Ship.LandingPad.IsRefueling
            && Station.Supplies.HaveSupplyOf(StationSupplies.ESupplyType.Fuel, (int)customer.Ship.RequiredFuel)
            && Station.TryGetFuelPurchaseRoom(out RoomObject fuelPurchaseRoom)
            && !customer.Info.RoomsVisited.Contains(fuelPurchaseRoom))
        {
            customer.SetNewState(new CS_MovingToRoom(customer, fuelPurchaseRoom));
        }

        else if (/*!customer.HasRefueled && customer.Ship.LandingPad.IsRefueling
            &&*/ Station.TryGetInteractableRoom(out RoomObject interactableRoom)
            && !customer.Info.RoomsVisited.Contains(interactableRoom))
        {
            customer.SetNewState(new CS_MovingToRoom(customer, interactableRoom));
        }

        else if (customer.HasRefueled)
        {
            customer.SetNewState(new CS_ReturningToShip(customer));
        }

        else // nothing else to do
        {
            customer.SetNewState(new CS_WonderingHallway(customer));
        }
    }

    public override void StateEnd()
    {

    }

    public override string Status()
    {
        return "Idle";

    }
}
