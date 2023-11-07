using UnityEngine;

public class SlowRigidbodyMoveHandler : MoveHandler
{
    private const float SlowingCoefficient = 0.5f;

    private void Start()
    {
        characterRigidbody2D.mass = 1f;
        characterRigidbody2D.drag = 0f;
        characterRigidbody2D.velocity = Vector2.zero;
    }
    
    public override void Move(Vector2 velocityVector)
    {
        characterRigidbody2D.MovePosition(characterRigidbody2D.position + velocityVector * SlowingCoefficient);
    }
}