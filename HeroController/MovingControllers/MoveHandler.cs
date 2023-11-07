using UnityEngine;

public class MoveHandler : MonoBehaviour
{
    protected Rigidbody2D characterRigidbody2D;

    private void Awake()
    {
        characterRigidbody2D = GetComponent<Rigidbody2D>();
    }
    public virtual void Move(Vector2 velocityVector) { }

    public void BlockMoving() => characterRigidbody2D.bodyType = RigidbodyType2D.Static;
}