public sealed class HeroWaveStarterInteraction : BaseController
{
    private readonly InputData _inputData;

    public HeroWaveStarterInteraction()
    {
        _inputData = GameData.Instance.Input;
        _inputData.InteractionButton.SubscribeToChange(ActivateAltarOnInput);
    }

    private void ActivateAltarOnInput(ButtonState buttonState)
    {
        if (buttonState != ButtonState.Released) return;
        _inputData.InteractionButton.UnsubscribeFromChange(ActivateAltarOnInput);
        GameData.Instance.LevelData.CurrentArenaData.ArenaState.Value = ArenaState.WaveSpawningGoing;
        GameData.Instance.ActiveSound.Value = SoundID.ArenaLever;
    }

    protected override void OnDispose()
    {
        _inputData.InteractionButton.UnsubscribeFromChange(ActivateAltarOnInput);
        base.OnDispose();
    }
}
