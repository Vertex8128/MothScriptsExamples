
public sealed class HeroEquipmentController : BaseController
{
    private readonly ActiveHeroData _heroData;
    private HeroWeaponController _currentWeapon;
    private HeroDamageObjectID _currentHeroDamageObject;
    
    private readonly HeroWeaponMagazineBarController _heroWeaponMagazineBarController;
    private readonly HeroArtifactBarController _heroArtifactBarController;

    public HeroEquipmentController(ActiveHeroData heroData)
    {
        _heroData = heroData;

        _heroWeaponMagazineBarController = new HeroWeaponMagazineBarController();
        AddChildController(_heroWeaponMagazineBarController);
        _heroArtifactBarController = new HeroArtifactBarController();
        AddChildController(_heroArtifactBarController);
        
        _currentHeroDamageObject = HeroDamageObjectID.None;
        var defaultWeapon = GameData.Instance.Prefabs.heroDefaultWeaponData;
        CreateWeaponOnEquipmentDataChanged(defaultWeapon);
        _heroData.currentWeaponPower = defaultWeapon.weaponPower;

        _heroData.ReceivedEquipmentData.SubscribeToChange(CreateWeaponOnEquipmentDataChanged);
        _heroData.HeroState.SubscribeToChange(DestroyWeaponOnHeroStateChange);
   }

    private void CreateWeaponOnEquipmentDataChanged(EquipmentData equipmentData)
    {
        switch (equipmentData.equipmentID)
        {
            case EquipmentID.Weapon:
            {
                var weaponData = (WeaponData)equipmentData;

                if (_currentHeroDamageObject != HeroDamageObjectID.None)
                    GameData.Instance.ChargersData.ResetCharger.Value = _currentHeroDamageObject;
                _currentHeroDamageObject = weaponData.weaponParams.ProjectileID;

                _currentWeapon?.Dispose();
                switch (weaponData.weaponID)
                {
                    case WeaponID.Default:
                        _currentWeapon = new HeroWeaponDefaultController(_heroData, weaponData, _heroWeaponMagazineBarController);
                        break;
                    case WeaponID.Fan:
                        _currentWeapon = new HeroWeaponFanController(_heroData, weaponData, _heroWeaponMagazineBarController);
                        break;
                    case WeaponID.Spray:
                        _currentWeapon = new HeroWeaponSprayController(_heroData, weaponData, _heroWeaponMagazineBarController);
                        break;
                    case WeaponID.ChargingOrb:
                        _currentWeapon = new HeroWeaponChargingOrbController(_heroData, weaponData, _heroWeaponMagazineBarController);
                        break;
                    case WeaponID.Boomerang:
                        _currentWeapon = new HeroWeaponBoomerangController(_heroData, weaponData, _heroWeaponMagazineBarController);
                        break;
                    case WeaponID.ElectricChain:
                        _currentWeapon = new HeroWeaponElectricChainController(_heroData, weaponData, _heroWeaponMagazineBarController);
                        break;
                    case WeaponID.Meteor:
                        _currentWeapon = new HeroWeaponMeteorController(_heroData, weaponData, _heroWeaponMagazineBarController);
                        break;
                }
                break;
            }
            case EquipmentID.Artefact:
                var artifactData = (ArtifactData)equipmentData;
                _heroArtifactBarController.UpdateArtifactBar(artifactData);
                break;
        }
    }

    private void DestroyWeaponOnHeroStateChange(HeroState heroState)
    {
        if(heroState != HeroState.Defeated) return;
        _currentWeapon?.Dispose();
    }

    protected override void OnDispose()
    {
        _heroData.ReceivedEquipmentData.UnsubscribeFromChange(CreateWeaponOnEquipmentDataChanged);
        _heroData.HeroState.UnsubscribeFromChange(DestroyWeaponOnHeroStateChange);
        _currentWeapon?.Dispose();
    }
} 