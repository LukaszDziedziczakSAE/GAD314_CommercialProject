using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// this manages the player's station raiting

public class StationRating : MonoBehaviour
{
    // treat these as const's
    [field: SerializeField] public float MIN_RATING { get; private set; } = 0f;
    [field: SerializeField] public float MAX_RATING { get; private set; } = 50f;
    [field: SerializeField] public int LastCustomers { get; private set; } = 10; // numbers of customers to look at to get raiting

    //public event Action OnRatingChange;

    private void OnEnable()
    {
        Station.CustomerManager.OnCustomerDeparted += AddCustomerSatisfaction;
    }

    private void OnDisable()
    {
        Station.CustomerManager.OnCustomerDeparted -= AddCustomerSatisfaction;
    }


    public float Value // a number between 0 and 50, each 10 represents a full star
    {
        get
        {
            if (values.Count == 0) return 0;

            float average = 0;

            if (values.Count > 10)
            {
                for (int i = values.Count - LastCustomers; i < values.Count; i++)
                {
                    average += values[i];
                }
                average = average / LastCustomers;
            }
            else
            {
                foreach (float value in values)
                {
                    average += value;
                }
                average = average / values.Count;
            }

                
            //Mathf.Clamp(average, 0f, 1.0f); // the value should already be clamed before it even gets here
            return average;
        }
    }

    /*public float Value // a number between 0 and 50, each 10 represents a full star
    {
        get
        {
            return Station.CustomerManager.AverageLastDepartedRating(LastCustomers);
        }
    }*/

    private List<float> values = new List<float>();

    void Start()
    {
        UI.UpdateRatingVisual();
        //UI.UpdateRatingText();
    }

    public void AddCustomerSatisfaction(Customer customer)
    {
        values.Add(Mathf.Clamp(customer.Info.Satisfaction, MIN_RATING, MAX_RATING));
        //OnRatingChange?.Invoke();
        UI.UpdateRatingVisual();
    }
}