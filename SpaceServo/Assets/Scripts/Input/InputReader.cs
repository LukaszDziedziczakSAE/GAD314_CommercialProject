using System;
using UnityEngine;
using UnityEngine.InputSystem;

// this script is like the bridge between the input system and all the other game's scripts

// TODO: camera zooming in/out controlls

public class InputReader : MonoBehaviour, InputSystem_Actions.IPlayerActions
{
    InputSystem_Actions actions;

    [field: SerializeField] public Vector2 MousePosition { get; private set; }
    [field: SerializeField] public Vector2 MouseDelta { get; private set; }
    [field: SerializeField] public bool PrimaryButtonDown { get; private set; }
    [field: SerializeField] public bool SecondaryButtonDown { get; private set; }
    [field: SerializeField] public Vector2 CameraMove { get; private set; }
    [field: SerializeField] public Vector2 CameraZoom { get; private set; }
    [field: SerializeField] public bool ContinueProcedure { get; private set; }

    public event Action OnPrimaryPress;
    public event Action OnPrimaryRelease;
    public event Action OnSecondaryPress;
    public event Action OnSecondaryRelease;
    public event Action OnRotatePress;
    public event Action OnCameraReset;

    private void Awake()
    {
        actions = new InputSystem_Actions();
        actions.Player.AddCallbacks(this);
        actions.Player.Enable();
    }

    public void OnMoveCamera(InputAction.CallbackContext context)
    {
        CameraMove = context.ReadValue<Vector2>();
    }

    public void OnPrimaryClick(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            PrimaryButtonDown = true;
            OnPrimaryPress?.Invoke();
        }
        else if (context.canceled)
        {
            PrimaryButtonDown = false;
            OnPrimaryRelease?.Invoke();
        }
    }

    public void OnSecondaryClick(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            SecondaryButtonDown = true;
            OnSecondaryPress?.Invoke();
        }
        else if (context.canceled)
        {
            SecondaryButtonDown = false;
            OnSecondaryRelease?.Invoke();
        }
    }

    public void OnMousePosition(InputAction.CallbackContext context)
    {
        MousePosition = context.ReadValue<Vector2>();
    }

    public void OnZoom(InputAction.CallbackContext context)
    {
        CameraZoom = context.ReadValue<Vector2>();
    }

    public void OnPause(InputAction.CallbackContext context)
    {
        Game.PauseGame(!Game.IsPaused);
    }

    public void OnMouseDelta(InputAction.CallbackContext context)
    {
        MouseDelta = context.ReadValue<Vector2>();
    }

    public void OnRotate(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            OnRotatePress?.Invoke();
        }
        else if (context.canceled)
        {
        }
    }

    public void OnContinueProcedure(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            ContinueProcedure = true;
        }
        else if (context.canceled)
        {
            ContinueProcedure = false;
        }
    }

    public void OnResetCamera(InputAction.CallbackContext context)
    {
        if (context.performed)
            OnCameraReset?.Invoke();
    }
}
