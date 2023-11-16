using UnityEngine;

public class AttackProjectileModule : BehaviourModule
{
    private readonly AttackProjectileModuleData _attackProjectileModuleData;
    protected readonly Transform centralGunPointTransform;
    private readonly TimerController _startDelayTimer;
    private readonly TimerController _endDelayTimer;
    private readonly TimerController _projectileInWaveDelayTimer;
    private int _projectileCounter;

    protected AttackProjectileModule(ActiveEnemyData enemyData, AttackProjectileModuleData attackProjectileModuleData) 
        : base(enemyData, attackProjectileModuleData)
    {
        _attackProjectileModuleData = attackProjectileModuleData;

        if (attackProjectileModuleData.aimingDirectionID == AimingDirectionID.GunPointToCenterRelated)
            centralGunPointTransform = ((EnemyShooterObjectDataKeeper)enemyData.EnemyObjectDataKeeper).CentralGunPointTransform;

        var startDelay = attackProjectileModuleData.startCastPosition * attackProjectileModuleData.attackDuration;
        _startDelayTimer = new TimerController(StartProjectileCast, false, startDelay);
        AddChildController(_startDelayTimer);
        
        var endDelay = attackProjectileModuleData.attackDuration - startDelay;
        _endDelayTimer = new TimerController(StopModuleExecutionNextFrame, false, endDelay);
        AddChildController(_endDelayTimer);

        if (attackProjectileModuleData.projectileNumbers <= 0) return;
        _projectileInWaveDelayTimer = new TimerController(CastProjectileInWave, true, _attackProjectileModuleData.projectileDelay);
        AddChildController(_projectileInWaveDelayTimer);
    }

    public override void StartModuleExecution()
    {
        base.StartModuleExecution();
        _startDelayTimer.StartWithSetDelay();
    }

    private void StartProjectileCast()
    {

        if (_attackProjectileModuleData.projectileNumbers > 1)
            _projectileInWaveDelayTimer.StartWithAction();
        else
            CastProjectileInWave();
    }

    private void CastProjectileInWave()
    {
        CastProjectile();
        if (_attackProjectileModuleData.playForEach)
        {
            if(Utils.CheckChanceIsTrue(_attackProjectileModuleData.chanceToPlaySound)) 
                GameData.Instance.ActiveSound.Value = _attackProjectileModuleData.attackSound;
        }
        else if (_projectileCounter == 0)
            GameData.Instance.ActiveSound.Value = _attackProjectileModuleData.attackSound;
        _projectileCounter++;
        
        if (_projectileCounter < _attackProjectileModuleData.projectileNumbers) return;
        _projectileInWaveDelayTimer.StopAndReset();
        ResetCastValues();
        _endDelayTimer.StartWithSetDelay();
    }

    protected virtual void CastProjectile(){}
    protected virtual void ResetCastValues(){}

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
        _projectileInWaveDelayTimer?.StopAndReset();
        _projectileCounter = 0;
    }
}