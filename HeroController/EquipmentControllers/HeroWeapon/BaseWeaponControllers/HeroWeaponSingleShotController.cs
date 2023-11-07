public class HeroWeaponSingleShotController : HeroWeaponController
{
    protected HeroWeaponSingleShotController(ActiveHeroData heroData, WeaponData weaponData, HeroWeaponMagazineBarController heroWeaponMagazineBarController) 
        : base(heroData, weaponData, heroWeaponMagazineBarController)
    {
        inputData.FireBaseButton.SubscribeToChange(FireOnInput);
    }

    private void FireOnInput(ButtonState buttonState)
    {
        if(buttonState != ButtonState.Pressed) return;
        if(isReloading) return;
        CastProjectile();
    }

    protected virtual void CastProjectile()
    {
        projectileNumberInMagazine --;
        heroWeaponMagazineBarController.UpdateCurrentValue(projectileNumberInMagazine);
        if(projectileNumberInMagazine > 0) return;
        StartReload();
    }

    protected override void OnDispose()
    {
        inputData.FireBaseButton.UnsubscribeFromChange(FireOnInput);
        base.OnDispose();
    }
}