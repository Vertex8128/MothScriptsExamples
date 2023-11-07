using UnityEngine;

public sealed class HeroRotationTowardsTargetController : CharacterRotationTowardsTargetController
{
    private readonly InputData _inputData;

    public HeroRotationTowardsTargetController(ActiveHeroData heroData)
    : base(heroData.IsHeroFlipped, heroData.HeroObjectDataKeeper.RotationHandlersList)
    {
        _inputData = GameData.Instance.Input;
        _inputData.LookAtDirection.SubscribeToChange(RotateOnInput);
    }

    private void RotateOnInput(Vector2 lookAtDirection)
    {
        var rotation = Quaternion.LookRotation(Vector3.forward, lookAtDirection);
        ApplyRotation(rotation);
    }

    protected override void OnDispose()
    {
        _inputData.LookAtDirection.UnsubscribeFromChange(RotateOnInput);
        base.OnDispose();
    }
}
