using System.Collections.Generic;
using UnityEngine;

public class HeroWeaponController : BaseController
{
    protected readonly ActiveHeroData heroData;
    private readonly WeaponData _weaponData;
    private readonly WeaponParams _weaponParams;
    protected readonly GameObject weaponObject;
    protected readonly HeroWeaponObjectDataKeeper heroWeaponObjectDataKeeper;

    private readonly Transform _aimHolderTransform;
    protected readonly Transform gunPointTransform;

    protected readonly HeroWeaponHandler heroWeaponHandler;
    protected readonly HeroWeaponMagazineBarController heroWeaponMagazineBarController;
    private readonly HeroReloadPanelHandler _heroReloadPanelHandler;
    protected bool isReloading;
    protected int projectileNumberInMagazine;

    protected readonly InputData inputData;
    
    private const float MinTotalWeaponReloadTime = 0.1f;
    private const float ToAxisYOffset = 90f;

    protected HeroWeaponController(ActiveHeroData heroData, WeaponData weaponData, HeroWeaponMagazineBarController heroWeaponMagazineBarController)
    {
        this.heroData = heroData;
        _weaponData = weaponData;
        _weaponParams = (WeaponParams)weaponData.weaponParams;
        weaponObject = GameObject.Instantiate(weaponData.weaponObject, heroData.HeroObjectDataKeeper.weaponHolder.transform);
        AddGameObject(weaponObject);
        heroWeaponObjectDataKeeper = weaponObject.GetComponent<HeroWeaponObjectDataKeeper>();

        _aimHolderTransform = heroData.HeroObjectDataKeeper.aimHolder.transform;
        gunPointTransform = heroWeaponObjectDataKeeper.GunPointTransform;
            
        // Weapon Params Set
        heroData.CurrentWeaponDamage = new Vector2Int(
                Utils.GetIncreasedPercentValue(_weaponParams.weaponDamage.x, this.heroData.WeaponDamageBonusPRC.Value, 1),             
                Utils.GetIncreasedPercentValue(_weaponParams.weaponDamage.y, this.heroData.WeaponDamageBonusPRC.Value, 1));

        heroData.CurrentWeaponMagazineSize = Utils.GetIncreasedPercentValue(_weaponParams.weaponMagazineSize, this.heroData.WeaponMagazineSizeBonusPRC.Value, 1);
        
        var reloadTime = Utils.GetDecreasedPercentValue(_weaponParams.weaponReloadTime, this.heroData.WeaponReloadTimeBonusPRC.Value, 1);
        heroData.CurrentWeaponReloadTime = reloadTime < MinTotalWeaponReloadTime 
            ? MinTotalWeaponReloadTime 
            : reloadTime;
        
        heroData.CurrentWeaponRange = Utils.GetIncreasedPercentValue(_weaponParams.projectileRange, this.heroData.WeaponRangeBonusPRC.Value, 1);

        heroWeaponHandler = heroWeaponObjectDataKeeper.HeroWeaponHandler;
        this.heroWeaponMagazineBarController = heroWeaponMagazineBarController;
        SetMagazineBarParams();
        _heroReloadPanelHandler = heroData.HeroObjectDataKeeper.reloadPanel.GetComponent<HeroReloadPanelHandler>();
        
        inputData = GameData.Instance.Input;

        inputData.ReloadWeaponButton.SubscribeToChange(ReloadOnInput);
        this.heroData.HeroState.SubscribeToChange(UpdateWeaponVisibilityOnHeroStateChange);
        this.heroData.WeaponDamageBonusPRC.SubscribeToChange(UpdateTotalWeaponDamageOnBonusChange);
        this.heroData.WeaponMagazineSizeBonusPRC.SubscribeToChange(UpdateTotalWeaponMagazineSizeOnBonusChange);
        this.heroData.WeaponReloadTimeBonusPRC.SubscribeToChange(UpdateTotalWeaponReloadTimeOnBonusChange);
        this.heroData.WeaponRangeBonusPRC.SubscribeToChange(UpdateTotalWeaponRangeOnBonusChange);
    }

    protected virtual void UpdateWeaponVisibilityOnHeroStateChange(HeroState heroState)
    {
        switch (heroState)
        {
            case HeroState.Activated:
                weaponObject.SetActive(true);

                break;
            case HeroState.Disappearing:
                weaponObject.SetActive(false);
                break;
        }
    }

    private void ReloadOnInput(ButtonState buttonState)
    {
        if(buttonState != ButtonState.Pressed) return;
        if(isReloading) return;
        if(projectileNumberInMagazine == heroData.CurrentWeaponMagazineSize) return;
        StartReload();
    }

    protected virtual void StartReload()
    {
        isReloading = true;
        heroWeaponMagazineBarController.StartReload();

        var reloadTime = heroData.CurrentWeaponReloadTime * (heroData.CurrentWeaponMagazineSize-projectileNumberInMagazine) / heroData.CurrentWeaponMagazineSize;
        _heroReloadPanelHandler.StartReload(reloadTime);
    }

