using System.Collections.Generic;
using UnityEngine;

public class StationSupplies : MonoBehaviour
{
    public enum ESupplyType
    {
        NoSupplies,
        Fuel,
        Food
    }

    [System.Serializable]
    public class Supply
    {
        public ESupplyType Type;
        public int Amount;

        public Supply(ESupplyType type)
        {
            Type = type;
            Amount = 0;
        }

        public Supply(ESupplyType type, int amount)
        {
            Type = type;
            Amount = amount;
        }

        public void RemoveAmount(int amount)
        {
            Amount -= amount;
        }

        public void AddAmount(int amount)
        {
            Amount += amount;
        }
    }

    [SerializeField] private List<Supply> InitialSupplies = new List<Supply>();
    [SerializeField] private List<Supply> SupplyCosts = new List<Supply>();
    [field: SerializeField] public List<Supply> Supplies { get; private set; } = new List<Supply>();

    private void Start()
    {
        foreach(Supply IniSupply in InitialSupplies)
        {
            Supplies.Add(new Supply(IniSupply.Type, IniSupply.Amount));
        }
    }

    public int SupplyOf(ESupplyType type)
    {
        foreach (Supply supply in Supplies)
        {
            if (supply.Type == type) return supply.Amount;
        }
        return -1;
    }

    public int MaxSupplyOf(ESupplyType type)
    {
        foreach (Supply initSupply in InitialSupplies)
        {
            if (initSupply.Type == type) return initSupply.Amount;
        }
        return -1;
    }

    public int RoomForSupplyOf(ESupplyType type)
    {
        int Supply = SupplyOf(type);
        int MaxSupply = MaxSupplyOf(type);
        if (Supply < 0 || MaxSupply < 0) return -1;
        return MaxSupply - Supply;
    }

    public int CostForSupplies(ESupplyType type, int amount = 1)
    {
        foreach (Supply supplyCost in SupplyCosts)
        {
            if (supplyCost.Type == type) return supplyCost.Amount * amount;
        }
        return -1;
    }

    public bool HaveSupplyOf(ESupplyType type, int amount = 1)
    {
        if (type == ESupplyType.NoSupplies) return true;

        foreach (Supply supply in Supplies)
        {
            if (supply.Type == type) return supply.Amount >= amount;
        }
        return false;
    }

    public bool CanAfford(ESupplyType type, int amount = 1)
    {
        int cost = CostForSupplies(type, amount);
        if (cost < -1) return false;

        return cost <= Station.Money.Amount;
    }

    public bool TryCustomerPurchasedSupply(ESupplyType type, int amount = 1)
    {
        if (type == ESupplyType.NoSupplies) return true;
        if (HaveSupplyOf(type, amount))
        {
            for(int i = 0; i < Supplies.Count; i++)
            {
                if (Supplies[i].Type == type)
                {
                    //Debug.Log("Removing " + amount + " " + type.ToString() + " from " + Supplies[i].Amount);
                    Supplies[i].RemoveAmount(amount);
                    //Debug.Log("Remaining: " + Supplies[i].Amount);
                    return true;
                }
            }
        }
        return false;
    }

    public bool TryPurchaseSupplyForStation(ESupplyType type, int amount = 1)
    {
        if (CanAfford(type, amount))
        {
            for (int i = 0; i < Supplies.Count; i++)
            {
                if (Supplies[i].Type == type)
                {
                    Supplies[i].AddAmount(amount);
                    Station.Money.Remove(CostForSupplies(type, amount));
                    return true;
                }
            }
        }
        return false;
    }

    public int AmountCanAfford(ESupplyType type)
    {
        return Station.Money.Amount / CostForSupplies(type);
    }
}
