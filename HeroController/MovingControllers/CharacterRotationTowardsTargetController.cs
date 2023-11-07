using System.Collections.Generic;
using UnityEngine;

public class CharacterRotationTowardsTargetController : BaseController
{
    private readonly ISubscriptionReactiveProperty<bool> _isCharacterFlipped;
    private readonly List <CharacterRotationTowardsTargetHandler> _rotationHandlersList;

    protected CharacterRotationTowardsTargetController(ISubscriptionReactiveProperty<bool> isCharacterFlipped, List <CharacterRotationTowardsTargetHandler> rotationHandlersList)
    {
        _isCharacterFlipped = isCharacterFlipped;
        _rotationHandlersList = rotationHandlersList;
        
        foreach (var rotationHandler in _rotationHandlersList)
            rotationHandler.SetStartValues();
        _isCharacterFlipped.SubscribeToChange(UpdateRotationLimitsOnFlip);
    }

    private void UpdateRotationLimitsOnFlip(bool isCharacterFlipped)
    {
        foreach (var rotationHandler in _rotationHandlersList)
            rotationHandler.UpdateLimitsOnFlip(isCharacterFlipped);
    }

    protected void ApplyRotation(Quaternion rotation)
    {
        foreach (var rotationHandler in _rotationHandlersList)
            rotationHandler.Rotate(rotation);
    }

    protected override void OnDispose()
    {
        _isCharacterFlipped.UnsubscribeFromChange(UpdateRotationLimitsOnFlip);
        base.OnDispose();
    }
}