using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;



namespace PROJECT_A11.Develops.Common
{

    /// <summary>
    /// A state action is an action that a state can perform.
    /// Notes:
    ///     + All state action gameobject components must be disabled by default.
    /// </summary>
    public class StateAction<StateMachineType> :
        MonoBehaviour
        where StateMachineType : StateMachine<StateMachineType>
    {

        [HideInInspector]
        public StateMachineType machine;



        protected virtual void OnEnable()
        {



        }
        protected virtual void OnDisable()
        {



        }

        protected void Update()
        {

        }

    }

}