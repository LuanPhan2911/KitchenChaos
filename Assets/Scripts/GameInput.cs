using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameInput : MonoBehaviour
{

    private PlayerInputActions playerInputActions;
    public event EventHandler OnInteractAction;
    public event EventHandler OnInteractAlternateAction;
    public event EventHandler OnPauseAction;

    private const string PLAYER_PREFS_BINDINGS = "PlayerPrefsBinding";

    public static GameInput Instance { get; private set; }
    public enum Binding
    {
        MoveUp,
        MoveDown,
        MoveLeft,
        MoveRight,
        Interact,
        InteractAlternate,
        Escape
    }

    private void Awake()
    {
        Instance = this;
        playerInputActions = new PlayerInputActions();

        if (PlayerPrefs.HasKey(PLAYER_PREFS_BINDINGS))
        {
            playerInputActions.LoadBindingOverridesFromJson(PlayerPrefs.GetString(PLAYER_PREFS_BINDINGS));
        }

        playerInputActions.Player.Enable();

        playerInputActions.Player.Interact.performed += Interact;
        playerInputActions.Player.InteractAlternate.performed += InteractAlternate;
        playerInputActions.Player.Pause.performed += Pause;

    }
    private void OnDestroy()
    {
        playerInputActions.Player.Interact.performed -= Interact;
        playerInputActions.Player.InteractAlternate.performed -= InteractAlternate;
        playerInputActions.Player.Pause.performed -= Pause;
        playerInputActions.Dispose();
    }

    private void Pause(InputAction.CallbackContext context)
    {
        OnPauseAction?.Invoke(this, EventArgs.Empty);
    }

    private void Interact(InputAction.CallbackContext obj)
    {
        OnInteractAction?.Invoke(this, EventArgs.Empty);
    }
    private void InteractAlternate(InputAction.CallbackContext obj)
    {
        OnInteractAlternateAction?.Invoke(this, EventArgs.Empty);
    }
    public Vector2 GetMovementVectorNormalized()
    {
        Vector2 inputVector = playerInputActions.Player.Move.ReadValue<Vector2>();

        inputVector.Normalize();
        return inputVector;
    }

    public string GetBindingText(Binding binding)
    {
        return binding switch
        {
            Binding.MoveUp => playerInputActions.Player.Move.bindings[1].ToDisplayString(),
            Binding.MoveDown => playerInputActions.Player.Move.bindings[2].ToDisplayString(),
            Binding.MoveLeft => playerInputActions.Player.Move.bindings[3].ToDisplayString(),
            Binding.MoveRight => playerInputActions.Player.Move.bindings[4].ToDisplayString(),
            Binding.Interact => playerInputActions.Player.Interact.bindings[0].ToDisplayString(),
            Binding.InteractAlternate => playerInputActions.Player.InteractAlternate.bindings[0].ToDisplayString(),
            Binding.Escape => playerInputActions.Player.Pause.bindings[0].ToDisplayString(),
            _ => ""

        };
    }

    public void BindingKey(Binding binding, Action onActionRebound)
    {
        playerInputActions.Player.Disable();
        InputAction inputAction;
        int index;
        switch (binding)
        {
            default:
            case Binding.MoveUp:
                index = 1;
                inputAction = playerInputActions.Player.Move;
                break;
            case Binding.MoveDown:
                index = 2;
                inputAction = playerInputActions.Player.Move;
                break;
            case Binding.MoveLeft:
                index = 3;
                inputAction = playerInputActions.Player.Move;
                break;
            case Binding.MoveRight:
                index = 4;
                inputAction = playerInputActions.Player.Move;
                break;
            case Binding.Interact:
                index = 0;
                inputAction = playerInputActions.Player.Interact;
                break;
            case Binding.InteractAlternate:
                index = 0;
                inputAction = playerInputActions.Player.InteractAlternate;
                break;
            case Binding.Escape:
                index = 0;
                inputAction = playerInputActions.Player.Pause;
                break;
        }

        inputAction.PerformInteractiveRebinding(index)
        .OnComplete(callback =>
                   {
                       callback.Dispose();
                       playerInputActions.Player.Enable();
                       PlayerPrefs.SetString(PLAYER_PREFS_BINDINGS, playerInputActions.SaveBindingOverridesAsJson());
                       PlayerPrefs.Save();
                       onActionRebound();

                   }
               )
        .Start();
    }
}
