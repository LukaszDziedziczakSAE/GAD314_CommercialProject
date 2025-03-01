using System.Collections.Generic;
using UnityEngine;

public class TransactionDesk : PlaceableObject
{
    [field: SerializeField] public Transform CustomerPositionTransform {  get; private set; }
    [field: SerializeField] public Transform StaffPosition { get; private set; }
    [field: SerializeField] public float TransactionBaseTime { get; private set; } = 1.5f;
    [field: SerializeField] public Customer CurrentCustomer;
    [field: SerializeField] public List<Customer> CustomerQueue = new List<Customer>();
    [SerializeField] int MaxQueueSize = 3;
    [field: SerializeField] public StaffMember StaffMember;
    float timer;

    public bool IsAvailable => StaffMember != null && CustomerQueue.Count < MaxQueueSize;

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
        StaffMember = Station.Staff.SpawnNew(StaffPosition);
        StaffMember.SetNewState(new SMS_SittingIdle(StaffMember));
        UI.UpdateRoomInfo();
    }

    public void BeginTransaction()
    {
        CurrentCustomer.SetNewState(new CS_CompletingTransaction(CurrentCustomer, this));
        StaffMember.SetNewState(new SMS_SittingTyping(StaffMember));
    }

    public void CompleteTransaction()
    {
        // apply customer stat changes here

        if (Room.Config.Type == global::Room.EType.FuelPurchase) CurrentCustomer.Ship.BeginRefueling();

        CurrentCustomer.SetNewState(new CS_Idle(CurrentCustomer));
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

        return CustomerPositionTransform.position + (-1 * CustomerPositionTransform.forward * positionInQue);
    }
}


