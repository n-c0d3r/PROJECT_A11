using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;



namespace PROJECT_A11.Develops.Common
{
    public class PersonBrain :
        PawnBrain<Person, PersonController>
    {

        public virtual void OnStartGrounded()
        {



        }
        public virtual void OnEndGrounded()
        {



        }

        public virtual void OnStartGroundedMoving(Vector2 input)
        {



        }
        public virtual void OnStopGroundedMoving()
        {



        }
        public virtual void OnStartOrdinaryBody()
        {



        }
        public virtual void OnStopOrdinaryBody()
        {



        }
        public virtual void OnStartCrouching()
        {



        }
        public virtual void OnStopCrouching()
        {



        }

        public virtual void OnStartStrafing(Vector2 input)
        {



        }
        public virtual void OnStrafing(Vector2 input)
        {



        }
        public virtual void OnStopStrafing()
        {



        }
        public virtual void OnStartJumping()
        {



        }

        public virtual void OnLooking(Vector2 input)
        {



        }



        protected override void Awake()
        {

            base.Awake();

        }

        private void Update()
        {



        }

    }

}