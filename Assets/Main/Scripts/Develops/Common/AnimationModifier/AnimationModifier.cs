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

        public bool usePreviousOutputAsInput = false;

        public virtual void Apply(AnimationClip inputClip, AnimationClip outputClip)
        {

            Assert.IsNotNull(inputClip);
            Assert.IsNotNull(outputClip);

        }

    }

}

#endif