public sealed class HeroSpriteController : CharacterSpriteController
{
    private readonly ISubscriptionReactiveProperty<HeroState> _heroState;
    private const int HeroUISortingLayer = 110;

    public HeroSpriteController(ActiveHeroData heroData)
        : base(heroData.HeroObjectDataKeeper.CharacterLayerSetter, heroData.IsHeroFlipped)
    {
        _heroState = heroData.HeroState;
        _heroState.SubscribeToChange(UpdateLayerOnDHeroStateChange);
    }

    private void UpdateLayerOnDHeroStateChange(HeroState heroState)
    {
        if(heroState != HeroState.Defeated) return;
        updateSortingLayerOrderTimer.StopAndReset();
        characterLayerSetter.SetSortingGroupLayer(SortingLayerName.StaticUI);
        characterLayerSetter.SetSortingGroupLayerOrder(HeroUISortingLayer);
        characterLayerSetter.SetVisualObjectsLayers(ObjectLayerName.UI);
    }

    protected override void OnDispose()
    {
        _heroState.UnsubscribeFromChange(UpdateLayerOnDHeroStateChange);
        base.OnDispose();
    }
}