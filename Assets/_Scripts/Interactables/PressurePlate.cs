using DG.Tweening;
using UnityEngine;

public class PressurePlate : MonoBehaviour
{
    [SerializeField] private InputNode inputNode;
    [SerializeField] private GameObject plateVisual;
    [SerializeField] private Vector3 pressedOffset = new Vector3(0, -0.1f, 0);

    private Vector3 initialPosition;

    void Start()
    {
        initialPosition = plateVisual.transform.localPosition;
        if (!inputNode)
        {
            inputNode = GetComponent<InputNode>();
        }

        if (!plateVisual)
        {
            plateVisual = transform.GetChild(0).gameObject;
        }
    }

    // private void OnCollisionEnter(Collision other)
    // {
    //     Debug.Log("Collision Detected from: " + other.gameObject.name);
    //     if (other.gameObject.CompareTag("Player"))
    //     {
    //         Debug.Log("Player stepped on pressure plate");
    //         _plateDown();
    //         inputNode.setState(true);
    //     }
    // }

    // private void OnTriggerExit(Collider other)
    // {
    //     if (other.CompareTag("Player"))
    //     {
    //         Debug.Log("Player left pressure plate");
    //         inputNode.setState(false);
    //         _plateUp();
    //     }
    // }

    private void _plateDown()
    {
        plateVisual.transform.DOLocalMove(initialPosition + pressedOffset, 0.2f);
    }

    private void _plateUp()
    {
        plateVisual.transform.DOLocalMove(initialPosition, 0.2f);
    }
}
