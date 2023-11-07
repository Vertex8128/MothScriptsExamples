using UnityEngine;

public sealed class RetreatToPointModule : BehaviourModule
{
    private readonly RetreatToPointModuleData _retreatToPointModuleData;
    private float _retreatDistance;
    private float _retreatOffset;
    private int _movingModifier;

    private const int RetreatFromPositionCoefficient = 3;
    
    public RetreatToPointModule(ActiveEnemyData enemyData, RetreatToPointModuleData retreatToPointModuleData) 
        : base(enemyData, retreatToPointModuleData)
    {
        _retreatToPointModuleData = retreatToPointModuleData;
    }

    public override void StartModuleExecution()
    {
        _retreatDistance = _retreatToPointModuleData.isRandomRetreatDistance
            ? Utils.GetRandomRoundedFloat(_retreatToPointModuleData.randomRetreatDistance)
            : _retreatToPointModuleData.retreatDistance;
        
        _retreatOffset = _retreatToPointModuleData.isRandomRetreatOffset
            ? Utils.GetRandomRoundedFloat(_retreatToPointModuleData.randomRetreatOffset)
            : _retreatToPointModuleData.retreatOffset;
        
        _movingModifier = _retreatToPointModuleData.isRandomMovingModifier
            ? Utils.GetRandomIntMaxIncluded(_retreatToPointModuleData.randomMovingModifier)
            : _retreatToPointModuleData.movingModifier;

        base.StartModuleExecution();
        var enemyPathSetupData = new EnemyPathSetupData();
        switch (_retreatToPointModuleData.retreatType)
        {
            case ReatreatTypeID.AwayFromTarget:
                enemyPathSetupData.SetRetreatType(heroBodyCenterTransform.position,_retreatDistance, _retreatOffset);
                break;
            case ReatreatTypeID.CloserToTarget:
                var direction = (enemyBodyCenterTransform.position - heroBodyCenterTransform.position).normalized;
                var retreatFromPosition = enemyBodyCenterTransform.position + direction * RetreatFromPositionCoefficient;
                enemyPathSetupData.SetRetreatType(retreatFromPosition,_retreatDistance, _retreatOffset);
                break;
            case ReatreatTypeID.Random:
                enemyPathSetupData.SetRandomType(_retreatDistance, _retreatOffset);
                break;
            default:
                Debug.LogError("Retreat Path type wasn't set");
                enemyPathSetupData.SetRetreatType(heroBodyCenterTransform.position,_retreatDistance, _retreatOffset); 
                break;
        }
        enemyData.PathSetupData.Value = enemyPathSetupData;
        if(_movingModifier > 0)
            enemyData.ReceivedStatsUpdateData.Value = Utils.GetStatsUpdateData(StatsImpactID.MovingSpeedIncreasePRC, _movingModifier, true);
        
        enemyData.PathTargetReached.SubscribeToChange(StopModuleExecutionNextFrame);
    }

    public override void InterruptModuleExecution()
    {
        if (!isActive) return;
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
        enemyData.PathTargetReached.UnsubscribeFromChange(StopModuleExecutionNextFrame);
        enemyData.PathSetupData.Value = null;

        if(_movingModifier > 0)
            enemyData.ReceivedStatsUpdateData.Value = Utils.GetStatsUpdateData(StatsImpactID.MovingSpeedDecreasePRC, _movingModifier, true);
    }

    protected override void OnDispose()
    {
        enemyData.PathTargetReached.UnsubscribeFromChange(StopModuleExecutionNextFrame);
        base.OnDispose();
    }
}