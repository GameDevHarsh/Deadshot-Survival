 using UnityEngine;
 using Core.Player;
using UnityEngine.InputSystem;

namespace Core.Inputs
{
    public class InputManager : MonoBehaviour
    {
        public bool canInteract;

        private PlayerInputBroadcaster playerInputBroadcaster;
        private PlayerInput playerInput;
        PlayerInputCallbacks callbacks;


        #region Initialization
        private void Awake()
        {
            playerInputBroadcaster = new PlayerInputBroadcaster();
            playerInput = GetComponent<PlayerInput>();
        }
        #endregion

        public PlayerInputBroadcaster PlayerInputBroadcaster
        {
            get
            {
                return playerInputBroadcaster;
            }
        }
        public PlayerInputCallbacks CallBacks
        {
            get
            {
                return callbacks;
            }
        }
    }       

}


