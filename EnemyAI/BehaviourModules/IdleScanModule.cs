using UnityEngine;

public sealed class IdleScanModule : BehaviourModule
{
    private readonly IdleScanModuleData _idleScanModuleData;
    private readonly TimerController _checkDistanceFrequencyTimer;

    public IdleScanModule(ActiveEnemyData enemyData, IdleScanModuleData idleScanModuleData) 
        : base(enemyData, idleScanModuleData)
    {
        _idleScanModuleData = idleScanModuleData;

        if (!idleScanModuleData.hasLimitedDetectionRadius) return;
        _checkDistanceFrequencyTimer = new TimerController(CheckDistanceToHero, true, idleScanModuleData.idleScanInterval);
        AddChildController(_checkDistanceFrequencyTimer);
    }

    public override void StartModuleExecution()
    {
        base.StartModuleExecution();
        if (_idleScanModuleData.hasLimitedDetectionRadius)
            _checkDistanceFrequencyTimer.StartWithSetDelay();
        else
            StopModuleExecutionNextFrame();
    }
    
    private void CheckDistanceToHero()
    {
        var distance = Vector2.Distance(enemyBodyCenterTransform.position, heroBodyCenterTransform.position);
        if (distance >= _idleScanModuleData.detectionRadius) return;
        StopModuleExecutionNextFrame();
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
        _checkDistanceFrequencyTimer?.StopAndReset();
    }
}
