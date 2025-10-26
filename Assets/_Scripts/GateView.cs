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

    async void HandleSignalOn(SignalNode node)
    {
        if (node == logicGate)
        {
            foreach (var wireManager in wireManagers)
            {
                if (wireManager == null) continue;
                await wireManager.PowerOn(0.2f);
            }
        }
    }
    async void HandleSignalOff(SignalNode node)
    {
        if (node == logicGate)
        {
            foreach (var wireManager in wireManagers)
            {
                if (wireManager == null) continue;
                await wireManager.PowerOff(0.2f);
            }
        }
    }
}
