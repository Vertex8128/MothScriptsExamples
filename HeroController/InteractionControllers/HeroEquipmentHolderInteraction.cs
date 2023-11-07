public sealed class HeroEquipmentHolderInteraction : BaseController
{
    private readonly InputData _inputData;
    private readonly EqipmentHolderInteractionProcessor _equipmentHolderInteractionProcessor;

    public HeroEquipmentHolderInteraction(EqipmentHolderInteractionProcessor equipmentHolderInteractionProcessor)
    {
        _inputData = GameData.Instance.Input;
        _equipmentHolderInteractionProcessor = equipmentHolderInteractionProcessor;
        _inputData.InteractionButton.SubscribeToChange(ChangeEquipmentOnInput);
    }

    private void ChangeEquipmentOnInput(ButtonState buttonState)
    {
        if (buttonState != ButtonState.Pressed) return;
        var heroData = GameData.Instance.HeroData;

        if (heroData.CurrentHeroDrops.Value < _equipmentHolderInteractionProcessor.ItemPrice)
        {
            _equipmentHolderInteractionProcessor.NotEnoughMoneyForPurchase?.Invoke();
            return;
        }
        
        if (_equipmentHolderInteractionProcessor.EquipmentData.equipmentID == EquipmentID.Artefact)
        {
            if (((ArtifactData) _equipmentHolderInteractionProcessor.EquipmentData).artifactID == ArtifactID.CurrentHealth
                && GameData.Instance.HeroData.CurrentHeroHealth.Value == GameData.Instance.HeroData.HeroMaxHealth.Value)
            {
                GameData.Instance.ActiveSound.Value = SoundID.HeroPurchaseDenied;
                return;
            }
        }

        _inputData.InteractionButton.UnsubscribeFromChange(ChangeEquipmentOnInput);
        
        GameData.Instance.ActiveSound.Value = _equipmentHolderInteractionProcessor.EquipmentData.equipmentID == EquipmentID.Artefact 
            ? SoundID.HeroArtefactPurchase 
            : SoundID.HeroWeaponPurchase;
        
        heroData.ReceivedStatsData.Value = new StatsData(StatsImpactID.HeroCurrentDropsDecrease, _equipmentHolderInteractionProcessor.EquipmentData.itemPrice);
        heroData.ReceivedEquipmentData.Value = _equipmentHolderInteractionProcessor.EquipmentData;
    }
    
    protected override void OnDispose()
    {
        _inputData.InteractionButton.UnsubscribeFromChange(ChangeEquipmentOnInput);
        base.OnDispose();
    }
}