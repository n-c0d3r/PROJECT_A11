using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;



namespace PROJECT_A11.Develops.Common
{

    public class PawnFSM<PawnType, PawnFSMType> :
        StateMachine<PawnFSMType>
        where PawnFSMType : StateMachine<PawnFSMType>
    {

        private PawnType m_Pawn;
        public PawnType pawn { get { return m_Pawn; } }



        protected override void Awake()
        {

            base.Awake();

            m_Pawn = GetComponent<PawnType>();

        }

        protected override void Update()
        {

            base.Update();

        }

    }

}