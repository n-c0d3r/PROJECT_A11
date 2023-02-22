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
    public class Pawn<PawnType, PawnMovementControllerType, PawnFSMType, PawnBrainType, PawnAnimationControllerType> :
        MonoBehaviour
    {

        private PawnMovementControllerType m_Controller;
        public PawnMovementControllerType controller
        {
            get
            {

                if (m_Controller == null)
                    m_Controller = GetComponent<PawnMovementControllerType>();

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

        private PawnAnimationControllerType m_AnimationController;
        public PawnAnimationControllerType animationController
        {
            get
            {

                if (m_AnimationController == null)
                    m_AnimationController = GetComponent<PawnAnimationControllerType>();

                return m_AnimationController;
            }
        }



        protected virtual void Awake()
        {

            m_Controller = GetComponent<PawnMovementControllerType>();
            m_FSM = GetComponent<PawnFSMType>();
            m_Brain = GetComponent<PawnBrainType>();
            m_AnimationController = GetComponent<PawnAnimationControllerType>();

        }

    }



    /// <summary>
    /// A pawn is an object that is controllable by a controller.
    /// </summary>
    public class Pawn<PawnType, PawnMovementControllerType, PawnFSMType, PawnBrainType> :
        MonoBehaviour
    {

        private PawnMovementControllerType m_Controller;
        public PawnMovementControllerType controller
        {
            get
            {

                if (m_Controller == null)
                    m_Controller = GetComponent<PawnMovementControllerType>();

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

            m_Controller = GetComponent<PawnMovementControllerType>();
            m_FSM = GetComponent<PawnFSMType>();
            m_Brain = GetComponent<PawnBrainType>();

        }

    }



    /// <summary>
    /// A pawn is an object that is controllable by a controller.
    /// </summary>
    public class Pawn<PawnType, PawnMovementControllerType, PawnFSMType> :
        MonoBehaviour
    {

        private PawnMovementControllerType m_Controller;
        public PawnMovementControllerType controller { get { return m_Controller; } }

        private PawnFSMType m_FSM;
        public PawnFSMType FSM { get { return m_FSM; } }



        protected virtual void Awake()
        {

            m_Controller = GetComponent<PawnMovementControllerType>();
            m_FSM = GetComponent<PawnFSMType>();

        }

    }



    /// <summary>
    /// A pawn is an object that is controllable by a controller.
    /// </summary>
    public class Pawn<PawnType, PawnMovementControllerType> :
        MonoBehaviour
    {

        private PawnMovementControllerType m_PawnMovementController;
        public PawnMovementControllerType PawnMovementController { get { return m_PawnMovementController; } }



        protected virtual void Awake()
        {

            m_PawnMovementController = GetComponent<PawnMovementControllerType>();

        }

    }

}