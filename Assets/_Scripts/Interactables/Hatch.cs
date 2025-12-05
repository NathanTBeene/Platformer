using UnityEngine;

public class Hatch : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject leftDoor;
    [SerializeField] private GameObject rightDoor;
    [SerializeField] private OutputNode outputNode;

    private void OnEnable() {
        if (outputNode != null)
        {
            outputNode.onStateChange += SetState;
        }
    }

    private void OnDisable() {
        if (outputNode != null)
        {
            outputNode.onStateChange -= SetState;
        }
    }

    private void Start() {
        // Set initial state based on output node
        SetState(outputNode.getState());
    }

    private void SetState(bool state)
    {
        if (state)
            Open();
        else
            Close();
    }

    public void Open()
    {
        // Animate doors opening
        Animator leftAnim = leftDoor.GetComponent<Animator>();
        Animator rightAnim = rightDoor.GetComponent<Animator>();

        leftAnim.SetBool("isOpen", true);
        rightAnim.SetBool("isOpen", true);
    }

    public void Close()
    {
        // Animate doors closing
        Animator leftAnim = leftDoor.GetComponent<Animator>();
        Animator rightAnim = rightDoor.GetComponent<Animator>();

        leftAnim.SetBool("isOpen", false);
        rightAnim.SetBool("isOpen", false);
    }
}
