using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Assertions;



namespace PROJECT_A11.Develops.Common
{

    /// <summary>
    /// </summary>
    public class StateTransition<StateMachineType> :
        MonoBehaviour
        where StateMachineType : StateMachine<StateMachineType>
    {

        public State<StateMachineType> fromState;
        public State<StateMachineType> toState;

        [HideInInspector]
        public StateMachineType machine;



        public virtual State<StateMachineType> Check()
        {

            return fromState;
        }

    }

}