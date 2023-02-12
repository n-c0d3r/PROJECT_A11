using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.InputSystem;

namespace PROJECT_A11.Researches.InputSystem
{

    public class InputSystemTester :
        MonoBehaviour
    {

        public void Fire(InputAction.CallbackContext ctx)
        {

            Debug.Log("Fire");

        }
        public void Move(InputAction.CallbackContext ctx)
        {

            Debug.Log(ctx.ReadValue<Vector2>());

        }

    }

}