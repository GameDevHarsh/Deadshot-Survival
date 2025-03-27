using Core.Player;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Core.Inputs
{
    public class PlayerInputs : MonoBehaviour, GameInputs.IPlayerInputsActions
    {
        [Header("Character Input Values")]
        public Vector2 move;
        public Vector2 look;
        public bool jump;
        public bool sprint;
        public bool aim;    //NEW
        public bool shoot;    //NEW
        public bool interact;    //NEW
        public bool ignoreInputs;
        public bool reload;

        public bool resetAim;
        private bool switchToHoldMode;

        [Header("Movement Settings")]
        public bool analogMovement;

        [Header("Mouse Cursor Settings")]
        public bool cursorLocked = true;
        public bool cursorInputForLook = true;

        private GameInputs gameInputs;
        private PlayerInputCallbacks callbacks;


        #region Constructor
        public PlayerInputs(GameInputs gameInputs, PlayerInputCallbacks callbacks)
        {
            this.gameInputs = gameInputs;
            this.callbacks = callbacks;

            gameInputs.PlayerInputs.SetCallbacks(this);
        }
        #endregion

        #region Enable/Disable
        public void Enable()
        {
            gameInputs.PlayerInputs.Enable();
           // Cursor.lockState = CursorLockMode.Locked;
            //Cursor.visible = false;
            Application.isFocused.Equals(true);
        }

        public void Disable()
        {
            gameInputs.PlayerInputs.Disable();
           // Cursor.lockState = CursorLockMode.None;
        }
        #endregion

#if ENABLE_INPUT_SYSTEM
        #region Inputs
        public void OnMove(InputAction.CallbackContext context)
        {
            move = context.ReadValue<Vector2>();
            callbacks.OnPlayerMoveFired?.Invoke(move);
        }

        public void OnLook(InputAction.CallbackContext context)
        {
            look = context.ReadValue<Vector2>();
            callbacks.OnPlayerLookFired?.Invoke(look);
        }

        public void OnJump(InputAction.CallbackContext context)
        {
            jump = context.performed;
            callbacks.OnPlayerJumpFired?.Invoke(jump);
        }

        public void OnReload(InputAction.CallbackContext context)
        {
            reload = context.performed;
            callbacks.OnPlayerReloadFired?.Invoke(reload);
        }

        public void OnSprint(InputAction.CallbackContext context)
        {
            switch (context.phase)
            {
                case InputActionPhase.Performed:
                    sprint = true;
                    callbacks.OnPlayerSprintFired?.Invoke(sprint);
                    break;

                case InputActionPhase.Canceled:
                    sprint = false;
                    callbacks.OnPlayerSprintFired?.Invoke(sprint);
                    break;
            }
        }

        public void OnAim(InputAction.CallbackContext context)
        {
            callbacks.OnPlayerSwitchAimType += (type) =>
            {
                switchToHoldMode = type;
            };

            if (switchToHoldMode)
            {
                //Hold Aim Section
                switch (context.phase)
                {
                    case InputActionPhase.Performed:
                        aim = true;
                        callbacks.OnPlayerStartAim?.Invoke(aim);
                        break;

                    case InputActionPhase.Canceled:
                        aim = false;
                        callbacks.OnPlayerStartAim?.Invoke(aim);
                        break;
                }
            }
            else
            {
                //Toggle Aim Section
                switch (context.phase)
                {
                    case InputActionPhase.Performed:
                        if (!resetAim)   //Similar to : if(resetAim == false)
                            aim = true;
                        else
                            aim = false;

                        callbacks.OnPlayerStartAim?.Invoke(aim);
                        if (aim == false)    //Similar to : if(!aim)
                            resetAim = false;
                        else
                            resetAim = true;
                        break;
                }
            }
        }

        public void OnShoot(InputAction.CallbackContext context)
        {
            switch (context.phase)
            {
                case InputActionPhase.Performed:
                    shoot = true;
                    callbacks.OnPlayerShootFired?.Invoke(shoot);
                    break;

                case InputActionPhase.Canceled:
                case InputActionPhase.Disabled:
                    shoot = false;
                    callbacks.OnPlayerShootFired?.Invoke(shoot);
                    break;
            }
        }

        public void OnPause(InputAction.CallbackContext context)
        {
            switch (context.phase)
            {
                case InputActionPhase.Performed:
                    callbacks.OnPausePressed?.Invoke();
                    break;
            }
        }

        public void OnInteract(InputAction.CallbackContext context)
        {
            switch (context.phase)
            {
                case InputActionPhase.Performed:
                    callbacks.OnPlayerInteract?.Invoke();
                    break;
            }
        }
        #endregion
#endif

        private void OnApplicationFocus(bool hasFocus)
        {
            SetCursorState(cursorLocked);
        }

        private void SetCursorState(bool newState)
        {
            Cursor.lockState = newState ? CursorLockMode.Locked : CursorLockMode.None;
        }

        public void SetAimInputType(bool type)
        {
            if (type == true)
                switchToHoldMode = true;
            else
                switchToHoldMode = false;
        }
    }
}