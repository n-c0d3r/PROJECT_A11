#if UNITY_EDITOR

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;



namespace PROJECT_A11.Develops.Common
{
    [System.Serializable]
    public class AnimationModifier :
        ScriptableObject
    {

        public virtual void Apply(AnimationClip clip)
        {

            Assert.IsNotNull(clip);

        }

    }

}

#endif