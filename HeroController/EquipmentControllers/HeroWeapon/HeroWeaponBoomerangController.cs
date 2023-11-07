public sealed class HeroWeaponBoomerangController : HeroWeaponSingleShotController
{
    private readonly WeaponBoomerangParams _weaponBoomerangParams;
    
    public HeroWeaponBoomerangController(ActiveHeroData heroData, WeaponData weaponData, HeroWeaponMagazineBarController heroWeaponMagazineBarController) 
        : base(heroData, weaponData, heroWeaponMagazineBarController)
    {
        _weaponBoomerangParams = (WeaponBoomerangParams)weaponData.weaponParams;
    }

    protected override void CastProjectile()
    {
        base.CastProjectile();
        heroWeaponHandler.PlayFireEffect();
        GameData.Instance.ActiveSound.Value = SoundID.WeaponBoomerang;
        var projectileController = (HeroProjectileBoomerangController)GameData.Instance.ChargersData.GetHeroDamageObject(_weaponBoomerangParams.projectileID);
        projectileController.SpawnObject(gunPointTransform.position, GetProjectileRotation(0f), 
            heroData.CurrentWeaponRange, _weaponBoomerangParams.projectileSize, _weaponBoomerangParams.projectileSpeed, _weaponBoomerangParams.projectileFinalSpeed, _weaponBoomerangParams.finalPointWaitTime, 
            _weaponBoomerangParams.projectileBaseColor, GetDamageInteractionDataList());
    }
}