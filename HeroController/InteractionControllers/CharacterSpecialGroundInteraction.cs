public sealed class CharacterSpecialGroundInteraction : BaseController
{
    private readonly ISubscriptionReactiveProperty<GroundID> _enteredGround;

    public CharacterSpecialGroundInteraction(ISubscriptionReactiveProperty<GroundID> enteredGround, SpecialGroundInteractionProcessor specialGroundInteractionProcessor)
    {
        _enteredGround = enteredGround;
        _enteredGround.Value = specialGroundInteractionProcessor.groundID;
    }

    protected override void OnDispose()
    {
        _enteredGround.Value = GroundID.Default;
    }
}
