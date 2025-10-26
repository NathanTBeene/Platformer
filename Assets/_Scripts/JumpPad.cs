using UnityEngine;

public class JumpPad : MonoBehaviour
{
    public enum TriggerType
    {
        Collision,
        Node
    }

    [Header("References")]
    [SerializeField] private OutputNode outputNode;
    [SerializeField] private SpriteRenderer jumpPadSprite;

    [Header("Sprites")]
    [SerializeField] private Sprite idleSprite;
    [SerializeField] private Sprite poweredSprite;

    [Header("Settings")]
    [SerializeField] private bool isPowered = false;
    [SerializeField] private float jumpForce = 15f;
    [SerializeField] private TriggerType triggerType = TriggerType.Collision;


    private void OnEnable()
    {
        if (!outputNode)
        {
            outputNode = GetComponent<OutputNode>();
        }

        if (outputNode)
        {
            outputNode.onStateChange += onStateChange;
        }
    }

    private void OnDisable()
    {
        if (outputNode != null)
        {
            outputNode.onStateChange -= onStateChange;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerMovement playerMovement = collision.GetComponent<PlayerMovement>();
            if (playerMovement != null)
            {
                if (triggerType == TriggerType.Collision)
                {
                    _powerOn();
                }

                if (isPowered)
                {
                    playerMovement.SetJumpForce(jumpForce);
                }
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerMovement playerMovement = collision.GetComponent<PlayerMovement>();
            if (playerMovement != null)
            {
                if (triggerType == TriggerType.Collision)
                {
                    _powerOff();
                }
                playerMovement.ResetJumpForce();
            }
        }
    }

    private void _getSprite()
    {
        if (isPowered)
        {
            jumpPadSprite.sprite = poweredSprite;
        }
        else
        {
            jumpPadSprite.sprite = idleSprite;
        }
    }

    private void _powerOn()
    {
        isPowered = true;
        _getSprite();
    }

    private void _powerOff()
    {
        isPowered = false;
        _getSprite();
    }

    private void onStateChange(bool state)
    {
        if (triggerType != TriggerType.Node) return;
        if (state)
        {
            _powerOn();
        }
        else
        {
            _powerOff();
        }
    }
}
