using System.Collections;
using DG.Tweening;
using UnityEngine;


public class PanelPopIn : MonoBehaviour
{
    // Panel pop-in and pop-out animation using DoTween
    // Should be attached directly to the panel GameObject

    [SerializeField] private bool startHidden = true;
    [SerializeField] private float startingScale = 0f;

    [Header("Animation Settings")]
    [SerializeField] private float popInDuration = 0.5f;
    [SerializeField] private float popInOvershoot = 1.2f;
    [SerializeField] private float popOutDuration = 0.5f;
    [SerializeField] private float popOutUndershoot = 0.8f;

    [SerializeField] private Ease popInEase = Ease.OutBack;
    [SerializeField] private Ease popOutEase = Ease.InBack;

    private void Start()
    {
        startingScale = transform.localScale.x;
        if (startHidden)
        {
            transform.localScale = Vector3.zero;
        }

    }

    // Popin uses DoTween to animate the panel popping in and out
    public IEnumerator PopIn()
    {
        // Implement DoTween pop-in animation here
        transform.DOScale(startingScale, popInDuration).From(Vector3.zero).SetEase(popInEase, popInOvershoot);
        yield return new WaitForSeconds(popInDuration);
    }

    public IEnumerator PopOut()
    {
        // Implement DoTween pop-out animation here
        transform.DOScale(Vector3.zero, popOutDuration).SetEase(popOutEase, popOutUndershoot);
        yield return new WaitForSeconds(popOutDuration);
    }

    [ContextMenu("Test PopIn")]
    private void TestPopIn()
    {
        StartCoroutine(PopIn());
    }

    [ContextMenu("Test PopOut")]
    private void TestPopOut()
    {
        StartCoroutine(PopOut());
    }
}
