using System;
using UnityEngine;
using UnityEngine.InputSystem;

public enum Binding
{
    Move_Up,
    Move_Down,
    Move_Left,
    Move_Right,
    Interact,
    InteractAlternate,
    Pause,
    Gamepad_Interact,
    Gamepad_InteractAlternate,
    Gamepad_Pause
}

public class GameInput : MonoBehaviour
{
    #region Singleton

    public static GameInput Instance { get; private set; }

    #endregion

    #region Events

    public event EventHandler OnInteractActionEvent;
    public event EventHandler OnInteractAlternateActionEvent;
    public event EventHandler OnPauseActionEvent;
    public event EventHandler OnBindingRebindEvent;

    #endregion

    #region Fields

    private const string PLAYER_PREFS_BINDINGS = "InputBindings";

    private PlayerInputActions playerInputActions;

    #endregion


    #region Override: Awake | OnDestroy

    private void Awake()
    {
        Instance = this;


        playerInputActions = new PlayerInputActions();

        if (PlayerPrefs.HasKey(PLAYER_PREFS_BINDINGS))
        {
            playerInputActions.LoadBindingOverridesFromJson(PlayerPrefs.GetString(PLAYER_PREFS_BINDINGS));
        }

        playerInputActions.Player.Enable();

        AddEvents();
    }

    private void OnDestroy()
    {
        RemoveEvents();

        playerInputActions.Dispose();
    }

    #endregion


    #region Get: MovementVectorNormalized

    public Vector2 GetMovementVectorNormalized()
    {
        Vector2 inputVector = playerInputActions.Player.Move.ReadValue<Vector2>();

        inputVector = inputVector.normalized;

        return inputVector;
    }

    #endregion

    #region Get: BindingText

    public string GetBindingText(Binding binding)
    {
        switch (binding)
        {
            default:
            case Binding.Move_Up:
                return playerInputActions.Player.Move.bindings[1].ToDisplayString();
            case Binding.Move_Down:
                return playerInputActions.Player.Move.bindings[2].ToDisplayString();
            case Binding.Move_Left:
                return playerInputActions.Player.Move.bindings[3].ToDisplayString();
            case Binding.Move_Right:
                return playerInputActions.Player.Move.bindings[4].ToDisplayString();
            case Binding.Interact:
                return playerInputActions.Player.Interact.bindings[0].ToDisplayString();
            case Binding.InteractAlternate:
                return playerInputActions.Player.InteractAlternate.bindings[0].ToDisplayString();
            case Binding.Pause:
                return playerInputActions.Player.Pause.bindings[0].ToDisplayString();
            case Binding.Gamepad_Interact:
                return playerInputActions.Player.Interact.bindings[1].ToDisplayString();
            case Binding.Gamepad_InteractAlternate:
                return playerInputActions.Player.InteractAlternate.bindings[1].ToDisplayString();
            case Binding.Gamepad_Pause:
                return playerInputActions.Player.Pause.bindings[1].ToDisplayString();
        }
    }

    #endregion

    
    #region Rebind: Binding

    public void RebindBinding(Binding binding, Action onActionRebound)
    {
        playerInputActions.Player.Disable();

        InputAction inputAction;
        int bindingIndex;

        switch (binding)
        {
            default:
            case Binding.Move_Up:
                inputAction = playerInputActions.Player.Move;
                bindingIndex = 1;
                break;
            case Binding.Move_Down:
                inputAction = playerInputActions.Player.Move;
                bindingIndex = 2;
                break;
            case Binding.Move_Left:
                inputAction = playerInputActions.Player.Move;
                bindingIndex = 3;
                break;
            case Binding.Move_Right:
                inputAction = playerInputActions.Player.Move;
                bindingIndex = 4;
                break;
            case Binding.Interact:
                inputAction = playerInputActions.Player.Interact;
                bindingIndex = 0;
                break;
            case Binding.InteractAlternate:
                inputAction = playerInputActions.Player.InteractAlternate;
                bindingIndex = 0;
                break;
            case Binding.Pause:
                inputAction = playerInputActions.Player.Pause;
                bindingIndex = 0;
                break;
            case Binding.Gamepad_Interact:
                inputAction = playerInputActions.Player.Interact;
                bindingIndex = 1;
                break;
            case Binding.Gamepad_InteractAlternate:
                inputAction = playerInputActions.Player.InteractAlternate;
                bindingIndex = 1;
                break;
            case Binding.Gamepad_Pause:
                inputAction = playerInputActions.Player.Pause;
                bindingIndex = 1;
                break;
        }

        inputAction.PerformInteractiveRebinding(bindingIndex)
            .OnComplete(callback =>
            {
                callback.Dispose();
                playerInputActions.Player.Enable();
                onActionRebound();

                PlayerPrefs.SetString(PLAYER_PREFS_BINDINGS, playerInputActions.SaveBindingOverridesAsJson());
                PlayerPrefs.Save();

                OnBindingRebindEvent?.Invoke(this, EventArgs.Empty);
            })
            .Start();
    }

    #endregion


    #region Event: OnPausePerformed

    private void OnPausePerformed(InputAction.CallbackContext obj)
    {
        OnPauseActionEvent?.Invoke(this, EventArgs.Empty);
    }

    #endregion

    #region Event: OnInteractAlternatePerformed

    private void OnInteractAlternatePerformed(InputAction.CallbackContext obj)
    {
        OnInteractAlternateActionEvent?.Invoke(this, EventArgs.Empty);
    }

    #endregion

    #region Event: OnInteractPerformed

    private void OnInteractPerformed(InputAction.CallbackContext obj)
    {
        OnInteractActionEvent?.Invoke(this, EventArgs.Empty);
    }

    #endregion

    #region Events: Add | Remove

    private void AddEvents()
    {
        playerInputActions.Player.Interact.performed += OnInteractPerformed;
        playerInputActions.Player.InteractAlternate.performed += OnInteractAlternatePerformed;
        playerInputActions.Player.Pause.performed += OnPausePerformed;
    }

    private void RemoveEvents()
    {
        playerInputActions.Player.Interact.performed -= OnInteractPerformed;
        playerInputActions.Player.InteractAlternate.performed -= OnInteractAlternatePerformed;
        playerInputActions.Player.Pause.performed -= OnPausePerformed;
    }

    #endregion
}