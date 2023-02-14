using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.InputSystem;



namespace PROJECT_A11.Develops.Common
{

    public class PersonFSM :
        Develops.Common.PawnFSM<Person, PersonFSM>
    {

        private PersonController m_Controller;



        protected override void Awake()
        {

            base.Awake();

            m_Controller = GetComponent<PersonController>();

        }

    }

}