public sealed class HeroSpawnController : BaseController
{
    private readonly ActiveHeroData _heroData;
    private readonly ISubscriptionReactiveProperty<LevelState> _levelState;
    private readonly TimerController _appearDelayTimer;

    public HeroSpawnController(ActiveHeroData heroData)
    {
        _heroData = heroData;
        _levelState = GameData.Instance.LevelState;
        _heroData.HeroState.Value = HeroState.Deactivated;
        
        _appearDelayTimer = new TimerController(StartHeroAppearing, false, GameData.Instance.GameParams.startArenaDelay);
        AddChildController(_appearDelayTimer);
        
        _levelState.SubscribeToChange(SpawnHeroOnLevelStateChange);
    }

    private void SpawnHeroOnLevelStateChange(LevelState levelState)
    {
       if (levelState != LevelState.ArenaStarting) return;
       _heroData.HeroObjectDataKeeper.gameObject.transform.position = GameData.Instance.LevelData.CurrentArenaData.ArenaObjectDataKeeper.heroSpawnTransform.position;
       _appearDelayTimer.StartWithSetDelay();
    }

    private void StartHeroAppearing()
    {
        if(GameData.Instance.IsLoading.Value) 
            GameData.Instance.IsLoading.Value = false;
        _heroData.HeroState.Value = HeroState.Appearing;
    }

    protected override void OnDispose()
    {
        _levelState.UnsubscribeFromChange(SpawnHeroOnLevelStateChange);
        base.OnDispose();
    }
}