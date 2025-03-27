using System;
using Core.Inputs;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Core.Player
{
    public class PlayerInputBroadcaster
    {
        private PlayerInputCallbacks callbacks;
        private GameInputs gameInputs;
        
        [Header("Input Scripts")]
        public PlayerInputs playerInputs;


        #region Constructor
        public PlayerInputBroadcaster()
        {
            callbacks = new PlayerInputCallbacks();
            gameInputs = new GameInputs();
            playerInputs = new PlayerInputs(gameInputs, callbacks);

            EnableAction();
        }
        #endregion

        public void Destroy()
        {
            gameInputs.Dispose();
        }

        public void EnableAction()
        {

            DisableActions();
            playerInputs.Enable();
        }

        private void DisableActions()
        {
            playerInputs.Disable();
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