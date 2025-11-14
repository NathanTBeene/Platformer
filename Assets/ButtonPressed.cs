using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class ButtonPressed : StateMachineBehaviour
{
    private Vector3 originalPosition;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        GameObject gameObject = animator.gameObject;
        originalPosition = gameObject.transform.position;

        // Translate the button downwards to simulate a press
        gameObject.transform.DOMoveY(originalPosition.y - 4f, 0.2f).SetEase(Ease.OutQuad);
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        GameObject gameObject = animator.gameObject;

        // Translate the button back to its original position
        gameObject.transform.DOMoveY(originalPosition.y, 0.2f).SetEase(Ease.OutQuad);

    }

    // OnStateMove is called right after Animator.OnAnimatorMove()
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that processes and affects root motion
    //}

    // OnStateIK is called right after Animator.OnAnimatorIK()
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that sets up animation IK (inverse kinematics)
    //}
}
