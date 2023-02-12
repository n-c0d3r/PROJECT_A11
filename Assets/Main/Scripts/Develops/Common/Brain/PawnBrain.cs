using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;



namespace PROJECT_A11.Develops.Common
{

    /// <summary>
    /// A pawn brain is an object that decides what the pawn controller needs to do.
    /// </summary>
    public class PawnBrain<PawnType, PawnControllerType> :
        MonoBehaviour
    {

        private PawnType m_Pawn;
        public PawnType pawn { get { return m_Pawn; } }

        private PawnControllerType m_Controller;
        public PawnControllerType controller { get { return m_Controller; } }



        protected virtual void Awake()
        {

            m_Pawn = GetComponent<PawnType>();
            m_Controller = GetComponent<PawnControllerType>();

        }

        private void Update()
        {



        }

    }

}