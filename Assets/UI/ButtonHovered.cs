using DG.Tweening;
using UnityEngine;

public class ButtonHovered : StateMachineBehaviour
{

    private Vector2 originalPosition;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        GameObject gameObject = animator.gameObject;

        originalPosition = gameObject.transform.position;

        // Scale up the button to simulate a hover effect
        gameObject.transform.DOMoveY(originalPosition.y + 2f, 0.2f).SetEase(Ease.OutQuad);
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        GameObject gameObject = animator.gameObject;


        // Scale down the button to simulate the end of a hover effect
        gameObject.transform.DOMoveY(originalPosition.y, 0.2f).SetEase(Ease.OutQuad);
    }
}
