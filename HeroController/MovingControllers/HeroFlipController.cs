public sealed class HeroFlipController : CharacterFlipController
{
    private readonly InputData _inputData;

    private const float MouseFlipThreshold = 0.3f;
    private const float ControllerFlipThreshold = 0f;

    public HeroFlipController(ActiveHeroData heroData)
        : base(heroData.IsHeroFlipped)
    {
        _inputData = GameData.Instance.Input;
        
        SetFlipThresholdOnInputTypeChange(_inputData.InputType.Value);

        _inputData.InputType.SubscribeToChange(SetFlipThresholdOnInputTypeChange);
        _inputData.LookAtDirection.SubscribeToChange(SetCharacterFlip);
    }
    
    private void SetFlipThresholdOnInputTypeChange(InputTypeID inputTypeID)
    {
        switch (inputTypeID)
        {
            case InputTypeID.KeyboardMouse:
                flipThreshold = MouseFlipThreshold;
                break;

            case InputTypeID.Xbox:
                flipThreshold = ControllerFlipThreshold;
                break;
        }
    }

    protected override void OnDispose()
    {
        _inputData.InputType.UnsubscribeFromChange(SetFlipThresholdOnInputTypeChange);
        _inputData.LookAtDirection.UnsubscribeFromChange(SetCharacterFlip);
        base.OnDispose();
    }
}