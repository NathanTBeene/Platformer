using System;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class Draggable : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{

    [SerializeField] private bool returnToStart = true;
    [SerializeField] private Vector3 startPosition;
    [SerializeField] private RectTransform rectTransform;

    [SerializeField] private float breakDistance = 5f; // Distance to break snapping to drop zones

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();

        DropZone.OnItemEntered += _onItemEntered;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        startPosition = rectTransform.anchoredPosition;
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            rectTransform.parent as RectTransform,
            GetMouseScreenPosition(),
            eventData.pressEventCamera,
            out localPoint
        );
        rectTransform.DOAnchorPos(localPoint, 0.1f);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (returnToStart)
        {
            rectTransform.DOAnchorPos(startPosition, 0.2f).SetEase(Ease.OutBack);
        }
    }

    private Vector3 GetMouseScreenPosition()
    {
        return Input.mousePosition;
    }
    
    private void _onItemEntered(DropZone dropZone)
    {
        
    }
}
