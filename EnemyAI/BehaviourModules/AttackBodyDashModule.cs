using JoostenProductions;
using UnityEngine;

public sealed class AttackBodyDashModule : BehaviourModule
{
    private readonly AttackBodyDashModuleData _attackBodyDashModuleData;
    private readonly TimerController _attackTimer;
    private readonly EnemyInteractionProcessor _enemyInteractionProcessor;
    private Vector2 _movingDirection;
    private readonly Rigidbody2D _enemyRigidbody2D;

    private readonly TimerController _bounceTimer;
    private const float BounceTime = 0.1f; 

    public AttackBodyDashModule(ActiveEnemyData enemyData, AttackBodyDashModuleData attackBodyDashModuleData) 
        : base(enemyData, attackBodyDashModuleData)
    {
        _attackBodyDashModuleData = attackBodyDashModuleData;

        var acceleration = enemyData.InitialEnemyData.movingSpeed +
                           Utils.GetIncreasedPercentValue(enemyData.InitialEnemyData.movingSpeed, _attackBodyDashModuleData.acceleration, 1);
        var attackTime = _attackBodyDashModuleData.attackRange / acceleration;
        _attackTimer = new TimerController(StopModuleExecutionNextFrame, false, attackTime);
        AddChildController(_attackTimer);

        _enemyInteractionProcessor = enemyData.EnemyObjectDataKeeper.EnemyInteractionProcessor;
        _enemyRigidbody2D = enemyData.EnemyObjectDataKeeper.GetComponent<Rigidbody2D>();
        
        _bounceTimer = new TimerController(StopModuleExecutionNextFrame, false, BounceTime);
        AddChildController(_bounceTimer);
    }

    public override void StartModuleExecution()
    {
        base.StartModuleExecution();
        _enemyRigidbody2D.bodyType = RigidbodyType2D.Dynamic;
        _movingDirection = (heroBodyCenterTransform.position - enemyBodyCenterTransform.position).normalized;
        enemyData.ReceivedStatsUpdateData.Value = Utils.GetStatsUpdateData(StatsImpactID.MovingSpeedIncreasePRC, _attackBodyDashModuleData.acceleration, true);

        _enemyInteractionProcessor.SubscribeToEnterCollider(OnEnterCollider);
        UpdateManager.SubscribeToFixedUpdate(DashAttackOnFixedUpdate);
        _attackTimer.StartWithSetDelay();
        GameData.Instance.ActiveSound.Value = _attackBodyDashModuleData.attackSound;
    }

    private void DashAttackOnFixedUpdate()
    {
        enemyData.EnemyMovingDirection.Value = _movingDirection;
    }

    private void OnEnterCollider(InteractionProcessor targetInteractionProcessor)
    {
        switch (targetInteractionProcessor)
        {
            case BorderTopInteractionProcessor _:
            case BorderBottomInteractionProcessor _:
            case NoWalkGroundInteractionProcessor _:
            case ObstacleInteractionProcessor _:
                SetAndStartBounceMovement();
                break;
            case HeroBodyInteractionProcessor _:
                StopModuleExecutionNextFrame();
                break;
        } 
    }

    private void SetAndStartBounceMovement()
    {
        if(_bounceTimer.IsRunning)
            _bounceTimer.StopAndReset();
        _movingDirection = Vector2.Reflect(_movingDirection,
            _enemyInteractionProcessor.LastCollisionEnterData.GetContact(0).normal);
        _bounceTimer.StartWithSetDelay();
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
        StopModuleExecutionThisFrame();
    }

    private void ResetModule()
    {
        _attackTimer.StopAndReset();
        _enemyInteractionProcessor.UnsubscribeFromEnterTriggerCollider(OnEnterCollider);
        UpdateManager.UnsubscribeFromFixedUpdate(DashAttackOnFixedUpdate);
        
        _enemyRigidbody2D.bodyType = RigidbodyType2D.Kinematic;
        _enemyRigidbody2D.velocity = Vector2.zero;

        enemyData.ReceivedStatsUpdateData.Value = Utils.GetStatsUpdateData(StatsImpactID.MovingSpeedDecreasePRC, _attackBodyDashModuleData.acceleration, true);
    }

    protected override void OnDispose()
    {
        _enemyInteractionProcessor.UnsubscribeFromEnterCollider(OnEnterCollider);
        UpdateManager.UnsubscribeFromFixedUpdate(DashAttackOnFixedUpdate);
        base.OnDispose();
    }
}
