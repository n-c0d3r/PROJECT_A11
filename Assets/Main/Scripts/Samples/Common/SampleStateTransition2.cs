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
    public class SampleStateTransition2 :
        Develops.Common.StateTransition<SampleStateMachine>
    {

        public override Develops.Common.State<SampleStateMachine> Check()
        {

            if (((SampleState)fromState).a == 4)
            {

                return toState;

            }

            return fromState;
        }

    }

}