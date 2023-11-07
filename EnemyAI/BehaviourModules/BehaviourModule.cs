using System.Collections;
using UnityEngine;

public class BehaviourModule : BaseController
{
    public EnemyState ModuleID { get; }
    protected bool isActive;
    
    protected readonly ActiveChargersData chargersData;
    protected readonly ActiveEnemyData enemyData;
    
    protected readonly Transform enemyBodyCenterTransform;
    protected readonly Transform enemyFeetCenterTransform;
    protected readonly Transform heroBodyCenterTransform;
    protected readonly Transform heroFeetCenterTransform;

    private readonly IBehaviourModuleData _behaviourModuleData;

    private EnemyState _exitEnemyState;

    public BehaviourModule()
    {
        ModuleID = EnemyState.None;
    }
    
    protected BehaviourModule(ActiveEnemyData activeEnemyData, IBehaviourModuleData behaviourModuleData)
    {
        ModuleID = behaviourModuleData.EnterID;
        chargersData = GameData.Instance.ChargersData;
        enemyData = activeEnemyData;
        
        enemyBodyCenterTransform = activeEnemyData.EnemyObjectDataKeeper.bodyCenterTransform;
        enemyFeetCenterTransform = activeEnemyData.EnemyObjectDataKeeper.FeetCenterTransform;
        var heroDataKeeper = GameData.Instance.HeroData.HeroObjectDataKeeper;
        heroBodyCenterTransform = heroDataKeeper.bodyCenterTransform;
        heroFeetCenterTransform = heroDataKeeper.FeetCenterTransform;
        
        _behaviourModuleData = behaviourModuleData;
    }
    
    public virtual void StartModuleExecution()
    {
        isActive = true;
        enemyData.IsInvulnerable = enemyData.InitialEnemyData.hasInvulnerability && _behaviourModuleData.IsInvulnerable;
        enemyData.IsRotating.Value = _behaviourModuleData.IsRotating;
        enemyData.IsEyesMonitoring.Value = _behaviourModuleData.IsEyesMonitoring;
    }

    public virtual void InterruptModuleExecution() => isActive = false;
    
    protected void StopModuleExecutionThisFrame()
    {
        isActive = false;
        enemyData.EnemyState.Value = _behaviourModuleData.ExitID;
    }
    
    protected virtual void StopModuleExecutionNextFrame()
    {
        _exitEnemyState = _behaviourModuleData.ExitID;
        GameData.Instance.Keeper.StartCoroutine(ExitModule());
    }
    
    private IEnumerator ExitModule()
    {
        yield return 0;
        isActive = false;
        enemyData.EnemyState.Value = _exitEnemyState;
    }
}