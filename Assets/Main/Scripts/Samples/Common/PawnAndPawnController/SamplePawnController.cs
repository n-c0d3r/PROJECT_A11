using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;



namespace PROJECT_A11.Samples.Common
{

    /// <summary>
    /// A controller for sample pawn.
    /// </summary>
    public class SamplePawnController :
        Develops.Common.PawnController<SamplePawn>
    {

        public KeyCode moveKey;



        private void Update()
        {

            if (Input.GetKey(moveKey))
            {

                pawn.velocity = new Vector3(1.0f , 0.0f, 0.0f);

            }
            else
            {

                pawn.velocity = new Vector3(0.0f, 0.0f, 0.0f);

            }

        }

    }

}