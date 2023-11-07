public sealed class HeroWeaponMeteorController : HeroWeaponAutoShotController
{
    private readonly WeaponMeteorParams _weaponMeteorParams;
    
    public HeroWeaponMeteorController(ActiveHeroData heroData, WeaponData weaponData, HeroWeaponMagazineBarController heroWeaponMagazineBarController) 
        : base(heroData, weaponData, heroWeaponMagazineBarController)
    {
        _weaponMeteorParams = (WeaponMeteorParams)weaponData.weaponParams;
    }

    protected override void CastProjectile()
    {
        base.CastProjectile();
        heroWeaponHandler.PlayFireEffect();
        GameData.Instance.ActiveSound.Value = SoundID.WeaponMeteor;
        var projectileController = (HeroProjectileMeteorController)GameData.Instance.ChargersData.GetHeroDamageObject(_weaponMeteorParams.projectileID);
        projectileController.SpawnObject(gunPointTransform.position, GetProjectileRotation(_weaponMeteorParams.weaponSpreadOffset), 
            heroData.CurrentWeaponRange, _weaponMeteorParams.projectileSize, _weaponMeteorParams.projectileSpeed, _weaponMeteorParams.targetsNumber,
            _weaponMeteorParams.projectileBaseColor, _weaponMeteorParams.trailCenterGradient, _weaponMeteorParams.trailSidesGradient, GetDamageInteractionDataList());
    }
}
