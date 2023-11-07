using System.Collections.Generic;
using JoostenProductions;
using UnityEngine;

public sealed class HeroWeaponChargingOrbController : HeroWeaponAccumulatingShotController
{
    private readonly WeaponChargingOrbParams _weaponChargingOrbParams;
    private HeroProjectileChargingOrbController _currentChargingOrbController;

    private float _currentTransitionTime;
    private float _transitionIndex;
    private List<ImpactData> _interactionDataList;
    
    private readonly float _projectileStageTransitionTime;
    private const float MinDamageZoneRadius = 2f;

    public HeroWeaponChargingOrbController(ActiveHeroData heroData, WeaponData weaponData, HeroWeaponMagazineBarController heroWeaponMagazineBarController) 
        : base(heroData, weaponData, heroWeaponMagazineBarController)
    {
        _weaponChargingOrbParams = (WeaponChargingOrbParams)weaponData.weaponParams;
        _projectileStageTransitionTime = _weaponChargingOrbParams.stageTransitionTime;
    }

    protected override void PrecastProjectile()
    {
        base.PrecastProjectile();
        heroWeaponHandler.PlayFireEffect();
        _currentChargingOrbController = (HeroProjectileChargingOrbController)GameData.Instance.ChargersData.GetHeroDamageObject(_weaponChargingOrbParams.projectileID);
        _currentChargingOrbController.SpawnObject(gunPointTransform.position, _weaponChargingOrbParams.projectileStartSize, _weaponChargingOrbParams.projectileFinalSize, 
            _weaponChargingOrbParams.projectileStartColor, _weaponChargingOrbParams.projectileFinalColor);
        _currentTransitionTime = 0;
        _transitionIndex = 0;
        UpdateManager.SubscribeToUpdate(ApplyProjectileTransitionOnUpdate);
        isAccumulatingStageActive = true;
    }

    private void ApplyProjectileTransitionOnUpdate()
    {
        _currentTransitionTime += Time.deltaTime;
        _transitionIndex = _currentTransitionTime / _projectileStageTransitionTime;
        _currentChargingOrbController.TransitObjectStage(gunPointTransform.position, _transitionIndex);
        
        if(_transitionIndex < 1f) return;
        if(!isAccumulatingStageActive) return;
        CastProjectile();
    }

    protected override void CastProjectile()
    {
        GameData.Instance.ActiveSound.Value = SoundID.WeaponChargingOrb;
        isAccumulatingStageActive = false;
        UpdateManager.UnsubscribeFromUpdate(ApplyProjectileTransitionOnUpdate);
        _interactionDataList = GetDamageInteractionDataList(_weaponChargingOrbParams.weaponDamageFullChargeBonus * _transitionIndex);
        _currentChargingOrbController.StartObjectMoving(GetProjectileRotation(0f), _weaponChargingOrbParams.projectileRange, _weaponChargingOrbParams.projectileSpeed, _interactionDataList, SpawnZoneOnResetProjectile);
        base.CastProjectile();
    }

    private void SpawnZoneOnResetProjectile(Vector2 spawnPosition)
    {
        GameData.Instance.ActiveSound.Value = SoundID.WeaponChargingOrbExplosion;
        var zoneSize = Mathf.Lerp(MinDamageZoneRadius, _weaponChargingOrbParams.damageZoneFullChargeRadius, _transitionIndex);
        var zoneColor = Color.Lerp(_weaponChargingOrbParams.projectileStartColor, _weaponChargingOrbParams.projectileFinalColor, _transitionIndex);

        var zoneChargingOrbController = (HeroZoneChargingOrbController)GameData.Instance.ChargersData.GetHeroDamageObject(HeroDamageObjectID.ZoneChargingOrb);
        zoneChargingOrbController.SpawnObject(spawnPosition, zoneSize, _weaponChargingOrbParams.damageZoneActiveTime, zoneColor, _interactionDataList);
    }

    protected override void OnDispose()
    {
        UpdateManager.UnsubscribeFromUpdate(ApplyProjectileTransitionOnUpdate);
        base.OnDispose();
    }
}