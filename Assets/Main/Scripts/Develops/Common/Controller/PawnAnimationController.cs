using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;



namespace PROJECT_A11.Develops.Common
{

    public class PawnAnimationController<PawnType, PawnControllerType> :
        MonoBehaviour
        where PawnType : MonoBehaviour
    {

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        #region Required Components
        private PawnType m_Pawn;
        public PawnType pawn
        {
            get
            {

                if (m_Pawn == null)
                    m_Pawn = GetComponent<PawnType>();

                return m_Pawn;
            }
        }

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
        #endregion

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        #region MonoBehavior Events
        protected virtual void Awake()
        {

            m_Pawn = GetComponent<PawnType>();
            m_Controller = GetComponent<PawnControllerType>();

        }

        protected virtual void Update()
        {



        }
        #endregion

    }

}