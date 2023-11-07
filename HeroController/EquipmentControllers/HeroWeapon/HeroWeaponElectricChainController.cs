public sealed class HeroWeaponElectricChainController : HeroWeaponSingleShotController
{
    private readonly WeaponElectricChainParams _weaponElectricChainParams;
    
    public HeroWeaponElectricChainController(ActiveHeroData heroData, WeaponData weaponData, HeroWeaponMagazineBarController heroWeaponMagazineBarController) 
        : base(heroData, weaponData, heroWeaponMagazineBarController)
    {
        _weaponElectricChainParams = (WeaponElectricChainParams)weaponData.weaponParams;
    }

    protected override void CastProjectile()
    {
        base.CastProjectile();
        heroWeaponHandler.PlayFireEffect();
        GameData.Instance.ActiveSound.Value = SoundID.WeaponElectricChain;
        var projectileController = (HeroProjectileElectricChainController)GameData.Instance.ChargersData.GetHeroDamageObject(_weaponElectricChainParams.projectileID);
        projectileController.SpawnObject(gunPointTransform, GetProjectileRotation(0f), 
            heroData.CurrentWeaponRange, _weaponElectricChainParams.projectileSpeed, _weaponElectricChainParams.targetsNumber, _weaponElectricChainParams.jumpProbability,
            GetDamageInteractionDataList());
    }
}
