using UnityEngine;
using System;

public class InputManager : MonoBehaviour
{
    public static event Action<float> onMove;
    public static event Action onJump;
    public static event Action onInteract;
    // public static event Action onPause;


    void Update()
    {
        float directionX = Input.GetAxisRaw("Horizontal");

        onMove?.Invoke(directionX);

        if (Input.GetButtonDown("Jump"))
        {
            onJump?.Invoke();
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            Debug.Log("Interact button pressed.");
            onInteract?.Invoke();
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (GameController.Instance)
            {
                GameController.Instance.TogglePauseGame();
            }
        }
    }
}
