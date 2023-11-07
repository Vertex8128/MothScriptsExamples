public sealed class IdleWaitModule : BehaviourModule
{
    private readonly TimerController _waitTimer;

    public IdleWaitModule(ActiveEnemyData enemyData, IdleWaitModuleData idleWaitModuleData) 
        : base(enemyData, idleWaitModuleData)
    {
        _waitTimer = new TimerController(StopModuleExecutionNextFrame, false, idleWaitModuleData.waitTime);
        AddChildController(_waitTimer);
    }

    public override void StartModuleExecution()
    {
        base.StartModuleExecution();
        _waitTimer.StartWithSetDelay();
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
        _waitTimer.StopAndReset();
    }
}
