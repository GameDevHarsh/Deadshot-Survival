using System;
using Core.Inputs;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Core.Player
{
    public class PlayerInputCallbacks 
    {
        public Action<Vector2> OnPlayerMoveFired = delegate { };
        public Action<Vector2> OnPlayerLookFired = delegate { };
        public Action<bool> OnPlayerJumpFired = delegate { };
        public Action<bool> OnPlayerReloadFired = delegate { };
        public Action<bool> OnPlayerSprintFired = delegate { };

        public Action<bool> OnPlayerStartAim = delegate { };
        public Action<bool> OnPlayerSwitchAimType = delegate { };
        public Action<bool> OnPlayerEndAim = delegate { };
        public Action<bool> OnPlayerShootFired = delegate { };
        
        public Action OnPlayerInteract = delegate { };
        public Action OnPausePressed = delegate { };
        public Action OnReturnFromPause = delegate { };
    }
}