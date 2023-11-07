using System.Collections.Generic;
using UnityEngine;

public sealed class AttackZoneModule : BehaviourModule
{
    private readonly AttackZoneModuleData _attackZoneModuleData;
    private readonly List<ImpactData> _interactionDataList;
    private readonly TimerController _startDelayTimer;
    private readonly TimerController _endDelayTimer;

    public AttackZoneModule(ActiveEnemyData enemyData, AttackZoneModuleData attackZoneModuleData) 
        : base(enemyData, attackZoneModuleData)
    {
        _attackZoneModuleData = attackZoneModuleData;
        var oneTimeImpactInteractionData = attackZoneModuleData.enemyZoneParamsData.damageToEnemiesPoints > 0 ? 
            Utils.GetOneTimeImpactInteractionData(CharacterID.Everyone, StatsImpactID.CurrentHealthDecrease, attackZoneModuleData.enemyZoneParamsData.damageToEnemiesPoints) : 
            Utils.GetOneTimeImpactInteractionData(CharacterID.Hero, StatsImpactID.CurrentHealthDecrease, enemyData.InitialEnemyData.baseDamage);
        _interactionDataList = new List<ImpactData> {oneTimeImpactInteractionData};
        
        var startDelay = attackZoneModuleData.startCastPosition * attackZoneModuleData.attackDuration;
        _startDelayTimer = new TimerController(StartZoneCast, false, startDelay);
        AddChildController(_startDelayTimer);
        
        var endDelay = attackZoneModuleData.attackDuration - startDelay;
        _endDelayTimer = new TimerController(StopModuleExecutionNextFrame, false, endDelay);
        AddChildController(_endDelayTimer);
    }

    public override void StartModuleExecution()
    {
        base.StartModuleExecution();
        _startDelayTimer.StartWithSetDelay();
    }

    private void StartZoneCast()
    {
        var damageZoneController = (EnemyZoneController)chargersData.GetEnemyDamageObject(_attackZoneModuleData.enemyZoneParamsData.enemyDamageObjectID);
        var casterObjectDataKeeper = (EnemyCasterObjectDataKeeper) enemyData.EnemyObjectDataKeeper;

        var  spawnPosition = casterObjectDataKeeper.hasPreparingZoneTransform 
            ? casterObjectDataKeeper.preparingZoneTransform.position
            : _attackZoneModuleData.enemyZoneParamsData.spawnPosition switch
            {
                CharacterID.Hero => heroFeetCenterTransform.position,
                CharacterID.Enemy => enemyBodyCenterTransform.position,
            };

        damageZoneController.SpawnObject(spawnPosition, _attackZoneModuleData.enemyZoneParamsData, _interactionDataList);
        _endDelayTimer.StartWithSetDelay();
        GameData.Instance.ActiveSound.Value = _attackZoneModuleData.attackSound;
    }

    public override void InterruptModuleExecution()
    {
        if(!isActive) return;
        ResetModule();
        base.InterruptModuleExecution();
    }
    
    protected override void StopModuleExecutionNextFrame()
    {
        if(!isActive) return;
        ResetModule();
        base.StopModuleExecutionNextFrame();
    }

    private void ResetModule()
    {
        _startDelayTimer.StopAndReset();
        _endDelayTimer.StopAndReset();
    }
}