using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.InputSystem;



namespace PROJECT_A11.Develops.Common
{

    public class IsPersonStartJumpingDone :
        Develops.Common.StateTransition<PersonFSM>
    {

        public override State<PersonFSM> Check()
        {

            base.Check();

            return fromState;
        }

    }

}