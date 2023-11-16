using System.Collections.Generic;
using UnityEngine;

public class EnemyAIController : BaseController
{
    private readonly ActiveEnemyData _enemyData;
    private BehaviourModule _currentBehaviourModule;
    private readonly Dictionary<EnemyState, BehaviourModule> _behaviourModulesList;
    private readonly DefeatModule _defeatModule;
    private readonly SpawnLootModule _spawnLootModule;

    public EnemyAIController(ActiveEnemyData enemyData)
    {
        _enemyData = enemyData;
        _currentBehaviourModule = new BehaviourModule();        
        _behaviourModulesList = new Dictionary<EnemyState, BehaviourModule>();
        foreach (var behavioralData in enemyData.BehaviourModuleDataList)
        {
            switch (behavioralData)
            {
                case IdleScanModuleData idleScanModuleData:
                    var idleScanModule = new IdleScanModule(_enemyData, idleScanModuleData);
                    AddChildController(idleScanModule);
                    _behaviourModulesList.Add(idleScanModuleData.EnterID, idleScanModule);
                    break;
                
                case IdleWaitModuleData idleModuleWaitData:
                    var idleWaitModule = new IdleWaitModule(_enemyData, idleModuleWaitData);
                    AddChildController(idleWaitModule);
                    _behaviourModulesList.Add(idleModuleWaitData.EnterID, idleWaitModule);
                    break;

                case ChaseTargetModuleData chaseTargetModuleData:
                    var chaseTargetModule = new ChaseTargetModule(_enemyData, chaseTargetModuleData);
                    AddChildController(chaseTargetModule);
                    _behaviourModulesList.Add(chaseTargetModuleData.EnterID, chaseTargetModule);
                    break;

                case PreparingAttackModuleData preparingAttackModuleData:
                    var relatedAttackModuleData = enemyData.BehaviourModuleDataList.Find(attackData => attackData.EnterID == preparingAttackModuleData.ExitID);
                    var preparingAttackModule = new PreparingAttackModule(_enemyData, preparingAttackModuleData, relatedAttackModuleData);
                    AddChildController(preparingAttackModule);
                    _behaviourModulesList.Add(preparingAttackModuleData.EnterID, preparingAttackModule);
                    break;

                case AttackBodyDashModuleData attackModuleBodyDashData:
                    var attackBodyDashModule = new AttackBodyDashModule(_enemyData, attackModuleBodyDashData);
                    AddChildController(attackBodyDashModule);
                    _behaviourModulesList.Add(attackModuleBodyDashData.EnterID, attackBodyDashModule);
                    break;
                case AttackZoneModuleData attackZoneModuleData:
                    var attackZoneModule = new AttackZoneModule(_enemyData, attackZoneModuleData);
                    AddChildController(attackZoneModule);
                    _behaviourModulesList.Add(attackZoneModuleData.EnterID, attackZoneModule);
                    break;
                case AttackProjectileGunPointsModuleData attackModuleProjectileGunPointsData:
                    var attackProjectileGunPointsModule = new AttackProjectileGunPointsModule(_enemyData, attackModuleProjectileGunPointsData);
                    AddChildController(attackProjectileGunPointsModule);
                    _behaviourModulesList.Add(attackModuleProjectileGunPointsData.EnterID, attackProjectileGunPointsModule);
                    break;
                case AttackProjectileRadiusModuleData attackModuleProjectileRadiusData:
                    var attackProjectileRadiusModule = new AttackProjectileRadiusModule(_enemyData, attackModuleProjectileRadiusData);
                    AddChildController(attackProjectileRadiusModule);
                    _behaviourModulesList.Add(attackModuleProjectileRadiusData.EnterID, attackProjectileRadiusModule);
                    break;

                case RetreatToPointModuleData retreatModuleToPointData:
                    var retreatToPointModule = new RetreatToPointModule(_enemyData, retreatModuleToPointData);
                    AddChildController(retreatToPointModule);
                    _behaviourModulesList.Add(retreatModuleToPointData.EnterID, retreatToPointModule);
                    break;

                case ConditionCheckDistanceToHeroModuleData conditionCheckModuleDistanceToHeroData:
                    var conditionCheckDistanceToHeroModule = new ConditionCheckDistanceToHeroModule(_enemyData, conditionCheckModuleDistanceToHeroData);
                    AddChildController(conditionCheckDistanceToHeroModule);
                    _behaviourModulesList.Add(conditionCheckModuleDistanceToHeroData.EnterID, conditionCheckDistanceToHeroModule);
                    break;
                
                case RandomTransitionModuleData randomTransitionModuleData:
                    var randomTransitionModule = new RandomTransitionModule(_enemyData, randomTransitionModuleData);
                    AddChildController(randomTransitionModule);
                    _behaviourModulesList.Add(randomTransitionModuleData.EnterID, randomTransitionModule);
                    break;
            }
        }
        
        _defeatModule = new DefeatModule(_enemyData, enemyData.DefeatModuleData);
        AddChildController(_defeatModule);

        _spawnLootModule = new SpawnLootModule(_enemyData);
        AddChildController(_spawnLootModule);

        _enemyData.EnemyState.SubscribeToChange(UpdateModulesOnChangeEnemyState);
    }

    private void UpdateModulesOnChangeEnemyState(EnemyState enemyStateID)
    {
        switch (enemyStateID)
        {
            case EnemyState.Appearing:
                break;
            
            case EnemyState.Spawned:
                _enemyData.EnemyObjectDataKeeper.EnemyInteractionProcessor.SetDetectionColliderStatus(true);
                _enemyData.EnemyState.Value = EnemyState.IdleScanN00;
                break;
            
            case EnemyState.Defeated:
                _defeatModule.StartModuleExecution(_currentBehaviourModule);
                break;

            case EnemyState.Collapsing:
                _currentBehaviourModule.InterruptModuleExecution();
                _defeatModule.StopRunningOnDefeatModule();
                break;
            
            case EnemyState.SpawningLoot:
                _spawnLootModule.StartModuleExecution();
                break;
            
            case EnemyState.Destructed:
                break;

            default:
                if (enemyStateID == _currentBehaviourModule.ModuleID)
                {
                    Debug.LogWarning($"Module {_currentBehaviourModule.ModuleID} was called when it was running");
                    return;
                }
                
                if(!_behaviourModulesList.ContainsKey(enemyStateID))
                {
                    Debug.LogWarning($"Key {enemyStateID} wasn't found in tempModulesList");
                    return;
                }
                
                _currentBehaviourModule = _behaviourModulesList[enemyStateID];
                _currentBehaviourModule.StartModuleExecution();
                break;
        }
    }

    protected override void OnDispose()
    {
        _enemyData.EnemyState.UnsubscribeFromChange(UpdateModulesOnChangeEnemyState);
        base.OnDispose();
    }
}