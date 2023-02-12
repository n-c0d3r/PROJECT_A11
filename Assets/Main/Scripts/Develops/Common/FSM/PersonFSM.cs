using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.InputSystem;



namespace PROJECT_A11.Develops.Common
{

    [RequireComponent(typeof(PersonStartMovingState))]
    [RequireComponent(typeof(PersonMovingState))]
    [RequireComponent(typeof(PersonStartJumpingState))]
    [RequireComponent(typeof(PersonIdleState))] 
    public class PersonFSM :
        Develops.Common.PawnFSM<Person, PersonFSM>
    {

        private PersonController m_Controller;



        protected override void Awake()
        {

            startState = GetComponent<PersonIdleState>();

            base.Awake();

            m_Controller = GetComponent<PersonController>();

        }

    }

}