using JoostenProductions;
using UnityEngine;

public sealed class HeroAimController : BaseController
{
    private readonly ISubscriptionReactiveProperty<InputTypeID> _inputType;
    private readonly ISubscriptionReactiveProperty<GameState> _gameState;
    private readonly ISubscriptionReactiveProperty<HeroState> _heroState;
    private readonly GameObject _heroAim;
    private readonly SpriteRenderer _heroAimSpriteRenderer;
    private readonly Transform _aimHolderTransform;
    
    private bool _isAimActive;

    public HeroAimController(ActiveHeroData heroData)
    {
        var gameData = GameData.Instance;
        _inputType = gameData.Input.InputType;
        _gameState = gameData.GameState;
        _heroState = heroData.HeroState;

        _heroAim = GameObject.Instantiate(GameData.Instance.Prefabs.heroAimPrefab, heroData.HeroObjectDataKeeper.transform);
        AddGameObject(_heroAim);
        _heroAimSpriteRenderer = _heroAim.GetComponent<SpriteRenderer>();
        
        _heroAim.SetActive(false);
        _heroAimSpriteRenderer.enabled = false;
        
        _aimHolderTransform = heroData.HeroObjectDataKeeper.aimHolder.transform;

        UpdateAimVisibilityOnInputTypeChange(_inputType.Value);

        _inputType.SubscribeToChange(UpdateAimVisibilityOnInputTypeChange);
        _gameState.SubscribeToChange(UpdateAimSpriteVisibilityOnGamePause);
        _heroState.SubscribeToChange(UpdateAimSpriteVisibilityOnHeroStateChange);
        UpdateManager.SubscribeToUpdate(SetAimPositionOnUpdate);
    }
    
    private void UpdateAimVisibilityOnInputTypeChange(InputTypeID inputType) // Object
    {
        switch (inputType)
        {
            case InputTypeID.KeyboardMouse:
                _heroAim.SetActive(false);
                _isAimActive = false;
                break;

            case InputTypeID.Xbox:
                _heroAim.SetActive(true);
                _isAimActive = true;
                break;
        }
    }

    private void UpdateAimSpriteVisibilityOnHeroStateChange(HeroState heroState)
    {
        switch (heroState)
        {
            case HeroState.Activated:
            case HeroState.Unfrozen:
                _heroAimSpriteRenderer.enabled = true;
                break;

            case HeroState.Disappearing:
            case HeroState.Frozen:
                _heroAimSpriteRenderer.enabled = false;
                break;
        }
    }
    
    private void UpdateAimSpriteVisibilityOnGamePause(GameState gameState)
    {
        switch (gameState)
        {
            case GameState.BattlePaused:
            case GameState.BattleWon:
            case GameState.BattleFailed:
                _heroAimSpriteRenderer.enabled = false;
                break;

            case GameState.BattleUnpaused:
                _heroAimSpriteRenderer.enabled = true;
                break;
        }
    }

    private void SetAimPositionOnUpdate()
    {
        if(!_isAimActive) return;
        _heroAim.transform.position = _aimHolderTransform.transform.position;
    } 

    protected override void OnDispose()
    {
        _inputType.UnsubscribeFromChange(UpdateAimVisibilityOnInputTypeChange);
        _gameState.UnsubscribeFromChange(UpdateAimSpriteVisibilityOnGamePause);
        _heroState.UnsubscribeFromChange(UpdateAimSpriteVisibilityOnHeroStateChange);
        UpdateManager.UnsubscribeFromUpdate(SetAimPositionOnUpdate);
        base.OnDispose();
    }
}