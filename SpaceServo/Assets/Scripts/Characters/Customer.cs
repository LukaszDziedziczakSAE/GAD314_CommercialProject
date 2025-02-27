using UnityEngine;
using UnityEngine.Rendering;

public class Customer : Character
{
    [field: SerializeField] public Ship Ship { get; private set; }

    public bool HasBoughtFuel; // this is to be replaced once fuel system is in
    Transform target;
    public bool KeepIdle;
    [field: SerializeField] public EntityStat Satisfaction { get; private set; } = new();

    protected override void Update()
    {
        base.Update();

        /**
        if (KeepIdle) return;

        else if (!IsMoving && target == null)
        {
            if (!hasBoughtFuel && Station.TryGetAvialableTransactionDesk(out TransactionDesk desk))
            {
                print("customer moving to buy fuel");
                target = desk.CustomerPosition;
                NavMeshAgent.SetDestination(target.position);
                desk.CurrentCustomer = this;
            }

            else if (hasBoughtFuel) // later this will change to "shipHasRefuled" and customer finished w/e they were doing
            {
                target = Ship.LandingPad.CustomerSpawnPoint;
                NavMeshAgent.SetDestination(target.position);
            }

        }

        else if (hasBoughtFuel && !IsMoving && HasArrivedAtNavMeshDestination && target == Ship.LandingPad.CustomerSpawnPoint)
        {
            // apply customer satisfaction to station rating
            float custStatisfactionDifference = Satisfaction.ValueCurrent - Satisfaction.ValueBase;
            if (custStatisfactionDifference > 0)
            {
                Debug.Log($"Customer Satisfaction: {Satisfaction.ValueCurrent.ToString()}, Positive by {custStatisfactionDifference}");
            }
            else if (custStatisfactionDifference < 0)
            {
                Debug.Log($"Customer Satisfaction: {Satisfaction.ValueCurrent.ToString()}, Negative by {custStatisfactionDifference}");
            }
            else
            {
                Debug.Log($"Customer Satisfaction: {Satisfaction.ValueCurrent.ToString()}, Neutral");
            }
            Ship.BeginTakeOff();
            Destroy(this.gameObject);
        }

        */
    }

    public void Initilize(Ship ship)
    {
        Ship = ship;
        SetNewState(new CS_Idle(this));
    }

    private void OnDestroy()
    {
        Station.CustomerManager.CustomerDespawn(this);
    }

    public void DestoryCustomer()
    {
        Ship.BeginTakeOff();

        // TODO: replace this with adding to station rating
        // apply customer satisfaction to station rating
        float custStatisfactionDifference = Satisfaction.ValueCurrent - Satisfaction.ValueBase;
        if (custStatisfactionDifference > 0)
        {
            Debug.Log($"Customer Satisfaction: {Satisfaction.ValueCurrent.ToString()}, Positive by {custStatisfactionDifference}");
        }
        else if (custStatisfactionDifference < 0)
        {
            Debug.Log($"Customer Satisfaction: {Satisfaction.ValueCurrent.ToString()}, Negative by {custStatisfactionDifference}");
        }
        else
        {
            Debug.Log($"Customer Satisfaction: {Satisfaction.ValueCurrent.ToString()}, Neutral");
        }

        Destroy(this.gameObject);
    }
}