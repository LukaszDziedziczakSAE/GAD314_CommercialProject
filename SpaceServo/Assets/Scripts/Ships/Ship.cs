using System;
using UnityEngine;

public class Ship : MonoBehaviour
{
    [field: SerializeField] public EState State { get; private set; } = EState.Idle;
    [SerializeField] float speed;
    [SerializeField] float verticalSpeed;
    [SerializeField] float rotationSpeed = 10;
    [field: SerializeField] public ShipFuel Fuel { get; private set; } 

    public LandingPad LandingPad { get; private set; }
    public Customer Customer { get; private set; }
    private Quaternion targetRotation;
    public float RequiredFuel => Fuel.MaxAmount - Fuel.Amount;

    private Vector3 departurePoint;
    [field: SerializeField] public A_Ship Sound { get; private set; }

    public enum EState
    {
        Idle,
        Arriving,
        Landing,
        TakingOff,
        Leaving
    }

    private void Start()
    {
        targetRotation = transform.rotation;
    }

    private void Update()
    {
        if (State != EState.Leaving && LandingPad == null) return;

        switch (State)
        {
            case EState.Arriving:
                ArrivingUpdate();
                break;

            case EState.Landing:
                LandingUpdate();
                break;

            case EState.TakingOff: 
                TakeOffUpdate();
                break;

            case EState.Leaving: 
                LeavingUpdate();
                break;
        }

        RotationUpdate();
    }

    private void RotationUpdate()
    {
        if (transform.eulerAngles.y < targetRotation.eulerAngles.y)
        {
            float step = rotationSpeed * Time.deltaTime;
            Vector3 euler = transform.eulerAngles;

            if (targetRotation.eulerAngles.y - transform.eulerAngles.y > step)
            {
                euler.y += step;
            }

            else
            {
                euler.y = targetRotation.eulerAngles.y;
            }

            transform.eulerAngles = euler;
        }

        else if (transform.eulerAngles.y > targetRotation.eulerAngles.y)
        {
            float step = rotationSpeed * Time.deltaTime;
            Vector3 euler = transform.eulerAngles;

            if (transform.eulerAngles.y - targetRotation.eulerAngles.y > step)
            {
                euler.y -= step;
            }

            else
            {
                euler.y = targetRotation.eulerAngles.y;
            }

            transform.eulerAngles = euler;
        }
    }

    public void Initilize(LandingPad landingPad)
    {
        this.LandingPad = landingPad;
        State = EState.Arriving;
        transform.LookAt(landingPad.ArivalPosition);
        landingPad.CurrentShip = this;
        Sound.PlayRunningSound();
    }

    private void ArrivingUpdate()
    {
        Vector3 step = transform.forward * speed * Time.deltaTime;
        if (Vector3.Distance(transform.position, LandingPad.ArivalPosition) > Vector3.Distance(transform.position, transform.position + step))
        {
            transform.position += step;
        }
        else
        {
            transform.position = LandingPad.ArivalPosition;
            //transform.rotation = LandingPad.transform.rotation;
            targetRotation = LandingPad.transform.rotation;
            State = EState.Landing;
            Sound.PlayLandingSound();
        }
    }

    private void LandingUpdate()
    {
        float step = verticalSpeed * Time.deltaTime;
        if (step < transform.position.y - LandingPad.LandingLevel)
        {
            Vector3 position = transform.position;
            position.y -= step;
            transform.position = position;
        }
        else
        {
            Vector3 position = transform.position;
            position.y = LandingPad.LandingLevel;
            transform.position = position;

            State = EState.Idle;
            Customer = Station.CustomerManager.SpawnCustomer(LandingPad.CustomerSpawnPoint, this);
            Sound.Stop();
        }
    }

    private void TakeOffUpdate()
    {
        float step = verticalSpeed * Time.deltaTime;
        if (step < LandingPad.ArivalPosition.y - transform.position.y)
        {
            Vector3 position = transform.position;
            position.y += step;
            transform.position = position;
        }
        else
        {
            transform.position = LandingPad.ArivalPosition;
            //transform.LookAt(Station.ShipManager.DeparturePoint);
            State = EState.Leaving;
            LandingPad.CurrentShip = null;
            LandingPad = null;
            Sound.PlayRunningSound();
        }
    }

    private void LeavingUpdate()
    {
        Vector3 step = transform.forward * speed * Time.deltaTime;
        if (Vector3.Distance(transform.position, departurePoint) > Vector3.Distance(transform.position, transform.position + step))
        {
            transform.position += step;
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    private void OnDestroy()
    {
        if (Customer != null) Destroy(Customer.gameObject);
        Station.ShipManager.ShipDespawn(this);
    }

    public void BeginTakeOff()
    {
        State = EState.TakingOff;
        Customer = null;
        departurePoint = Station.ShipManager.RandomPoint(LandingPad.transform.position);

        targetRotation = Quaternion.LookRotation(departurePoint - transform.position, transform.up);
        Sound.PlayTakeOffSound();
    }

    public void BeginRefueling()
    {
        if (Fuel == null)
        {
            Debug.LogError(name + " missing refueling system");
            return;
        }

        LandingPad.Refueling.BeginRefueling(Fuel);
        LandingPad.Indicator.gameObject.SetActive(true);
    }
}
