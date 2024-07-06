using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeetleIdleBehavior : StateMachineBehaviour
{
    [SerializeField]
    float timeToBreakIdle;

    [SerializeField]
    float numberOfBreaks;
    bool isIdle;
    float idleTime;
    int breakAnimation;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(
        Animator animator,
        AnimatorStateInfo stateInfo,
        int layerIndex
    )
    {
        ResetIdle();
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(
        Animator animator,
        AnimatorStateInfo stateInfo,
        int layerIndex
    )
    {
        if (isIdle)
        {
            idleTime += Time.deltaTime;
            if (idleTime > timeToBreakIdle && stateInfo.normalizedTime % 1 < 0.02f)
            {
                isIdle = false;
                breakAnimation = (int)Random.Range(1, numberOfBreaks + 1);
                breakAnimation = breakAnimation * 2 - 1;

                animator.SetFloat("IdleAnimation", breakAnimation - 1);
            }
        }
        else if (stateInfo.normalizedTime % 1 > 0.98)
        {
            ResetIdle();
        }
        animator.SetFloat("IdleAnimation", breakAnimation, 0.2f, Time.deltaTime);
    }

    private void ResetIdle()
    {
        if (!isIdle)
        {
            breakAnimation--;
        }
        isIdle = true;
        idleTime = 0;
    }
}
