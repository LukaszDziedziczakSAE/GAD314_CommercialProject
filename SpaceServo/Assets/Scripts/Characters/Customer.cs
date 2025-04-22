using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class Customer : Character
{
    [System.Serializable]
    public class CustomerInfo
    {
        [field: SerializeField] public string Name;
        [field: SerializeField] public float Satisfaction;
        [field: SerializeField] public List<RoomObject> RoomsVisited = new List<RoomObject>();
    }

    //[field: SerializeField] public EntityStat Satisfaction { get; private set; } = new();
    [field: SerializeField] public Ship Ship { get; private set; }
    [field: SerializeField] public CustomerInfo Info { get; private set; } = new CustomerInfo();
    [field: SerializeField] public List<Placeable> RemainingInteractions = new List<Placeable>();

    public bool HasRefueled => Ship.Fuel.HasRefueled;
    public bool RefuelingStarted => HasRefueled || Ship.LandingPad.IsRefueling;
    Transform target;
    
    protected override void Update()
    {
        base.Update();
    }

    public void Initilize(Ship ship)
    {
        Ship = ship;
        Info.Name = name;
        Info.Satisfaction = Station.Rating.MIN_RATING;
        SetNewState(new CS_Idle(this));
    }

    private void OnDestroy()
    {
        Ship.BeginTakeOff();
        Station.CustomerManager.CustomerDespawn(this);
    }

    public void DestoryCustomer()
    {
        Destroy(this.gameObject);
    }

    public void ModifySatisfaction(float amount)
    {
        Info.Satisfaction = Mathf.Clamp(Info.Satisfaction + amount, Station.Rating.MIN_RATING, Station.Rating.MAX_RATING);
    }
}