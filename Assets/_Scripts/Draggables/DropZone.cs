using System;
using UnityEngine;

public class DropZone : MonoBehaviour
{

    public static Action<DropZone> OnItemEntered;
    public static Action<DropZone> OnItemExited;

    [SerializeField] private GameObject item;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        var draggable = collision.GetComponent<Draggable>();
        if (draggable != null)
        {
            OnItemEntered?.Invoke(this);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        var draggable = collision.GetComponent<Draggable>();
        if (draggable != null)
        {
            OnItemExited?.Invoke(this);
        }
    }
}
