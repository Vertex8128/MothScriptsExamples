using UnityEngine;

public class CharacterMoveController : BaseController
{
    private readonly GameObject _characterObject;
    protected MoveHandler currentMoveHandler;
    private readonly ISubscriptionReactiveProperty<GroundID> _enteredGround;

    protected CharacterMoveController(GameObject characterObject, ISubscriptionReactiveProperty<GroundID> enteredGround)
    {
        _characterObject = characterObject;
        currentMoveHandler = characterObject.GetOrAddComponent<RegularRigidbodyMoveHandler>();
        _enteredGround = enteredGround;
        _enteredGround.SubscribeToChange(OnEnteredGround);
    }

    private void OnEnteredGround(GroundID groundID)
    {
        GameObject.Destroy(currentMoveHandler);
        currentMoveHandler = groundID switch
        {
            GroundID.Default => _characterObject.AddComponent<RegularRigidbodyMoveHandler>(),
            GroundID.SlidingGround => _characterObject.AddComponent<SlidingRigidbodyMoveHandler>(),
            GroundID.SlowGroud => _characterObject.AddComponent<SlowRigidbodyMoveHandler>(),
            _=> _characterObject.AddComponent<RegularRigidbodyMoveHandler>()
        };
    }
    
    protected override void OnDispose()
    {
        _enteredGround.UnsubscribeFromChange(OnEnteredGround);
        base.OnDispose();
    }
}