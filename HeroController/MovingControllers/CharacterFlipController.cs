using UnityEngine;

public class CharacterFlipController : BaseController
{
    private readonly ISubscriptionReactiveProperty<bool> _isCharacterFlipped;
    protected float flipThreshold = 0.1f;

    protected CharacterFlipController(ISubscriptionReactiveProperty<bool> isCharacterFlipped)
    {
        _isCharacterFlipped = isCharacterFlipped;
    }

    protected void SetCharacterFlip(Vector2 lookAtDirection)
    {
        if (lookAtDirection.x < -flipThreshold) // looking to the left
        {
            if (!_isCharacterFlipped.Value) return;
            _isCharacterFlipped.Value = false;
        }
        else if (lookAtDirection.x > flipThreshold) // looking to the right
        {
            if (_isCharacterFlipped.Value) return;
            _isCharacterFlipped.Value = true;
        }
    }
}