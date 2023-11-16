using UnityEngine;

public class SlidingRigidbodyMoveHandler : MoveHandler
{
    private const float VelocityCoefficient = 180f; 

    private void Start()
    {
        characterRigidbody2D.mass = 1f;
        characterRigidbody2D.drag = 1.5f;
    }
    
    public override void Move(Vector2 velocityVector)
    {
        characterRigidbody2D.AddForce(velocityVector * VelocityCoefficient, ForceMode2D.Force);
    }
}