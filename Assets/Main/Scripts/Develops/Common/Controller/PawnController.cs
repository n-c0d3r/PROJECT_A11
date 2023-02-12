using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;



namespace PROJECT_A11.Develops.Common
{

    public class PawnController<PawnType> :
        MonoBehaviour
    {

        private PawnType m_Pawn;
        public PawnType pawn { get { return m_Pawn; } }



        protected virtual void Awake()
        {

            m_Pawn = GetComponent<PawnType>();

        }

        private void Update()
        {
            


        }

    }

}