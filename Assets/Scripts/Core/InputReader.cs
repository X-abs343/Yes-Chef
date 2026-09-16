using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace YesChef.Core
{
    [DefaultExecutionOrder(-100)]
    public class InputReader : MonoBehaviour, KitchenInputActions.IGameplayActions, KitchenInputActions.IDebugActions
    {
        public static InputReader Instance { get; private set; }

        public event Action<Vector2> OnMoveEvent;
        public event Action OnInteractEvent;
        public event Action OnPauseEvent;

        // Debug actions for feeling & juice testing
        public event Action OnDebugCelebration;
        public event Action OnDebugSadTilt;
        public event Action OnDebugStopSquash;

        public Vector2 MoveComposite { get; private set; }

        private KitchenInputActions _actions;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            _actions = new KitchenInputActions();
            _actions.Gameplay.SetCallbacks(this);
            _actions.Debug.SetCallbacks(this);
        }

        private void OnEnable()
        {
            _actions.Gameplay.Enable();
            _actions.Debug.Enable();
        }

        private void OnDisable()
        {
            _actions.Gameplay.Disable();
            _actions.Debug.Disable();
        }

        #region Gameplay Actions
        public void OnMove(InputAction.CallbackContext context)
        {
            MoveComposite = context.ReadValue<Vector2>();
            OnMoveEvent?.Invoke(MoveComposite);
        }

        public void OnInteract(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                OnInteractEvent?.Invoke();
            }
        }

        // Unused template actions implemented to satisfy IGameplayActions
        public void OnLook(InputAction.CallbackContext context) { }
        public void OnAttack(InputAction.CallbackContext context) { }
        public void OnCrouch(InputAction.CallbackContext context) { }
        public void OnJump(InputAction.CallbackContext context) { }
        public void OnPrevious(InputAction.CallbackContext context) { }
        public void OnNext(InputAction.CallbackContext context) { }
        public void OnSprint(InputAction.CallbackContext context) { }
        #endregion

        #region Debug Actions
        public void OnTestCelebration(InputAction.CallbackContext context)
        {
            if (context.performed) OnDebugCelebration?.Invoke();
        }

        public void OnTestSadTilt(InputAction.CallbackContext context)
        {
            if (context.performed) OnDebugSadTilt?.Invoke();
        }

        public void OnTestStopSquash(InputAction.CallbackContext context)
        {
            if (context.performed) OnDebugStopSquash?.Invoke();
        }
        #endregion
    }
}