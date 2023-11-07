public class HeroWeaponAutoShotController : HeroWeaponController
{
    private readonly TimerController _castProjectileDelayTimer;

    protected HeroWeaponAutoShotController(ActiveHeroData heroData, WeaponData weaponData, HeroWeaponMagazineBarController heroWeaponMagazineBarController) 
        : base(heroData, weaponData, heroWeaponMagazineBarController)
    {
        var castProjectileDelay = 60f / ((WeaponAutoShotParams)weaponData.weaponParams).weaponFireRate;
        _castProjectileDelayTimer = new TimerController(CastProjectile, true, castProjectileDelay);
        AddChildController(_castProjectileDelayTimer);

        inputData.FireBaseButton.SubscribeToChange(FireOnInput);
    }
    
    protected override void UpdateWeaponVisibilityOnHeroStateChange(HeroState heroState)
    {
        base.UpdateWeaponVisibilityOnHeroStateChange(heroState);

        switch (heroState)
        {
            case HeroState.Frozen:
            case HeroState.Disappearing:
                _castProjectileDelayTimer.StopAndReset();
                inputData.FireBaseButton.Value = ButtonState.None; 
                break;
        }
    }

    protected virtual void FireOnInput(ButtonState buttonState)
    {
        switch (buttonState)
        {
            case ButtonState.Pressed:
                if (isReloading) return;
                _castProjectileDelayTimer.StartWithAction();
                break;

            case ButtonState.Released:
                _castProjectileDelayTimer.StopAndReset();
                break;
        }
    }

    protected override void StartReload()
    {
        base.StartReload();
        if(_castProjectileDelayTimer.IsRunning)
            _castProjectileDelayTimer.StopAndReset();
    }

    protected virtual void CastProjectile()
    {
        projectileNumberInMagazine --;
        heroWeaponMagazineBarController.UpdateCurrentValue(projectileNumberInMagazine);
        if(projectileNumberInMagazine > 0) return;
        _castProjectileDelayTimer.Pause();
        StartReload();
    }

    protected override void OnReloadCompleted()
    {
        base.OnReloadCompleted();
        if(_castProjectileDelayTimer.IsPaused)
            _castProjectileDelayTimer.Continue();
    }

    protected override void OnDispose()
    {
        inputData.FireBaseButton.UnsubscribeFromChange(FireOnInput);
        base.OnDispose();
    }
}