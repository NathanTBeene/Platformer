using DG.Tweening;
using UnityEngine;

public class PressurePlate : MonoBehaviour
{
    [SerializeField] private InputNode inputNode;
    [SerializeField] private SpriteRenderer plateSprite;
    [SerializeField] private Sprite plateUpSprite;
    [SerializeField] private Sprite plateDownSprite;


    void Start()
    {
        if (!inputNode)
        {
            inputNode = GetComponent<InputNode>();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            _plateDown();
            inputNode.setState(true);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            inputNode.setState(false);
            _plateUp();
        }
    }

    private void _plateDown()
    {
        plateSprite.sprite = plateDownSprite;
    }

    private void _plateUp()
    {
        plateSprite.sprite = plateUpSprite;
    }
}
