public sealed class HeroHealthController : CharacterHealthController
{
    private readonly ActiveHeroData _heroData;
    private readonly TimerController _immunityTimer;
    private int _previousCurrentHealthValue;
    
    public HeroHealthController(ActiveHeroData heroData)
    {
        _heroData = heroData;
        _immunityTimer = new TimerController(ResetInvulnerability, false, _heroData.InitialHeroData.OnDamageInvulnerabilityTime);
        AddChildController(_immunityTimer);
        _previousCurrentHealthValue = heroData.CurrentHeroHealth.Value;
        
        var heroHealthBarController = new HeroHealthBarController();
        AddChildController(heroHealthBarController);

        _heroData.CurrentHeroHealth.SubscribeToChange(UpdateHeroStatusOnCurrentHealthChange);
    }

    private void UpdateHeroStatusOnCurrentHealthChange(int currentHealth)
    {
        if (currentHealth < 0) return;
        
        if(currentHealth < _previousCurrentHealthValue) 
            _heroData.GettingHit.Invoke();
        else
            _heroData.GettingHeal.Invoke();
        _previousCurrentHealthValue = currentHealth;

        _heroData.IsInvulnerable = true;
        _immunityTimer.StartWithSetDelay();
    }

    private void ResetInvulnerability() => _heroData.IsInvulnerable = false;

    protected override void OnDispose()
    {
        _heroData.CurrentHeroHealth.UnsubscribeFromChange(UpdateHeroStatusOnCurrentHealthChange);
        base.OnDispose();
    }
}