using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;



namespace PROJECT_A11.Samples.Common
{

    /// <summary>
    /// A basic sample state transition.
    /// </summary>
    public class SampleStateTransition :
        Develops.Common.StateTransition<SampleStateMachine>
    {

        public override Develops.Common.State<SampleStateMachine> Check()
        {

            if (((SampleState)fromState).a == 2)
            {

                return toState;

            }

            return fromState;
        }

    }

}