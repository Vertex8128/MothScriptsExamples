public sealed class HeroRerollHolderInteraction : BaseController
{
    private readonly InputData _inputData;
    private readonly ActiveBattleArenaData _arenaData;
    private readonly RerollHolderInteractionProcessor _rerollHolderInteractionProcessor;

    public HeroRerollHolderInteraction(RerollHolderInteractionProcessor rerollHolderInteractionProcessor)
    {
        _inputData = GameData.Instance.Input;
        _arenaData = (ActiveBattleArenaData)GameData.Instance.LevelData.CurrentArenaData;
        _rerollHolderInteractionProcessor = rerollHolderInteractionProcessor;
        _inputData.InteractionButton.SubscribeToChange(ChangeEquipmentOnInput);
    }

    private void ChangeEquipmentOnInput(ButtonState buttonState)
    {
        if (buttonState != ButtonState.Pressed) return;
        
        if(_arenaData.IsRerollingItems.Value) return;

        var heroData = GameData.Instance.HeroData;
        if (heroData.CurrentHeroDrops.Value < _rerollHolderInteractionProcessor.RerollPrice)
        {
            _rerollHolderInteractionProcessor.NotEnoughMoneyForPurchase?.Invoke();
            return;
        }

        heroData.ReceivedStatsData.Value = new StatsData(StatsImpactID.HeroCurrentDropsDecrease, _rerollHolderInteractionProcessor.RerollPrice);
        _arenaData.IsRerollingItems.Value = true;
    }
    
    protected override void OnDispose()
    {
        _inputData.InteractionButton.UnsubscribeFromChange(ChangeEquipmentOnInput);
        base.OnDispose();
    }
}
