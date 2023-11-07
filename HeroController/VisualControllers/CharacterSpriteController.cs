public class CharacterSpriteController : BaseController
{
    protected readonly CharacterLayerSetter characterLayerSetter;
    private readonly ISubscriptionReactiveProperty<bool> _isCharacterFlipped;
    protected readonly TimerController updateSortingLayerOrderTimer;

    protected CharacterSpriteController(CharacterLayerSetter characterLayerSetter, ISubscriptionReactiveProperty<bool> isCharacterFlipped)
    {
        this.characterLayerSetter = characterLayerSetter;
        this.characterLayerSetter.UpdateSortingLayer();
        
        _isCharacterFlipped = isCharacterFlipped;
        _isCharacterFlipped.SubscribeToChange(FlipSpriteOnChangeIsCharacterFlipped);

        updateSortingLayerOrderTimer = new TimerController(UpdateLayerOrder, true, GameData.Instance.GameParams.sortingLayerUpdateRate);
        AddChildController(updateSortingLayerOrderTimer);
        updateSortingLayerOrderTimer.StartWithSetDelay();
    }

    private void UpdateLayerOrder()
    {
        characterLayerSetter.UpdateSortingLayer();
    }

    private void FlipSpriteOnChangeIsCharacterFlipped(bool isFlipped) => characterLayerSetter.FlipSprite();

    protected override void OnDispose()
    {
        _isCharacterFlipped.UnsubscribeFromChange(FlipSpriteOnChangeIsCharacterFlipped);
        base.OnDispose();
    }
}
