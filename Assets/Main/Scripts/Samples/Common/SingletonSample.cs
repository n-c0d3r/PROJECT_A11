using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Assertions;



namespace PROJECT_A11.Samples.Common
{

    /// <summary>
    /// The sample of PROJECT_A11.Common.Singleton<T>
    /// </summary>
    public class SingletonSample :
        Develops.Common.Singleton<SingletonSample>
    {

        protected override void Awake()
        {

            base.Awake();

            Debug.Log(SingletonSample.instance);

        }

    }


}