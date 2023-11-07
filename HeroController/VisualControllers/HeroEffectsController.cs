public sealed class HeroEffectsController : BaseController
{
    private readonly ActiveHeroData _heroData;
    private readonly HeroEffectsHandler _heroEffectsHandler;

    public HeroEffectsController(ActiveHeroData heroData)
    {
        _heroData = heroData;
        _heroEffectsHandler = heroData.HeroObjectDataKeeper.HeroEffectsHandler;
        _heroEffectsHandler.SetEffects(OnCompleteAppearEffect, OnCompleteDisappearEffect);

        _heroData.HeroState.SubscribeToChange(PlayEffectOnChangeHeroState);
        _heroData.GettingHit.SubscribeToChange(PlayEffectOnGettingHit);
        _heroData.ReceivedEquipmentData.SubscribeToChange(PlayEffectOnSwitchingWeapon);
        _heroData.IsHeroFlipped.SubscribeToChange(FlipEffectsOnChangeIsCharacterFlipped);
    }

    private void PlayEffectOnChangeHeroState(HeroState heroState) => _heroEffectsHandler.PlayHeroStageEffect(heroState);

    private void PlayEffectOnGettingHit() => _heroEffectsHandler.PlayHeroStageEffect(HeroState.GettingHit);
    private void PlayEffectOnSwitchingWeapon(EquipmentData equipmentData)
    {
        if (!(equipmentData is WeaponData weaponData)) return;
        if (weaponData.weaponID == WeaponID.Default) return;
        _heroEffectsHandler.PlaySwitchWeaponEffect();
    }
    
    private void FlipEffectsOnChangeIsCharacterFlipped(bool isCharacterFlipped) => _heroEffectsHandler.FlipEffects();
    
    private void OnCompleteAppearEffect()
    {
        _heroData.HeroState.Value = HeroState.Activated;
        GameData.Instance.LevelData.CurrentArenaData.ArenaState.Value = ArenaState.HeroSpawned;
    }

    private void OnCompleteDisappearEffect()
    {
        GameData.Instance.IsLoading.Value = true;
        _heroData.HeroState.Value = HeroState.Deactivated;
        GameData.Instance.LevelState.Value = LevelState.ArenaExiting;
    } 

    protected override void OnDispose()
    {
        _heroData.HeroState.UnsubscribeFromChange(PlayEffectOnChangeHeroState);
        _heroData.GettingHit.UnsubscribeFromChange(PlayEffectOnGettingHit);
        _heroData.ReceivedEquipmentData.UnsubscribeFromChange(PlayEffectOnSwitchingWeapon);
        _heroData.IsHeroFlipped.UnsubscribeFromChange(FlipEffectsOnChangeIsCharacterFlipped);
        base.OnDispose();
    }
}