using UnityEngine;

public class SlidingRigidbodyMoveHandler : MoveHandler
{
    private const float VelocityCoefficient = 180f; 
    private bool _isFirstInput;

    private void Start()
    {
        characterRigidbody2D.mass = 1f;
        characterRigidbody2D.drag = 1.5f;
    }
    
    public override void Move(Vector2 velocityVector)
    {
        if (!_isFirstInput)
        {
            characterRigidbody2D.velocity = velocityVector * (VelocityCoefficient * 0.2f);
            _isFirstInput = true;
            return;
        }
        characterRigidbody2D.AddForce(velocityVector * VelocityCoefficient, ForceMode2D.Force);
    }
}