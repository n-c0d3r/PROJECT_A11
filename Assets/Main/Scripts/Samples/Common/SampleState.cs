using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;



namespace PROJECT_A11.Samples.Common
{

    /// <summary>
    /// A basic sample state.
    /// </summary>
    public class SampleState :
        Develops.Common.State<SampleStateMachine>
    {

        public int a = 2;

    }

}