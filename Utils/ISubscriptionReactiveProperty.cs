using System;

public interface ISubscriptionReactiveProperty <T>
{
    T Value { get; set; }
    public void SubscribeToChange(Action<T> subscriptionAction);
    public void UnsubscribeFromChange(Action<T> unsubscriptionAction);
}
