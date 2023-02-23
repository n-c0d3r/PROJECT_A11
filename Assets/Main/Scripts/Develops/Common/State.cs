using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;



namespace PROJECT_A11.Develops.Common
{

    /// <summary>
    /// A state is a situation of a state machine depending on previous inputs, and states,...
    /// Notes:
    ///     + All state gameobject components must be disabled by default.
    /// </summary>
    public class State<StateMachineType> :
        MonoBehaviour
        where StateMachineType : StateMachine<StateMachineType>
    {

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        #region Fields
        [HideInInspector]
        public List<StateTransition<StateMachineType>
            > transitions = new List<StateTransition<StateMachineType>>();

        public List<StateAction<StateMachineType>> actions = new List<StateAction<StateMachineType>>();

        [HideInInspector]
        public StateMachineType machine;
        #endregion

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        #region Methods
        /// <summary>
        /// Checks if there is a transition need to perform.
        /// </summary>
        public State<StateMachineType> CheckPotentialTransition()
        {

            foreach (var transition in transitions)
            {

                State<StateMachineType> result = transition.Check();
                if(result != this)
                {

                    return result;
                }

            }

            return this;
        }

        /// <summary>
        /// Enables this state to work inside a machine.
        /// </summary>
        public void StartPerforming()
        {

            enabled = true;

        }
        /// <summary>
        /// Disables this state.
        /// </summary>
        public void StopPerforming()
        {

            enabled = false;

        }
        #endregion

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        #region MonoBehavior Events
        protected virtual void OnEnable()
        {

            foreach (var action in actions)
            {

                action.enabled = true;

            }

        }
        protected virtual void OnDisable()
        {

            foreach (var action in actions)
            {

                action.enabled = false;

            }

        }

        protected virtual void Update()
        {
            
        }
        #endregion

    }

}