    protected virtual void OnReloadCompleted()
    {
        isReloading = false;
        projectileNumberInMagazine = heroData.CurrentWeaponMagazineSize;
    }

    #region Weapon Params UpdateOnChange
    private void UpdateTotalWeaponDamageOnBonusChange(float weaponDamageBonusPRC) =>
        heroData.CurrentWeaponDamage = new Vector2Int(
            Utils.GetIncreasedPercentValue(_weaponParams.weaponDamage.x, heroData.WeaponDamageBonusPRC.Value, 1),             
            Utils.GetIncreasedPercentValue(_weaponParams.weaponDamage.y, heroData.WeaponDamageBonusPRC.Value, 1));

    private void UpdateTotalWeaponMagazineSizeOnBonusChange(float weaponMagazineSizeBonusPRC)
    {
        heroData.CurrentWeaponMagazineSize = Utils.GetIncreasedPercentValue(_weaponParams.weaponMagazineSize, weaponMagazineSizeBonusPRC, 1);
        SetMagazineBarParams();
    }

    private void UpdateTotalWeaponReloadTimeOnBonusChange(float weaponReloadTimeBonus)
    {
        var reloadTime = Utils.GetDecreasedPercentValue(_weaponParams.weaponReloadTime, weaponReloadTimeBonus, 1);
        heroData.CurrentWeaponReloadTime = reloadTime < MinTotalWeaponReloadTime 
            ? MinTotalWeaponReloadTime 
            : reloadTime;
        SetMagazineBarParams();
    }
    
    private void UpdateTotalWeaponRangeOnBonusChange(float weaponRangeBonusPRC) => 
        heroData.CurrentWeaponRange = Utils.GetIncreasedPercentValue(_weaponParams.projectileRange, weaponRangeBonusPRC, 1);
    #endregion

    private void SetMagazineBarParams()
    {
        heroWeaponMagazineBarController.SetBaseValues(_weaponData.itemIcon, heroData.CurrentWeaponMagazineSize,
            heroData.CurrentWeaponReloadTime, _weaponData.rarityIndication, OnReloadCompleted);
        projectileNumberInMagazine = heroData.CurrentWeaponMagazineSize;
    }

    protected List<ImpactData> GetDamageInteractionDataList(float increaseCoefficient = 0)
    {
        var damageValue = Utils.GetRandomIntMaxIncluded(heroData.CurrentWeaponDamage);
        var oneTimeImpactInteractionData = Utils.GetOneTimeImpactInteractionData(CharacterID.Enemy, StatsImpactID.CurrentHealthDecrease,
            increaseCoefficient > 0 
                ? Utils.GetIncreasedPercentValue(damageValue, increaseCoefficient, 1)
                : damageValue);

        var interactionDataList = new List<ImpactData> {oneTimeImpactInteractionData};
        if(_weaponData.hasEffect)
            interactionDataList.Add(_weaponData.effectImpactData);
        return interactionDataList;
    }
    
    protected Quaternion GetProjectileRotation(float weaponSpreadOffset, Transform gunPointAimTransform = null)
    {
        if (weaponSpreadOffset == 0f)
            return GetBaseProjectileRotation();

        var offsetAngleInDegrees = weaponSpreadOffset / 2f;
        var randomOffsetAngleInRads = (Utils.GetRandomRoundedFloat(-offsetAngleInDegrees, offsetAngleInDegrees) + ToAxisYOffset) * Mathf.Deg2Rad;
        var offsetAngleVector = new Vector2(Mathf.Cos(randomOffsetAngleInRads), Mathf.Sin(randomOffsetAngleInRads));
        var offsetRotation = Quaternion.LookRotation(Vector3.forward, offsetAngleVector);
        return GetBaseProjectileRotation() * offsetRotation;
        
        Quaternion GetBaseProjectileRotation()
        {
            var rotationVector = gunPointAimTransform 
                ? gunPointAimTransform.position - gunPointTransform.position
                : _aimHolderTransform.position - gunPointTransform.position;
            return Quaternion.LookRotation(Vector3.forward, rotationVector);
        }
    }
    
    protected override void OnDispose()
    {
        inputData.ReloadWeaponButton.UnsubscribeFromChange(ReloadOnInput);
        heroData.HeroState.UnsubscribeFromChange(UpdateWeaponVisibilityOnHeroStateChange);
        heroData.WeaponDamageBonusPRC.UnsubscribeFromChange(UpdateTotalWeaponDamageOnBonusChange);
        heroData.WeaponMagazineSizeBonusPRC.UnsubscribeFromChange(UpdateTotalWeaponMagazineSizeOnBonusChange);
        heroData.WeaponReloadTimeBonusPRC.UnsubscribeFromChange(UpdateTotalWeaponReloadTimeOnBonusChange);
        heroData.WeaponRangeBonusPRC.UnsubscribeFromChange(UpdateTotalWeaponRangeOnBonusChange);
        base.OnDispose();
    }
}