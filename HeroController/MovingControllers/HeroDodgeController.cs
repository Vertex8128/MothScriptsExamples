using UnityEngine;

public sealed class HeroDodgeController : BaseController
{
    private readonly ActiveHeroData _heroData;
    private readonly GameObject _heroObject;
    private readonly GameObject _dodgePanel;
    private readonly HeroDodgePanelHandler _heroDodgePanelHandler;
    private readonly InputData _inputData;

    private readonly TimerController _accelerationMovingTimer;
    private readonly TimerController _accelerationEffectTimer;
    private readonly TimerController _restoreDodgePointTimer;

    private bool _isReadyForAcceleration;

    public HeroDodgeController(ActiveHeroData heroData)
    {
        _heroData = heroData;
        _heroObject = heroData.HeroObjectDataKeeper.gameObject;
        _dodgePanel = heroData.HeroObjectDataKeeper.dodgePanel;
        _heroDodgePanelHandler = _dodgePanel.GetComponent<HeroDodgePanelHandler>();
        _inputData = GameData.Instance.Input;

        _accelerationMovingTimer = new TimerController(StopAccelerationMoving, false, heroData.InitialHeroData.accelerationDuration);
        AddChildController(_accelerationMovingTimer);
        _accelerationEffectTimer = new TimerController(StopAccelerationEffect, false, heroData.InitialHeroData.accelerationDuration * 2f);
        AddChildController(_accelerationMovingTimer);
        _restoreDodgePointTimer = new TimerController(RestoreDodgePoint, true, heroData.InitialHeroData.dodgePointRestoreTime);
        AddChildController(_restoreDodgePointTimer);

        _isReadyForAcceleration = true;
        _dodgePanel.SetActive(false);
        _heroData.CurrentDodgePoints = 1;
        _heroData.ActiveDodgePoints = 1;

        _inputData.DodgeButton.SubscribeToChange(ApplyAccelerationOnInput);
        _heroData.HeroDodgePointsBonus.SubscribeToChange(AddPointOnBonusChange);
        _heroData.HeroState.SubscribeToChange(UpdatePanelVisibilityOnHeroStateChange);
    }
    
    private void ApplyAccelerationOnInput(ButtonState buttonState)
    {
        if(!_isReadyForAcceleration) return;
        if(_heroData.ActiveDodgePoints <= 0) return;
        if(buttonState != ButtonState.Pressed) return;
        
        _heroObject.layer = ObjectLayerName.HeroDodge;
        SetHeroMovingState(HeroState.Dodge);
        
        _heroData.ActiveDodgePoints--;
        _heroDodgePanelHandler.UpdateDodgePanel(_heroData.ActiveDodgePoints);
        
        _isReadyForAcceleration = false;
        _accelerationMovingTimer.StartWithSetDelay();
        _accelerationEffectTimer.StartWithSetDelay();
    }

    private void AddPointOnBonusChange(int heroDodgePointsBonus)
    {
        _heroData.CurrentDodgePoints = heroDodgePointsBonus + 1; // +1 - its a start point
        _heroData.ActiveDodgePoints = heroDodgePointsBonus + 1;
        _heroDodgePanelHandler.CreateAdditionalDodgePoint();
        _heroDodgePanelHandler.UpdateDodgePanel(_heroData.ActiveDodgePoints);
    }
    
    protected virtual void UpdatePanelVisibilityOnHeroStateChange(HeroState heroState)
    {
        switch (heroState)
        {
            case HeroState.Activated:
                _dodgePanel.SetActive(true);

                break;
            case HeroState.Disappearing:
            case HeroState.Defeated:
                _dodgePanel.SetActive(false);
                break;
        }
    }

    private void StopAccelerationMoving() => SetHeroMovingState(HeroState.Idle);

    private void StopAccelerationEffect()
    {
        _heroObject.layer = ObjectLayerName.Hero;
        _isReadyForAcceleration = true;
        if (_restoreDodgePointTimer.IsRunning)
        {
            _restoreDodgePointTimer.ResetCurrentTime();
            return;
        }
        _restoreDodgePointTimer.StartWithSetDelay();
    }

    private void RestoreDodgePoint()
    {
        _heroData.ActiveDodgePoints++;
        _heroDodgePanelHandler.UpdateDodgePanel(_heroData.ActiveDodgePoints);
        if(_heroData.ActiveDodgePoints < _heroData.CurrentDodgePoints) return;
        _restoreDodgePointTimer.StopAndReset();
    }

    private void SetHeroMovingState(HeroState heroState)
    {
        if(_heroData.HeroState.Value == heroState) return;
        _heroData.HeroState.Value = heroState;
    }

    protected override void OnDispose()
    {
        _inputData.DodgeButton.UnsubscribeFromChange(ApplyAccelerationOnInput);
        _heroData.HeroDodgePointsBonus.UnsubscribeFromChange(AddPointOnBonusChange);
        _heroData.HeroState.UnsubscribeFromChange(UpdatePanelVisibilityOnHeroStateChange);
        base.OnDispose();
    }
}