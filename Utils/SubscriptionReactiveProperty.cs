using System;
public class SubscriptionReactiveProperty <T> : ISubscriptionReactiveProperty<T>
{
    private T _value;
    private Action<T> _onChangeValue;
        
    public T Value
    {
        get => _value;
        set
        {
            _value = value;
            _onChangeValue?.Invoke(_value);
        }
    }

    public void SubscribeToChange(Action<T> subscriptionAction) => _onChangeValue += subscriptionAction;

    public void UnsubscribeFromChange(Action<T> unsubscriptionAction) => _onChangeValue -= unsubscriptionAction;

    public void UnsubscribeAll()
    {
        if (_onChangeValue == null) return;
        var delegateList = _onChangeValue.GetInvocationList();
        if (delegateList.Length <= 0) return; 
        foreach (var subscribedDelegate in delegateList)
            _onChangeValue -= (Action<T>) subscribedDelegate;
        _value = default;
    }
}
