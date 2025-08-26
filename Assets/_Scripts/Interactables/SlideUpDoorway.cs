using DG.Tweening;
using UnityEngine;

public class SlideUpDoorway : MonoBehaviour
{
    [SerializeField] private GameObject doorVisual;
    [SerializeField] private OutputNode outputNode;
    [SerializeField] private float slideDistance = 5f;
    [SerializeField] private float slideDuration = 1f;

    private Vector3 _initialPosition;
    private Vector3 _targetPosition;

    void Start()
    {
        if (!doorVisual)
        {
            doorVisual = transform.GetChild(0).gameObject;
        }

        if (!outputNode)
        {
            outputNode = GetComponent<OutputNode>();
        }

        OutputNode.onOutputOn += _onOutputOn;
        OutputNode.onOutputOff += _onOutputOff;

        _initialPosition = doorVisual.transform.localPosition;
        _targetPosition = _initialPosition + new Vector3(0, slideDistance, 0);
    }

    private void _onOutputOn(OutputNode sourceNode)
    {
        if (sourceNode.gameObject == gameObject)
        {
            _doorUp();
        }
    }

    private void _onOutputOff(OutputNode sourceNode)
    {
        if (sourceNode.gameObject == gameObject)
        {
            _doorDown();
        }
    }

    private void _doorUp()
    {
        doorVisual.transform.DOLocalMoveY(_targetPosition.y, slideDuration);
    }

    private void _doorDown()
    {
        doorVisual.transform.DOLocalMoveY(_initialPosition.y, slideDuration);
    }

}