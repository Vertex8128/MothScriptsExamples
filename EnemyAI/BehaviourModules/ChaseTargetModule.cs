using UnityEngine;

public sealed class ChaseTargetModule : BehaviourModule
{
    private readonly ChaseTargetModuleData _chaseTargetModuleData;
    private float _distanceToStop;
    private int _movingModifier;

    public ChaseTargetModule(ActiveEnemyData enemyData, ChaseTargetModuleData chaseTargetModuleData) 
        : base(enemyData, chaseTargetModuleData)
    {
        _chaseTargetModuleData = chaseTargetModuleData;
    }
    
    public override void StartModuleExecution()
    {
        _distanceToStop = _chaseTargetModuleData.isRandomDistanceToStop
            ? Utils.GetRandomRoundedFloat(_chaseTargetModuleData.randomDistanceToStop)
            : _chaseTargetModuleData.distanceToStop;

        _movingModifier = _chaseTargetModuleData.isRandomMovingModifier
            ? Utils.GetRandomIntMaxIncluded(_chaseTargetModuleData.randomMovingModifier)
            : _chaseTargetModuleData.movingModifier;

        if (!_chaseTargetModuleData.targetMustBeVisible)
        {
            var distanceToTarget = Vector2.Distance(enemyFeetCenterTransform.position, heroFeetCenterTransform.position);
            if (distanceToTarget < _distanceToStop)
            {
                base.StopModuleExecutionNextFrame();
                return;
            }
        }

        base.StartModuleExecution();
        var enemyPathSetupData = new EnemyPathSetupData();
        enemyPathSetupData.SetChasingType(heroBodyCenterTransform, _distanceToStop, _chaseTargetModuleData.targetMustBeVisible, _chaseTargetModuleData.spreadCoefficient);
        enemyData.PathSetupData.Value = enemyPathSetupData;
        if(_movingModifier > 0)
            enemyData.ReceivedStatsUpdateData.Value = Utils.GetStatsUpdateData(StatsImpactID.MovingSpeedIncreasePRC, _movingModifier, true);
        
        enemyData.PathTargetReached.SubscribeToChange(StopModuleExecutionNextFrame);
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
