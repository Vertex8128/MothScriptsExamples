public sealed class HeroWeaponSprayController : HeroWeaponAutoShotController
{
    private readonly WeaponSprayParams _weaponSprayParams;
    private readonly HeroProjectileSprayController _projectileSprayController;
    private readonly TimerController _fireEffectTimerController;
    private const float FireEffectDelay = 0.1f;

    public  HeroWeaponSprayController(ActiveHeroData heroData, WeaponData weaponData, HeroWeaponMagazineBarController heroWeaponMagazineBarController) 
        : base(heroData, weaponData, heroWeaponMagazineBarController)
    {
        _weaponSprayParams = (WeaponSprayParams)weaponData.weaponParams;
        _projectileSprayController = new HeroProjectileSprayController(GameData.Instance.Prefabs.GetHeroDamageObjectPrefab(HeroDamageObjectID.ProjectileSpray));
        AddChildController(_projectileSprayController);
        
        _fireEffectTimerController = new TimerController(heroWeaponHandler.PlayFireEffect, true, FireEffectDelay);
        AddChildController(_fireEffectTimerController);
    }

    protected override void CastProjectile()
    {
        base.CastProjectile();
        if (!_projectileSprayController.IsActive)
        {
            GameData.Instance.ActiveLoopSound.Value = (SoundID.WeaponSpray, true);
            _fireEffectTimerController.StartWithAction();
            _projectileSprayController.SpawnObject(heroData.CurrentWeaponRange, _weaponSprayParams.projectileSpeed, _weaponSprayParams.weaponFireRate, _weaponSprayParams.spraySpreadAngle, GetDamageInteractionDataList());
        }
        if (projectileNumberInMagazine <= 0)
            ResetSprayFire();
    }

    protected override void FireOnInput(ButtonState buttonState)
    {
        base.FireOnInput(buttonState);
        if (buttonState == ButtonState.Released)
            ResetSprayFire();
    }

    private void ResetSprayFire()
    {
        GameData.Instance.ActiveLoopSound.Value = (SoundID.WeaponSpray, false);
        _projectileSprayController.ResetObject();
        _fireEffectTimerController.StopAndReset();
    }
}