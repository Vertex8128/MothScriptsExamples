using System.Collections.Generic;

public sealed class HeroInteractionController : BaseController
{
    private readonly ActiveHeroData _heroData;
    private readonly HeroBodyInteractionProcessor _heroBodyInteractionProcessor;
    private readonly HeroFootInteractionProcessor _heroFootInteractionProcessor;
    private readonly Dictionary<int, BaseController> _activeInteractionsList;
    private BaseController _specialGroundInteraction;
    private readonly CharacterStatsStatusUpdateInteraction _characterStatsStatusUpdateInteraction;

    public HeroInteractionController(ActiveHeroData heroData)
    {
        _heroData = heroData;
        _heroBodyInteractionProcessor = heroData.HeroObjectDataKeeper.HeroBodyInteractionProcessor;
        _heroFootInteractionProcessor = heroData.HeroObjectDataKeeper.HeroFootInteractionProcessor;
        _activeInteractionsList = new Dictionary<int, BaseController>();
        _characterStatsStatusUpdateInteraction = new CharacterStatsStatusUpdateInteraction(_heroData.ReceivedStatsData, _heroData.ReceivedStatusData);
        
        _heroBodyInteractionProcessor.SubscribeToEnterTriggerCollider(ProcessHeroInteractionOnCollisionEnter);
        _heroBodyInteractionProcessor.SubscribeToEnterCollider(ProcessHeroInteractionOnCollisionEnter);
        _heroBodyInteractionProcessor.SubscribeToExitTriggerCollider(ProcessHeroInteractionOnCollisionExit);
        
        _heroFootInteractionProcessor.SubscribeToEnterTriggerCollider(ProcessHeroGroundInteractionOnCollisionEnter);
        _heroFootInteractionProcessor.SubscribeToExitTriggerCollider(ProcessHeroGroundInteractionOnCollisionExit);
    }

    private void ProcessHeroInteractionOnCollisionEnter(InteractionProcessor targetInteractionProcessor)
    {
        if(_heroData.HeroState.Value == HeroState.Deactivated) return;
        if(_activeInteractionsList.ContainsKey(targetInteractionProcessor.ObjectID)) return;

        switch (targetInteractionProcessor)
        {
            case EqipmentHolderInteractionProcessor equipmentHolderInteractionProcessor:
                _activeInteractionsList.Add(targetInteractionProcessor.ObjectID, 
                    new HeroEquipmentHolderInteraction(equipmentHolderInteractionProcessor));
                break;
            
            case RerollHolderInteractionProcessor rerollHolderInteractionProcessor:
                _activeInteractionsList.Add(targetInteractionProcessor.ObjectID, 
                    new HeroRerollHolderInteraction(rerollHolderInteractionProcessor));
                break;

            case WaveStarterInteractionProcessor _:
                _activeInteractionsList.Add(targetInteractionProcessor.ObjectID, 
                    new HeroWaveStarterInteraction());
                break;
            
            case DoorInteractionProcessor _:
                _activeInteractionsList.Add(targetInteractionProcessor.ObjectID, 
                    new HeroDoorInteraction());
                break;
            
            case DropInteractionProcessor _:
            case EnemyInteractionProcessor _:
            case EnemyDamageObjectInteractionProcessor _:
            case EveryoneDamageObjectInteractionProcessor _:
                _characterStatsStatusUpdateInteraction.ProcessInteraction(targetInteractionProcessor);
                return;
        }
    }

    private void ProcessHeroInteractionOnCollisionExit(InteractionProcessor targetInteractionProcessor)
    {
        if(_heroData.HeroState.Value == HeroState.Deactivated) return;
        if(!_activeInteractionsList.ContainsKey(targetInteractionProcessor.ObjectID)) return;

        _activeInteractionsList[targetInteractionProcessor.ObjectID].Dispose();
        _activeInteractionsList.Remove(targetInteractionProcessor.ObjectID);
    }

    private void ProcessHeroGroundInteractionOnCollisionEnter(InteractionProcessor targetInteractionProcessor)
    {
        if(_heroData.HeroState.Value == HeroState.Deactivated) return;
        if(_specialGroundInteraction != null) return;
        if (targetInteractionProcessor is SpecialGroundInteractionProcessor specialGroundInteractionHandler)
            _specialGroundInteraction = new CharacterSpecialGroundInteraction(_heroData.EnteredGround, specialGroundInteractionHandler);
    }

    private void ProcessHeroGroundInteractionOnCollisionExit(InteractionProcessor targetInteractionProcessor)
    {
        if(_heroData.HeroState.Value == HeroState.Deactivated) return;
        if (targetInteractionProcessor is not SpecialGroundInteractionProcessor) return;
        if(_specialGroundInteraction == null) return;
        _specialGroundInteraction.Dispose();
        _specialGroundInteraction = null;
    }

    protected override void OnDispose()
    {
        _heroBodyInteractionProcessor.UnsubscribeFromEnterTriggerCollider(ProcessHeroInteractionOnCollisionEnter);
        _heroBodyInteractionProcessor.UnsubscribeFromEnterCollider(ProcessHeroInteractionOnCollisionEnter);
        _heroBodyInteractionProcessor.UnsubscribeFromExitTriggerCollider(ProcessHeroInteractionOnCollisionExit);
        _heroFootInteractionProcessor.UnsubscribeFromEnterTriggerCollider(ProcessHeroGroundInteractionOnCollisionEnter);
        _heroFootInteractionProcessor.UnsubscribeFromExitTriggerCollider(ProcessHeroGroundInteractionOnCollisionExit);
        foreach (var interaction in _activeInteractionsList)
            interaction.Value?.Dispose();
        base.OnDispose();
    }
}