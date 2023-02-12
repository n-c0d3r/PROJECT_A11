using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;



namespace PROJECT_A11.Develops.Common
{

    /// <summary>
    /// The 'Singleton' implementation
    /// </summary>
    public class Singleton<T> :
        MonoBehaviour
        where T : class
    {

        private static T s_Instance;
        public static T instance { get { return s_Instance; } }



        /// <summary>
        /// Automatically sets the s_Instance value to the owner game object.
        /// If s_Instance is not currently null, assertion will fail. 
        /// </summary>
        protected virtual void Awake()
        {

            Assert.IsNull(s_Instance);

            s_Instance = (T)Convert.ChangeType(this, typeof(T));

        }

    }

}