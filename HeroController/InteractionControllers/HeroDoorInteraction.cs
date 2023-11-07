public sealed class HeroDoorInteraction : BaseController
{
    private readonly InputData _inputData;

    public HeroDoorInteraction()
    {
        _inputData = GameData.Instance.Input;
        _inputData.InteractionButton.SubscribeToChange(ExitArenaOnInput);
    }

    private void ExitArenaOnInput(ButtonState buttonState)
    {
        if (buttonState != ButtonState.Pressed) return;
        _inputData.InteractionButton.UnsubscribeFromChange(ExitArenaOnInput);
        GameData.Instance.HeroData.HeroState.Value = HeroState.Disappearing;
    }
    
    protected override void OnDispose()
    {
        _inputData.InteractionButton.UnsubscribeFromChange(ExitArenaOnInput);
        base.OnDispose();
    }
}
