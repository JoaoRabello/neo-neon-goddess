using System;
using UnityEngine;

namespace Player
{
    /// <summary>
    /// Observer that watches the player states.
    /// Used to control whether player is free to action or not
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
        /// <summary>
        /// Player States enumerator.
        /// <value> <c>• Free</c>: represents when player can action freely.<br></br></value>
        /// <value> <c>• OnAnimation</c>: represents when player is on an animation (like ledge grab or surprise attacks).<br></br></value>
        /// <value> <c>• OnCutscene</c>: represents when player is being controlled by a cutscene.<br></br></value>
        /// <value> <c>• OnDialogue</c>: represents when player is on dialogues or barks due to interactions.<br></br></value>
        /// </summary>
        public enum PlayerState
        {
            Free,
            OnAnimation,
            OnCutscene,
            OnDialogue
        }

        private PlayerState _currentState;
        public PlayerState CurrentState => _currentState;

        private Action DialogueStart;
        private Action DialogueEnd;

        private Action CutsceneStart;
        private Action CutsceneEnd;

        private Action AnimationStart;
        private Action AnimationEnd;

        private bool _isDuringState;
        
        //TODO: See if it's possible to build a state queue for situations like "Start cutscene and then start" + "a dialogue when ending cutscene" 

        /// <summary>
        /// Configure every event to their respective observed objects
        /// </summary>
        private void SetupEvents()
        {
            
        }
        
        private void StateStart(PlayerState state)
        {
            _isDuringState = true;

            _currentState = state;
        }

        private void StateEnd()
        {
            _isDuringState = false;
            
            _currentState = PlayerState.Free;
        }

        private void OnDialogueStart()
        {
            StateStart(PlayerState.OnDialogue);
        }

        private void OnDialogueEnd()
        {
            StateEnd();
        }

        private void OnCustsceneStart()
        {
            StateStart(PlayerState.OnCutscene);
        }

        private void OnCustsceneEnd()
        {
            StateEnd();
        }

        private void OnAnimationStart()
        {
            StateStart(PlayerState.OnAnimation);
        }

        private void OnAnimationEnd()
        {
            StateEnd();
        }
    }
}