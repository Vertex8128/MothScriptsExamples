public sealed class DefeatModule : BaseController
{
    private readonly ActiveEnemyData _enemyData;
    private readonly DefeatModuleData _defeatModuleData;
    private BehaviourModule _runningOnDefeatModule;

    public DefeatModule(ActiveEnemyData enemyData, DefeatModuleData defeatModuleData)
    {
        _enemyData = enemyData;
        _defeatModuleData = defeatModuleData;
    }

    public void StartModuleExecution(BehaviourModule currentBehaviourModule)
    {
        _enemyData.IsInvulnerable = true;

        if (_defeatModuleData.stopActiveModule)
            currentBehaviourModule.InterruptModuleExecution();
        else
            _runningOnDefeatModule = currentBehaviourModule;
        GameData.Instance.ActiveSound.Value = SoundID.EnemyDeath;
        _enemyData.EnemyState.Value = _defeatModuleData.exitID;
    }

    public void StopRunningOnDefeatModule()
    {
        _runningOnDefeatModule?.InterruptModuleExecution();
    }
}
