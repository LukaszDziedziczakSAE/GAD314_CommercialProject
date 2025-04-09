using System;
using System.Collections.Generic;
using UnityEngine;

public class TransactionDesk : PlaceableObject
{
    [field: SerializeField, Header("Transaction Desk Settings")] public Transform CustomerPositionTransform {  get; private set; }
    [field: SerializeField] public Transform StaffPosition { get; private set; }
    [field: SerializeField] public float TransactionBaseTime { get; private set; } = 1.5f;
    [field: SerializeField] public Customer CurrentCustomer;
    [field: SerializeField] public List<Customer> CustomerQueue = new List<Customer>();
    [SerializeField] int MaxQueueSize = 3;
    [field: SerializeField] public StaffMember StaffMember;
    float timer;
    [SerializeField] int unitBuyPrice = 1;
    [field: SerializeField] public UI_TransactionDeskIndicator Indicator { get; private set; }
    [field: SerializeField] public float SatisfactionChange { get; private set; }

    public bool IsAvailable => StaffMember != null && CustomerQueue.Count < MaxQueueSize;
    public int FreeCustomerSlots => MaxQueueSize - CustomerQueue.Count;

    public event Action<StaffMember> OnHireStaff;
    [field: SerializeField] public A_TransactionDesk Sound { get; private set; }

    protected override void Update()
    {
        base.Update();
        if (CurrentCustomer == null & CustomerQueue.Count > 0)
        {
            CurrentCustomer = CustomerQueue[0];
            CustomerQueue.RemoveAt(0);
        }
    }

    public override void SetPlaced()
    {
        base.SetPlaced();
    }

    public void HireStaffMember()
    {
        StaffMember = Station.Staff.SpawnNew(this, StaffPosition);
        StaffMember.SetNewState(new SMS_SittingIdle(StaffMember));
        OnHireStaff?.Invoke(StaffMember);
        UI.UpdateRoomInfo();
    }

    public void BeginTransaction()
    {
        CurrentCustomer.SetNewState(new CS_CompletingTransaction(CurrentCustomer, this));
        StaffMember.SetNewState(new SMS_SittingTyping(StaffMember));
    }

    public void CompleteTransaction()
    {
        CurrentCustomer.ModifySatisfaction(SatisfactionChange);

        if (Room.Config.Type == global::Room.EType.FuelPurchase)
        {
            if (Station.Supplies.TryCustomerPurchasedSupply(Room.Config.SupplyType, (int)CurrentCustomer.Ship.RequiredFuel))
            {
                Station.Money.Add((int)(CurrentCustomer.Ship.RequiredFuel * unitBuyPrice));
                CurrentCustomer.Ship.BeginRefueling();
                ShowMoneyIndicator((int)(CurrentCustomer.Ship.RequiredFuel * unitBuyPrice));
            }
            else
                Debug.LogError("Unable to TryCustomerPurchasedSupply");
        }
        else
        {
            if (Station.Supplies.TryCustomerPurchasedSupply(Room.Config.SupplyType))
            {
                Station.Money.Add(unitBuyPrice);
                ShowMoneyIndicator(unitBuyPrice);
            }
            else
                Debug.LogError("Unable to TryCustomerPurchasedSupply");
        }

        if (Sound == null) Sound = GetComponent<A_TransactionDesk>();
        if (Sound != null) Sound.PlayPurchaseSound();
        else Debug.LogError("Transaction Desk Missing ref to sound");
        
        CurrentCustomer.SetNewState(new CS_WonderingInRoom(CurrentCustomer, Room));
        StaffMember.SetNewState(new SMS_SittingIdle(StaffMember));

        CurrentCustomer = null;
    }

    public int CuePosition(Customer customer)
    {
        if (CurrentCustomer == customer) return 0;
        return CustomerQueue.IndexOf(customer) + 1;
    }

    public Vector3 CustomerPositionTarget(Customer customer)
    {
        int positionInQue = CuePosition(customer);

        if (positionInQue == 0) return CustomerPositionTransform.position;

        return CustomerPositionTransform.position + (-1 * CustomerPositionTransform.forward) + (-1 * CustomerPositionTransform.forward * positionInQue);
    }

    public void ShowMoneyIndicator(int moneyAmount)
    {
        Indicator.gameObject.SetActive(true);
        Indicator.Initilize(moneyAmount, transform.eulerAngles.y);
    }

    protected override void OnTriggerEnter(Collider other)
    {
        base.OnTriggerEnter(other);
    }
}


