public sealed class PreparingAttackModule : BehaviourModule
{
    private readonly PreparingAttackModuleData _preparingAttackModuleData;
    private readonly TimerController _preparationTimer;
    private readonly TimerController _indicationDelayTimer;

    public PreparingAttackModule(ActiveEnemyData enemyData, PreparingAttackModuleData preparingAttackModuleData, IBehaviourModuleData attackModuleData)
    : base(enemyData, preparingAttackModuleData)
    {
        _preparingAttackModuleData = preparingAttackModuleData;
        _preparationTimer = new TimerController(StopModuleExecutionNextFrame, false, preparingAttackModuleData.preparationDuration);
        AddChildController(_preparationTimer);

        if (!preparingAttackModuleData.hasPreparingEffect || !enemyData.EnemyObjectDataKeeper.HasPreparingAttackHandler) return;
        enemyData.EnemyObjectDataKeeper.PreparingAttackHandler.SetPreparingAttackHandler(preparingAttackModuleData, attackModuleData);
        _indicationDelayTimer = new TimerController(enemyData.EnemyObjectDataKeeper.PreparingAttackHandler.StartAttackPreparation, 
            false, preparingAttackModuleData.indicationDelay);
        AddChildController(_indicationDelayTimer);
    }

    public override void StartModuleExecution()
    {
        base.StartModuleExecution();
        _preparationTimer.StartWithSetDelay();

        if (!_preparingAttackModuleData.hasPreparingEffect || !enemyData.EnemyObjectDataKeeper.HasPreparingAttackHandler) return;
        _indicationDelayTimer.StartWithSetDelay();
        
        GameData.Instance.ActiveSound.Value = _preparingAttackModuleData.prepareAttackSound;
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
        if(enemyData.EnemyObjectDataKeeper.HasPreparingAttackHandler)
            enemyData.EnemyObjectDataKeeper.PreparingAttackHandler.StopAttackPreparation();
        _preparationTimer.StopAndReset();
    }
}
