public class HeroWeaponAccumulatingShotController : HeroWeaponController
{
    protected bool isAccumulatingStageActive;

    protected HeroWeaponAccumulatingShotController(ActiveHeroData heroData, WeaponData weaponData, HeroWeaponMagazineBarController heroWeaponMagazineBarController) 
        : base(heroData, weaponData, heroWeaponMagazineBarController)
    {
        inputData.FireBaseButton.SubscribeToChange(FireOnInput);
    }
    
    private void FireOnInput(ButtonState buttonState)
    {
        if (isReloading) return;
        switch (buttonState)
        {
            case ButtonState.Pressed:
                PrecastProjectile();
                break;

            case ButtonState.Released:
                if(!isAccumulatingStageActive) return;
                CastProjectile();
                break;
        }
    }

    protected virtual void PrecastProjectile() { }

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