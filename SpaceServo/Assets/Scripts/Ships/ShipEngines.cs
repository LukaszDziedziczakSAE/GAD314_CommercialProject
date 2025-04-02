using UnityEngine;

public class ShipEngines : MonoBehaviour
{
    [SerializeField] ParticleSystem[] Engines;

    public void TurnOn()
    {
        foreach (ParticleSystem engine in Engines)
        {
            engine.Play();
        }
    }

    public void TurnOff()
    {
        foreach (ParticleSystem engine in Engines)
        {
            engine.Stop();
        }
    }
}
