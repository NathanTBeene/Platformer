using UnityEngine;

public class PowerIndicator : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private SpriteRenderer indicatorSprite;
    [SerializeField] private Sprite indicatorON;
    [SerializeField] private Sprite indicatorOFF;

    public void TurnOn()
    {
        if (indicatorSprite != null && indicatorON != null)
        {
            indicatorSprite.sprite = indicatorON;
        }
    }

    public void TurnOff()
    {
        if (indicatorSprite != null && indicatorOFF != null)
        {
            indicatorSprite.sprite = indicatorOFF;
        }
    }

    public void SetState(bool isOn)
    {
        if (isOn)
        {
            TurnOn();
        }
        else
        {
            TurnOff();
        }
    }
}
