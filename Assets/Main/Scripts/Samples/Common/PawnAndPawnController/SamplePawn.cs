using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;



namespace PROJECT_A11.Samples.Common
{

    /// <summary>
    /// A sample pawn.
    /// </summary>
    public class SamplePawn :
        Develops.Common.Pawn<SamplePawn, SamplePawnController>
    {

        [HideInInspector]
        public Vector3 velocity;



        private void Update()
        {

            transform.Translate(velocity * Time.deltaTime);

        }

    }

}