using UnityEngine;

public sealed class HeroAnimationController : BaseController
{
    private readonly ActiveHeroData _heroData;
    private readonly GameData _gameData;
    private readonly Animator _heroAnimator;

    private static readonly int AnimationStateHash = Animator.StringToHash("animationState");
    private static readonly int BattleEyesHash = Animator.StringToHash("setBattleEyes");
    private static readonly int DefaultEyesHash = Animator.StringToHash("setDefaultEyes");
    private static readonly int DefeatedEyesHash = Animator.StringToHash("defeated");

    public HeroAnimationController(ActiveHeroData heroData)
    {
        _heroData = heroData;
        _gameData = GameData.Instance;
        _heroAnimator = heroData.HeroObjectDataKeeper.CharacterAnimator;

        _heroData.HeroState.SubscribeToChange(OnChangeHeroState);
        
        _gameData.LevelState.SubscribeToChange(SetArenaDataOnLevelStateChange);
    }

    private void OnChangeHeroState(HeroState heroState)
    {
        switch(heroState)
        {
            case HeroState.Activated:
                _heroData.HeroState.Value = HeroState.Idle;
                break;
            case HeroState.Idle:
            case HeroState.Frozen:
                SetAnimationState(HeroAnimationState.Idle); break;
            case HeroState.MovingForward:
                SetAnimationState(HeroAnimationState.MovingForward); break;
            case HeroState.MovingBack:
                SetAnimationState(HeroAnimationState.MovingBack); break;
            case HeroState.Defeated:
                SetAnimationState(HeroAnimationState.Defeated); 
                _heroData.HeroState.UnsubscribeFromChange(OnChangeHeroState);
                break;
        }
    }

    private void SetArenaDataOnLevelStateChange(LevelState levelState)
    {
        if(levelState != LevelState.ArenaStarting) return;
        _gameData.LevelData.CurrentArenaData.ArenaState.SubscribeToChange(SetHeroEyesOnChangeArenaState);
    }

    private void SetHeroEyesOnChangeArenaState(ArenaState arenaState)
    {
        switch (arenaState)
        {
            case ArenaState.WaveSpawningGoing:
                _heroAnimator.SetTrigger(BattleEyesHash);
                break;
            case ArenaState.WaveCompleted:
                _heroAnimator.SetTrigger(DefaultEyesHash);
                break;
            case ArenaState.AllWavesCompleted:
                _gameData.LevelData.CurrentArenaData.ArenaState.UnsubscribeFromChange(SetHeroEyesOnChangeArenaState);
                break;
        }
    }

    private void SetAnimationState(HeroAnimationState newAnimationState)
    {
        if (newAnimationState == HeroAnimationState.Defeated)
        {
            _heroAnimator.SetTrigger(DefeatedEyesHash);
            return;
        }
        
        _heroAnimator.SetInteger(AnimationStateHash, (int)newAnimationState);
    }

    protected override void OnDispose()
    {
        _gameData.LevelState.UnsubscribeFromChange(SetArenaDataOnLevelStateChange);
        _gameData.LevelData.CurrentArenaData.ArenaState.UnsubscribeFromChange(SetHeroEyesOnChangeArenaState);
        _heroData.HeroState.UnsubscribeFromChange(OnChangeHeroState);
        base.OnDispose();
    }
}