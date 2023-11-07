public sealed class HeroWeaponFanController : HeroWeaponAutoShotController
{
    private readonly WeaponFanParams _weaponFanParams;
    
    public HeroWeaponFanController(ActiveHeroData heroData, WeaponData weaponData, HeroWeaponMagazineBarController heroWeaponMagazineBarController) 
        : base(heroData, weaponData, heroWeaponMagazineBarController)
    {
        _weaponFanParams = (WeaponFanParams)weaponData.weaponParams;
    }
    
    protected override void CastProjectile()
    {
        base.CastProjectile();
        heroWeaponHandler.PlayFireEffect();
        GameData.Instance.ActiveSound.Value = SoundID.WeaponDefault;
        foreach (var gunPoint in heroWeaponObjectDataKeeper.gunPointsList)
        {
            var projectileController = (HeroProjectileDefaultController)GameData.Instance.ChargersData.GetHeroDamageObject(_weaponFanParams.projectileID);
            projectileController.SpawnObject(gunPointTransform.position, GetProjectileRotation(_weaponFanParams.weaponSpreadOffset, gunPoint), 
                heroData.CurrentWeaponRange, _weaponFanParams.projectileSize, _weaponFanParams.projectileSpeed,
                _weaponFanParams.projectileBaseColor, GetDamageInteractionDataList());
        }
    }
}