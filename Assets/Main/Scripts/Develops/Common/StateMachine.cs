using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Assertions;



namespace PROJECT_A11.Develops.Common
{

    /// <summary>
    /// A state machine is a behavior model.
    /// It consists of finite number of states and performs state transitions and state actions. 
    /// </summary>
    public class StateMachine<StateMachineType> : 
        MonoBehaviour
        where StateMachineType : StateMachine<StateMachineType>
    {
        [SerializeField]
        private State<StateMachineType> m_StartState;
        public State<StateMachineType> startState { set { m_StartState = value; }  get { return m_StartState; } }


        private State<StateMachineType> m_CurrentState;
        public State<StateMachineType> currentState { get { return m_CurrentState; } }



        /// <summary>
        /// Regularly checks if the state machine need to move to another state.
        /// </summary>
        private void CheckPotentialTransition()
        {

            if (currentState == null) return;

            var state = m_CurrentState.CheckPotentialTransition();

            if (state != m_CurrentState)
            {

                MoveToState(state);

            }

        }

        /// <summary>
        /// Moves to the indicated state.
        /// </summary>
        public void MoveToState(State<StateMachineType> state)
        {

            if (m_CurrentState != null)
            {

                m_CurrentState.StopPerforming();

            }

            m_CurrentState = state;

            m_CurrentState.StartPerforming();

        }



        protected virtual void Awake()
        {

            foreach (var component in GetComponents<State<StateMachineType>>())
            {

                component.enabled = false;
                component.machine = (StateMachineType)this;

            }
            foreach (var component in GetComponents<StateAction<StateMachineType>>())
            {

                component.enabled = false;
                component.machine = (StateMachineType)this;

            }
            foreach (var component in GetComponents<StateTransition<StateMachineType>>())
            {

                component.machine = (StateMachineType)this;

            }
            
            m_CurrentState = m_StartState;

            if (currentState != null)
                m_CurrentState.StartPerforming();

        }

        protected virtual void Update()
        {

            CheckPotentialTransition();

        }

    }

}