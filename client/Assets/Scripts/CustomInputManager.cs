using System;
using System.Collections;
using System.Collections.Generic;
using MoreMountains.TopDownEngine;
using UnityEngine;
using UnityEngine.Events;

public class CustomInputManager : MonoBehaviour
{
    [SerializeField] GameObject JoystickL;
    [SerializeField] GameObject mainAttack;
    [SerializeField] GameObject specialAttack;
    [SerializeField] GameObject dash;
    [SerializeField] GameObject ultimate;
    public void AssignInputToAbility(string trigger, string triggerType, UnityEvent<Vector2> ability)
    {
        specialAttack.GetComponent<CustomMMTouchJoystick>().newPointerEvent = ability;
    }
}
