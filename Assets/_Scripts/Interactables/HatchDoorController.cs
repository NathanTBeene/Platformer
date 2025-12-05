using System.Collections.Generic;
using UnityEngine;

public class HatchDoorController : MonoBehaviour
{
    public SpriteRenderer sprite;
    public PolygonCollider2D spriteCollider;
    private List<Vector2> PointsList = new List<Vector2>();


    private void Start() {
        sprite = GetComponent<SpriteRenderer>();
        spriteCollider = GetComponent<PolygonCollider2D>();
        RefreshCollider();
    }

    public void RefreshCollider()
    {
        PointsList.Clear();
        sprite.sprite.GetPhysicsShape(0, PointsList);
        spriteCollider.points = PointsList.ToArray();
    }
}
