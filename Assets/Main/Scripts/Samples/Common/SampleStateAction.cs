using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;



namespace PROJECT_A11.Samples.Common
{

    /// <summary>
    /// A basic sample state action.
    /// </summary>
    public class SampleStateAction :
        Develops.Common.StateAction<SampleStateMachine>
    {

        protected override void OnEnable()
        {

            Debug.Log("OnEnable");

        }
        protected override void OnDisable()
        {

            Debug.Log("OnDisable");

        }

    }

}