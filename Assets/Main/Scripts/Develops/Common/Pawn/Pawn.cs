using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;



namespace PROJECT_A11.Develops.Common
{

    /// <summary>
    /// A pawn is an object that is controllable by a controller.
    /// </summary>
    public class Pawn<PawnType, PawnControllerType, PawnFSMType, PawnBrainType> :
        MonoBehaviour
    {

        private PawnControllerType m_Controller;
        public PawnControllerType controller
        {
            get
            {

                if (m_Controller == null)
                    m_Controller = GetComponent<PawnControllerType>();

                return m_Controller;
            }
        }

        private PawnFSMType m_FSM;
        public PawnFSMType FSM
        {
            get
            {

                if (m_FSM == null)
                    m_FSM = GetComponent<PawnFSMType>();

                return m_FSM;
            }
        }

        private PawnBrainType m_Brain;
        public PawnBrainType brain
        {
            get
            {

                if (m_Brain == null)
                    m_Brain = GetComponent<PawnBrainType>();

                return m_Brain;
            }
        }



        protected virtual void Awake()
        {

            m_Controller = GetComponent<PawnControllerType>();
            m_FSM = GetComponent<PawnFSMType>();
            m_Brain = GetComponent<PawnBrainType>();

        }

    }



    /// <summary>
    /// A pawn is an object that is controllable by a controller.
    /// </summary>
    public class Pawn<PawnType, PawnControllerType, PawnFSMType> :
        MonoBehaviour
    {

        private PawnControllerType m_Controller;
        public PawnControllerType controller { get { return m_Controller; } }

        private PawnFSMType m_FSM;
        public PawnFSMType FSM { get { return m_FSM; } }



        protected virtual void Awake()
        {

            m_Controller = GetComponent<PawnControllerType>();
            m_FSM = GetComponent<PawnFSMType>();

        }

    }



    /// <summary>
    /// A pawn is an object that is controllable by a controller.
    /// </summary>
    public class Pawn<PawnType, PawnControllerType> :
        MonoBehaviour
    {

        private PawnControllerType m_PawnController;
        public PawnControllerType pawnController { get { return m_PawnController; } }



        protected virtual void Awake()
        {

            m_PawnController = GetComponent<PawnControllerType>();

        }

    }

}