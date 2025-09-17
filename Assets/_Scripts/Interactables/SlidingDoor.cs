using UnityEngine;
using DG.Tweening;

public class SlidingDoor : MonoBehaviour
{
  enum Direction { Up, Down, Left, Right }

  [Header("References")]
  [SerializeField] private GameObject doorSprite;
  [SerializeField] private OutputNode outputNode;

  [Header("Settings")]
  [SerializeField] private Direction slideDirection = Direction.Up;

  // In units to slide. Essentially one grid tile position of 32 pixels.
  [SerializeField] private float slideDistance = 3f;
  [SerializeField] private float slideDuration = 1f;

  private Vector3 _initialPosition;
  private Vector3 _targetPosition;


  void Start()
  {
    if (!doorSprite)
    {
      Debug.LogWarning("No door sprite assigned, defaulting to first child object.");
      doorSprite = transform.GetChild(0).gameObject;
    }

    if (!outputNode)
    {
      Debug.LogWarning("No output node assigned, defaulting to OutputNode component on this object.");
      outputNode = GetComponent<OutputNode>();
    }

    OutputNode.onOutputOn += _onOutputOn;
    OutputNode.onOutputOff += _onOutputOff;

    _initialPosition = doorSprite.transform.localPosition;
    _targetPosition = _initialPosition + _getTargetOffset();
  }

  private void _onOutputOn(OutputNode sourceNode)
  {
    if (sourceNode.gameObject == gameObject)
    {
      OpenDoor();
    }
  }

  private void _onOutputOff(OutputNode sourceNode)
  {
    if (sourceNode.gameObject == gameObject)
    {
      CloseDoor();
    }
  }

  void OpenDoor()
  {
    doorSprite.transform.DOLocalMove(_targetPosition, slideDuration);
  }

  void CloseDoor()
  {
    doorSprite.transform.DOLocalMove(_initialPosition, slideDuration);
  }

  private Vector3 _getTargetOffset()
  {
    return slideDirection switch
    {
      Direction.Up => new Vector3(0, slideDistance, 0),
      Direction.Down => new Vector3(0, -slideDistance, 0),
      Direction.Left => new Vector3(-slideDistance, 0, 0),
      Direction.Right => new Vector3(slideDistance, 0, 0),
      _ => Vector3.zero,
    };
  }
}