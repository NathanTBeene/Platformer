using UnityEngine;

public class GateView : MonoBehaviour
{   
    [Header("References")]
    [SerializeField] private LogicGate logicGate;

    [Header("Gate Sprites")]
    [SerializeField] private GameObject input1Sprite;
    [SerializeField] private GameObject input2Sprite;
    [SerializeField] private GameObject outputSprite;

    [Header("Settings")]
    [SerializeField] private bool isSingleInput = false;

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
        input1Sprite.SetActive(logicGate.input1);
        input2Sprite.SetActive(logicGate.input2);
        outputSprite.SetActive(logicGate.output);
    }

    // For single input gates like NOT
    void UpdateSingle()
    {
        input1Sprite.SetActive(logicGate.input1);
        outputSprite.SetActive(logicGate.output);
    }
}
