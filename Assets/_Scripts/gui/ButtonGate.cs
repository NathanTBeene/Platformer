using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ButtonGate : MonoBehaviour
{
  [Header("References")]
  [SerializeField] private InputNode inputNode;
  [SerializeField] private Sprite pressedSprite;
  [SerializeField] private Sprite unpressedSprite;
  [SerializeField] private Button button;
  [SerializeField] private WireManager wire;

  [Header("Settings")]
  [SerializeField] private bool isPressed = false;
  [SerializeField] private float wireFillDuration = 0.5f;

  private void OnEnable()
  {
    if (!inputNode)
    {
      inputNode = GetComponent<InputNode>();
    }

    button.onClick.AddListener(ToggleButton);
  }

  public void ToggleButton()
  {
    isPressed = !isPressed;
    if (isPressed)
    {
      StartCoroutine(PressButton());
    }
    else
    {
      StartCoroutine(ReleaseButton());
    }
  }

  public IEnumerator PressButton()
  {
    yield return _fill_wire(false);
    inputNode.setState(true);
  }

  public IEnumerator ReleaseButton()
  {
    yield return _fill_wire(true);
    inputNode.setState(false);
  }

  private IEnumerator _fill_wire(bool reverse = false)
  {
    if (wire)
    {
      if (reverse)
      {
        yield return wire.PowerOff(wireFillDuration * 0.5f);
      }
      else
      {
        yield return wire.PowerOn(wireFillDuration);
      }
    }
  }
}
