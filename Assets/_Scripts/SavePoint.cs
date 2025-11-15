using UnityEngine;

public class SavePoint : MonoBehaviour
{

    [SerializeField] private Vector3 respawnOffset;
    [SerializeField] private bool autosave = true;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            GameController.Instance.SetLastSavePoint(this);

            if (autosave)
            {
                GameController.Instance.SaveGame();
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(transform.position + respawnOffset, 0.1f);
    }

    public Vector3 GetRespawnPosition()
    {
        return transform.position + respawnOffset;
    }
}
