using UnityEngine;

public class PlayerAnimationEvents : MonoBehaviour
{

    [Header("References")]
    [SerializeField] private PlayerMovement PlayerMovement;

    private void AttackStarted()
    {
        Debug.Log("Attack started");
        PlayerMovement.setCanMove(false);
        PlayerMovement.setCanJump(false);
    }

    private void AttackEnded()
    {
        Debug.Log("Attack ended");
        PlayerMovement.setCanMove(true);
        PlayerMovement.setCanJump(true);
    }
}
