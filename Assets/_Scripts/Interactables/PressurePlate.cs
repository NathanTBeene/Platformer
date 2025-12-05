using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class PressurePlate : MonoBehaviour
{
    [SerializeField] private InputNode inputNode;
    [SerializeField] private SpriteRenderer plateSprite;
    [SerializeField] private Sprite plateUpSprite;
    [SerializeField] private Sprite plateDownSprite;

    private List<string> collisionTags = new List<string>();

    void Start()
    {
        if (!inputNode)
        {
            inputNode = GetComponent<InputNode>();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") || other.CompareTag("physicsObject"))
        {
            _plateDown();
            inputNode.setState(true);
            collisionTags.Add(other.tag);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (collisionTags.Contains(other.tag))
        {
            collisionTags.Remove(other.tag);
        }

        if (collisionTags.Count == 0)
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
