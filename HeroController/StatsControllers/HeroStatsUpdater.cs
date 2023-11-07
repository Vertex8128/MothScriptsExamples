using UnityEngine;

public sealed class HeroStatsUpdater : BaseController
{
    private readonly ActiveHeroData _heroData;
    
    public HeroStatsUpdater(ActiveHeroData heroData)
    {
        _heroData = heroData;
        _heroData.ReceivedEquipmentData.SubscribeToChange(UpdateStatsOnReceivedReceivedEquipmentData);
        _heroData.ReceivedStatsData.SubscribeToChange(UpdateStatsOnReceivedStatsDataChange);

        //Give start drops for test;
        UpdateStatsOnReceivedStatsDataChange(new StatsData(StatsImpactID.HeroCurrentDropsIncrease, _heroData.InitialHeroData.dropsStartAmount));
    }

    private void UpdateStatsOnReceivedReceivedEquipmentData(EquipmentData equipmentData)
    {
        switch (equipmentData)
        {
            case ArtifactData artifactData:
                UpdateStatsOnReceivedStatsDataChange(artifactData.statsData);
                break;
            case WeaponData weaponData:
                _heroData.currentWeaponPower = weaponData.weaponPower;
                break;
        }
    }

    private void UpdateStatsOnReceivedStatsDataChange(StatsData statsData)
    {
        switch (statsData.statsImpactID)
        {
            case StatsImpactID.MaxHealthIncrease:
                if (statsData.isPercent) DebugDataError(statsData.statsImpactID);
                _heroData.HeroMaxHealth.Value += (int)statsData.points;
                _heroData.CurrentHeroHealth.Value += (int)statsData.points;
                break;

            case StatsImpactID.CurrentHealthIncrease:
                if (statsData.isPercent) DebugDataError(statsData.statsImpactID);
                var healthIncreasedValue = _heroData.CurrentHeroHealth.Value + (int)statsData.points;
                _heroData.CurrentHeroHealth.Value = healthIncreasedValue > _heroData.HeroMaxHealth.Value ? _heroData.HeroMaxHealth.Value : healthIncreasedValue;
                break;
            
            case StatsImpactID.CurrentHealthDecrease:
                if(_heroData.IsInvulnerable) return;
                if (statsData.isPercent) DebugDataError(statsData.statsImpactID);
                var healthDecreasedValue = _heroData.CurrentHeroHealth.Value - (int)statsData.points;
                _heroData.CurrentHeroHealth.Value = healthDecreasedValue < 0 ? 0 : healthDecreasedValue;
                break;
            
            case StatsImpactID.HeroCurrentDropsIncrease:
                if (statsData.isPercent) DebugDataError(statsData.statsImpactID);
                _heroData.CurrentHeroDrops.Value += (int)statsData.points;
                break;
            
            case StatsImpactID.HeroCurrentDropsDecrease:
                if (statsData.isPercent) DebugDataError(statsData.statsImpactID);
                var dropsDecreasedValue = _heroData.CurrentHeroDrops.Value - (int)statsData.points;
                _heroData.CurrentHeroDrops.Value = dropsDecreasedValue < 0 ? 0 : dropsDecreasedValue;
                break;
            
            case StatsImpactID.HeroDropsPickupRadiusIncreasePRC:
                if (!statsData.isPercent) DebugDataError(statsData.statsImpactID);
                _heroData.HeroDropsPickupRadiusBonusPRC.Value += statsData.points;
                break;
            
            case StatsImpactID.HeroSpawnDropBonusIncreasePRC:
                if (!statsData.isPercent) DebugDataError(statsData.statsImpactID);
                _heroData.CurrentSpawnDropBonusPRC += statsData.points;
                break;
            
            case StatsImpactID.MovingSpeedIncreasePRC:
                if (!statsData.isPercent) DebugDataError(statsData.statsImpactID);
                _heroData.HeroMovingSpeedBonusPRC.Value += statsData.points;
                break;
            
            case StatsImpactID.MovingSpeedDecreasePRC:
                if (!statsData.isPercent) DebugDataError(statsData.statsImpactID);
                _heroData.HeroMovingSpeedBonusPRC.Value -= statsData.points;
                break;

            case StatsImpactID.HeroDodgeNumberIncrease:
                if (statsData.isPercent) DebugDataError(statsData.statsImpactID);
                _heroData.HeroDodgePointsBonus.Value += (int)statsData.points;
                break;
            
            case StatsImpactID.WeaponDamageIncreasePRC:
                if (!statsData.isPercent) DebugDataError(statsData.statsImpactID);
                _heroData.WeaponDamageBonusPRC.Value += statsData.points;
                break;
            
            case StatsImpactID.WeaponMagazineSizeIncreasePRC:
                if (!statsData.isPercent) DebugDataError(statsData.statsImpactID);
                _heroData.WeaponMagazineSizeBonusPRC.Value += statsData.points;
                break;
                
            case StatsImpactID.WeaponReloadTimeDecreasePRC:
                if (!statsData.isPercent) DebugDataError(statsData.statsImpactID);
                _heroData.WeaponReloadTimeBonusPRC.Value += statsData.points;
                break;
            
            case StatsImpactID.WeaponRangeIncreasePRC:
                if (!statsData.isPercent) DebugDataError(statsData.statsImpactID);
                _heroData.WeaponRangeBonusPRC.Value += statsData.points;
                break;
        }
        
        void DebugDataError(StatsImpactID statsImpactID) => Debug.LogError("Stats Data value points percent issue - " + statsImpactID);
    }

    protected override void OnDispose()
    {
        _heroData.ReceivedEquipmentData.UnsubscribeFromChange(UpdateStatsOnReceivedReceivedEquipmentData);
        _heroData.ReceivedStatsData.UnsubscribeFromChange(UpdateStatsOnReceivedStatsDataChange);
        base.OnDispose();
    }
}