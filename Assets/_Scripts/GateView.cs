using System.Collections;
using UnityEngine;

public class GateView : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private LogicGate logicGate;
    [SerializeField] private WireManager[] wireManagers;

    [Header("Gate Sprites")]
    [SerializeField] private GameObject input1Sprite;
    [SerializeField] private GameObject input2Sprite;
    [SerializeField] private GameObject outputSprite;

    [SerializeField] private Sprite TrueSprite;
    [SerializeField] private Sprite FalseSprite;

    [Header("Settings")]
    [SerializeField] private bool isSingleInput = false;
    [SerializeField] private float wireFillDuration = 0.5f;


    void OnEnable()
    {
        logicGate.signalOn += HandleSignalOn;
        logicGate.signalOff += HandleSignalOff;
    }
    void OnDisable()
    {
        logicGate.signalOn -= HandleSignalOn;
        logicGate.signalOff -= HandleSignalOff;
    }

    void Update()
    {
        if (isSingleInput)
        {
            UpdateSingle();
        }
        else
        {
            UpdateMulti();
        }
    }

    // For gates with multiple inputs like AND, OR
    void UpdateMulti()
    {
        if (input1Sprite != null)
            input1Sprite.SetActive(logicGate.input1);
        if (input2Sprite != null)
            input2Sprite.SetActive(logicGate.input2);
        if (outputSprite != null)
            outputSprite.SetActive(logicGate.output);
    }

    // For single input gates like NOT
    void UpdateSingle()
    {
        if (outputSprite != null)
        {
            bool inputState = logicGate.output;
            // Change output sprite based on state
            var spriteRenderer = outputSprite.GetComponent<SpriteRenderer>();
            if (spriteRenderer != null)
            {
                spriteRenderer.sprite = inputState ? TrueSprite : FalseSprite;
            }
        }
    }

    void HandleSignalOn(SignalNode node)
    {
      if (node == logicGate)
      {
        StartCoroutine(PowerOnWires());
      }
    }

    void HandleSignalOff(SignalNode node)
    {
      if (node == logicGate)
      {
        StartCoroutine(PowerOffWires());
      }
    }

    private IEnumerator PowerOnWires()
    {
        Coroutine[] wireCoroutines = new Coroutine[wireManagers.Length];

        for (int i = 0; i < wireManagers.Length; i++)
        {
          var wire = wireManagers[i];
          if (wire != null)
          {
            wireCoroutines[i] = StartCoroutine(wire.PowerOn(wireFillDuration));
          }
        }

        // Wait for all wire coroutines to complete
        foreach (var coroutine in wireCoroutines)
        {
            if (coroutine != null)
            {
                yield return coroutine;
            }
        }
    }

    private IEnumerator PowerOffWires()
    {
        Coroutine[] wireCoroutines = new Coroutine[wireManagers.Length];

        for (int i = 0; i < wireManagers.Length; i++)
        {
          var wire = wireManagers[i];
          if (wire != null)
          {
            wireCoroutines[i] = StartCoroutine(wire.PowerOff(wireFillDuration * 0.5f));
          }
        }

        // Wait for all wire coroutines to complete
        foreach (var coroutine in wireCoroutines)
        {
            if (coroutine != null)
            {
                yield return coroutine;
            }
        }
    }
}
