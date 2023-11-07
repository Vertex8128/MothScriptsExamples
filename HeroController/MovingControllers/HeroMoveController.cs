using JoostenProductions;
using UnityEngine;

public sealed class HeroMoveController : CharacterMoveController
{
    private readonly ActiveHeroData _heroData;
    private readonly InputData _inputData;

    private readonly float _accelerationMultiplier;

    public HeroMoveController(ActiveHeroData heroData) 
        : base(heroData.HeroObjectDataKeeper.gameObject, heroData.EnteredGround)
    {
        _heroData = heroData;
        _inputData = GameData.Instance.Input;
        UpdateHeroMovingSpeedOnBonusChange(0f);
        
        _accelerationMultiplier = heroData.InitialHeroData.accelerationMultiplier;

        _heroData.HeroState.SubscribeToChange(ActivateMoveOnChangeHeroState);
        _heroData.HeroMovingSpeedBonusPRC.SubscribeToChange(UpdateHeroMovingSpeedOnBonusChange);
        GameData.Instance.GameState.SubscribeToChange(BlockHeroMovingOnGameStateChange);
    }

    private void ActivateMoveOnChangeHeroState(HeroState heroState)
    {
        switch (heroState)
        {
            case HeroState.Activated:
            case HeroState.Unfrozen:
                UpdateManager.SubscribeToFixedUpdate(ApplyVelocityOnFixedUpdate);
                break;
            
            case HeroState.Disappearing:
            case HeroState.Frozen:
                UpdateManager.UnsubscribeFromFixedUpdate(ApplyVelocityOnFixedUpdate);
                SetHeroMovingState(HeroState.Idle);
                currentMoveHandler.Move(Vector2.zero);
                break;
        }
    }

    private void UpdateHeroMovingSpeedOnBonusChange(float heroMovingSpeedBonusPRC)
    {
        _heroData.CurrentHeroMovingSpeed = Utils.GetIncreasedPercentValue(_heroData.InitialHeroData.movingSpeedDefault, heroMovingSpeedBonusPRC, 1);
    }

    private void ApplyVelocityOnFixedUpdate()
    {
        if (!currentMoveHandler) return;

        Vector2 currentVelocity;
        if (_heroData.HeroState.Value == HeroState.Dodge)
            currentVelocity = _inputData.MoveDirection.Value * (_heroData.CurrentHeroMovingSpeed * _accelerationMultiplier * Time.fixedDeltaTime);
        else
        {
            currentVelocity = _inputData.MoveDirection.Value.normalized * (_heroData.CurrentHeroMovingSpeed * Time.fixedDeltaTime);
        
            if (currentVelocity.x < 0)
                SetHeroMovingState(!_heroData.IsHeroFlipped.Value ? HeroState.MovingForward : HeroState.MovingBack);
            else if(currentVelocity.x > 0)
                SetHeroMovingState(!_heroData.IsHeroFlipped.Value ? HeroState.MovingBack : HeroState.MovingForward);
            else
                SetHeroMovingState(HeroState.Idle);
        }
        currentMoveHandler.Move(currentVelocity);
    }

    private void SetHeroMovingState(HeroState heroState)
    {
        if(_heroData.HeroState.Value == heroState) return;
        _heroData.HeroState.Value = heroState;
    }

    private void BlockHeroMovingOnGameStateChange(GameState gameState)
    {
        if(gameState != GameState.BattleFailed) return;
        currentMoveHandler.BlockMoving();
    }

    protected override void OnDispose()
    {
        _heroData.HeroState.UnsubscribeFromChange(ActivateMoveOnChangeHeroState);
        _heroData.HeroMovingSpeedBonusPRC.UnsubscribeFromChange(UpdateHeroMovingSpeedOnBonusChange);
        GameData.Instance.GameState.UnsubscribeFromChange(BlockHeroMovingOnGameStateChange);
        UpdateManager.UnsubscribeFromFixedUpdate(ApplyVelocityOnFixedUpdate);
        base.OnDispose();
    }
}