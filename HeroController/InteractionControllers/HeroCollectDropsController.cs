public sealed class HeroCollectDropsController : BaseController
{
    private readonly ActiveHeroData _heroData;
    private readonly HeroCollectDropsHandler _collectDropsHandler;

    public HeroCollectDropsController(ActiveHeroData heroData)
    {
        _heroData = heroData;
        _collectDropsHandler = heroData.HeroObjectDataKeeper.CollectDropsHandler;
        
        UpdateColliderSizeOnBonusChange(0f);

        var heroDropsBarController = new HeroDropsBarController();
        AddChildController(heroDropsBarController);

        _heroData.HeroDropsPickupRadiusBonusPRC.SubscribeToChange(UpdateColliderSizeOnBonusChange);
    }

    private void UpdateColliderSizeOnBonusChange(float heroDropsPickupRadiusBonusPRC)
    {
        var radius = Utils.GetIncreasedPercentValue(_heroData.InitialHeroData.dropsPickupRadiusDefault, heroDropsPickupRadiusBonusPRC, 1);
        _collectDropsHandler.SetPickupZoneRadius(radius);
        _heroData.CurrentHeroPickupRadius = radius;
    }
    
    protected override void OnDispose()
    {
        _heroData.HeroDropsPickupRadiusBonusPRC.UnsubscribeFromChange(UpdateColliderSizeOnBonusChange);
        base.OnDispose();
    }
}