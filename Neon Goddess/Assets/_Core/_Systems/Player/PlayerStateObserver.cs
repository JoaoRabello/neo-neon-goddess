using System;
using UnityEngine;

namespace Player
{
    /// <summary>
    /// Observer that watches the player states.
    /// Used to control whether player is free to action or not<br></br>
    /// Subscribe to the events (Actions) to receive PlayerState updates
    /// <example>
    /// > Player starts a Dialogue.<br></br>
    /// > Observer changes the state to OnDialogue.<br></br>
    /// > Player Movements are now blocked
    /// </example>
    /// <value> <c>• CurrentState</c>: represents the public access player's state.<br></br></value>
    /// <value> <c>• _isDuringState</c>: represents that the player is in a different state than Free.</value>
    /// </summary>
    public class PlayerStateObserver : MonoBehaviour
    {
        public static PlayerStateObserver Instance;
        
        /// <summary>
        /// Player States enumerator.
        /// <value> <c>• Free</c>: represents when player can action freely.<br></br></value>
        /// <value> <c>• Aiming</c>: represents when player is aiming a weapon.<br></br></value>
        /// <value> <c>• OnAnimation</c>: represents when player is on an animation (like ledge grab or surprise attacks).<br></br></value>
        /// <value> <c>• OnCutscene</c>: represents when player is being controlled by a cutscene.<br></br></value>
        /// <value> <c>• OnDialogue</c>: represents when player is on dialogues or barks due to interactions.<br></br></value>
        /// </summary>
        public enum PlayerState
        {
            /// <summary>
            /// When Player is free to act
            /// </summary>
            Free,
            /// <summary>
            /// When Player is aiming
            /// </summary>
            Aiming,
            /// <summary>
            /// When Player is on an animation
            /// </summary>
            OnAnimation,
            /// <summary>
            /// When Player is on a cutscene
            /// </summary>
            OnCutscene,
            /// <summary>
            /// When Player is on dialogue
            /// </summary>
            OnDialogue,
            /// <summary>
            /// When Player is dead
            /// </summary>
            Dead
        }

        public PlayerState _currentState;
        /// <summary>
        /// Current Player state
        /// </summary>
        public PlayerState CurrentState => _currentState;

        /// <summary>
        /// Event called when dialogues start
        /// </summary>
        public Action DialogueStart;
        /// <summary>
        /// Event called when dialogues end
        /// </summary>
        public Action DialogueEnd;
        
        /// <summary>
        /// Event called when aim start
        /// </summary>
        public Action AimStart;
        /// <summary>
        /// Event called when aim end
        /// </summary>
        public Action AimEnd;

        /// <summary>
        /// Event called when cutscenes start
        /// </summary>
        public Action CutsceneStart;
        /// <summary>
        /// Event called when cutscenes end
        /// </summary>
        public Action CutsceneEnd;

        /// <summary>
        /// Event called when animations start
        /// </summary>
        public Action AnimationStart;
        /// <summary>
        /// Event called when animations end
        /// </summary>
        public Action AnimationEnd;
        
        /// <summary>
        /// Event called when player dies
        /// </summary>
        public Action PlayerDied;

        private bool _isDuringState;
        
        //TODO: See if it's possible to build a state queue for situations like "Start cutscene and then start" + "a dialogue when ending cutscene" 

        private void Awake()
        {
            if (Instance is null)
            {
                Instance = this;
            }
            else
            {
                Destroy(this);
            }
        }
        
        private void OnDestroy()
        {
            Instance = null;
        }

        /// <summary>
        /// Configure every event to their respective observed objects
        /// </summary>
        private void SetupEvents()
        {
            
        }
        
        /// <summary>
        /// Starts a State, setting the current state to the new state and sets to during state flag
        /// </summary>
        /// <param name="state">State to start</param>
        private void StateStart(PlayerState state)
        {
            _isDuringState = true;

            _currentState = state;
        }

        /// <summary>
        /// Reset the during state flag and sets the state to Free
        /// </summary>
        private void StateEnd()
        {
            if (_currentState == PlayerState.Dead) { return; }
            _isDuringState = false;
            
            _currentState = PlayerState.Free;
        }

        public void OnDialogueStart()
        {
            if (_currentState == PlayerState.Dead) { return; }
            StateStart(PlayerState.OnDialogue);
            DialogueStart?.Invoke();
        }

        public void OnDialogueEnd()
        {
            StateEnd();
            DialogueEnd?.Invoke();
        }
        
        public void OnAimStart()
        {
            if (_currentState == PlayerState.Dead) { return; }
            StateStart(PlayerState.Aiming);
            AimStart?.Invoke();
        }

        public void OnAimEnd()
        {
            StateEnd();
            AimEnd?.Invoke();
        }

        public void OnCustsceneStart()
        {
            if (_currentState == PlayerState.Dead) { return; }
            StateStart(PlayerState.OnCutscene);
            CutsceneStart?.Invoke();
        }

        public void OnCustsceneEnd()
        {
            StateEnd();
            CutsceneEnd?.Invoke();
        }

        public void OnAnimationStart()
        {
            if (_currentState == PlayerState.Dead) { return; }
            StateStart(PlayerState.OnAnimation);
            AnimationStart?.Invoke();
        }

        public void OnAnimationEnd()
        {
            StateEnd();
            AnimationEnd?.Invoke();
        }

        public void OnPlayerDeath()
        {
            if (_currentState == PlayerState.Aiming) { OnAimEnd(); }
            StateStart(PlayerState.Dead);
            PlayerDied?.Invoke();
        }
    }
}