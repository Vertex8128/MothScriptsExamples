public sealed class HeroWeaponDefaultController : HeroWeaponAutoShotController
{
    private readonly WeaponDefaultParams _weaponDefaultParams;
    
    public HeroWeaponDefaultController(ActiveHeroData heroData, WeaponData weaponData, HeroWeaponMagazineBarController heroWeaponMagazineBarController) 
        : base(heroData, weaponData, heroWeaponMagazineBarController)
    {
        _weaponDefaultParams = (WeaponDefaultParams)weaponData.weaponParams;
        weaponObject.SetActive(false);
    }

    protected override void CastProjectile()
    {
        base.CastProjectile();
        heroWeaponHandler.PlayFireEffect();
        GameData.Instance.ActiveSound.Value = SoundID.WeaponDefault;
        var projectileController = (HeroProjectileDefaultController)GameData.Instance.ChargersData.GetHeroDamageObject(_weaponDefaultParams.projectileID);
        projectileController.SpawnObject(gunPointTransform.position, GetProjectileRotation(_weaponDefaultParams.weaponSpreadOffset), 
            heroData.CurrentWeaponRange, _weaponDefaultParams.projectileSize, _weaponDefaultParams.projectileSpeed,
            _weaponDefaultParams.projectileBaseColor, GetDamageInteractionDataList());
    }
}