using System;
using UnityEngine;
using UnityEngine.Windows;

// controls the camera

// TODO: paning when mouse is on screen edge
// TODO: paning on while holding right mouse button

public class CameraController : MonoBehaviour
{
    [field: SerializeField] public Camera Camera { get; private set; }
    [field: SerializeField] public float DistanceToGround { get; private set; }
    [field: SerializeField] public float Angle { get; private set; } = 75;

    [Header("Zoom")]
    [SerializeField] float minZoom;
    [SerializeField] float zoomThreshhold; // for angle change
    [SerializeField] float maxZoom;
    [SerializeField] float zoomSpeed;
    [SerializeField] float zoomedAngle; 
    [SerializeField] float normalAngle = 75;
    [Header("Right-Click Pan")]
    [SerializeField, Tooltip("Units/Second")] float panSpeed;
    [SerializeField] float panDelay = 0.5f;
    [Header("Edge Pan")]
    [SerializeField] float edgePanSpeed;
    [SerializeField] float edgePanDelay = 0.1f;
    [SerializeField] float edgeSize = 20f;

    InputReader input => Game.Input;
    bool paning;
    float deselectStartTime;
    float deselectDuration => Time.time - deselectStartTime;
    bool edgePaning;
    float edgePanStartTime;
    float edgePanDuration => Time.time - edgePanStartTime;

    private void OnEnable()
    {
        input.OnSecondaryPress += Input_OnMouseSecondaryPress;
        input.OnSecondaryRelease += Input_OnMouseSecondaryRelease;
        input.OnCameraReset += OnCameraReset;
    }

    private void OnDisable()
    {
        input.OnSecondaryPress -= Input_OnMouseSecondaryPress;
        input.OnSecondaryRelease -= Input_OnMouseSecondaryRelease;
        input.OnCameraReset -= OnCameraReset;
    }

    void Update()
    {
        EdgePaning();
        Paning();
        Zoom();
        KeyboardPan();
        SetCameraPosition();
    }

    private void Input_OnMouseSecondaryRelease()
    {
        paning = false;
    }

    private void Input_OnMouseSecondaryPress()
    {
        paning = true;
        deselectStartTime = Time.time;
    }

    private void SetCameraPosition()
    {
        if (DistanceToGround < zoomThreshhold)
        {
            float progress = (DistanceToGround - minZoom) / (zoomThreshhold - minZoom);
            Angle = Mathf.Lerp(zoomedAngle, normalAngle, progress);
        }
        else Angle = normalAngle;

            float y = Mathf.Cos(Angle) * DistanceToGround; 
        float z = Mathf.Sin(Angle) * DistanceToGround;
        Camera.transform.localPosition = new Vector3(0, y, z);
        Camera.transform.localEulerAngles = new Vector3(Angle, 0, 0);
    }

    private void KeyboardPan()
    {
        if (Game.Input.CameraMove.magnitude > 0)
        {
            Vector3 position = transform.position;
            position.x += Game.Input.CameraMove.x * panSpeed * Time.deltaTime;
            position.z += Game.Input.CameraMove.y * panSpeed * Time.deltaTime;
            transform.position = position;
        }
    }

    private void Zoom()
    {
        if (UI.MouseOverUI) return;

        if (Game.Input.CameraZoom.y != 0)
        {
            DistanceToGround = Mathf.Clamp(DistanceToGround + (zoomSpeed * -Game.Input.CameraZoom.y * Time.deltaTime), minZoom, maxZoom);
            
        }
    }

    private void EdgePaning()
    {
        if (!edgePaning && mouseOnEdge)
        {
            edgePaning = true;
            edgePanStartTime = Time.time;
        }
        else if (edgePaning && !mouseOnEdge)
        {
            edgePaning = false;
        }

        if (edgePaning && edgePanDuration > edgePanDelay)
        {
            Vector3 position = transform.position;

            if (input.MousePosition.x < 0 + edgeSize)
            {
                position.x -= panSpeed * Time.deltaTime;
                //position.z += -input.MouseDelta.y * panSpeed * Time.deltaTime;
            }
            else if (input.MousePosition.x > Screen.width - edgeSize)
            {
                position.x += panSpeed * Time.deltaTime;
            }

            if (input.MousePosition.y < 0 + edgeSize)
            {
                position.z -= panSpeed * Time.deltaTime;
            }
            else if (input.MousePosition.y > Screen.height - edgeSize)
            {
                position.z += panSpeed * Time.deltaTime;
            }

            transform.position = position;
        }


    }

    private void Paning()
    {
        if (!paning || deselectDuration < panDelay) return;

        Vector3 position = transform.position;
        position.x += -input.MouseDelta.x * panSpeed * Time.deltaTime;
        position.z += -input.MouseDelta.y * panSpeed * Time.deltaTime;
        transform.position = position;
    }

    private bool mouseOnEdge
    {
        get
        {
            return (input.MousePosition.x < 0 + edgeSize) || (input.MousePosition.x > Screen.width - edgeSize)
                || (input.MousePosition.y < 0 + edgeSize) || (input.MousePosition.y > Screen.height - edgeSize);
        }
    }

    public float ScrollSpeed => panSpeed;

    public void SetScrollSpeed(float newSpeed)
    {
        panSpeed = newSpeed;
    }

    private void OnCameraReset()
    {
        Vector3 positon = transform.position;
        positon.x = 0;
        positon.z = 0;
        transform.position = positon;
    }
}
