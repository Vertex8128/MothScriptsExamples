using System;
using System.Collections;
using Sirenix.OdinInspector;
using UnityEngine;

public class InteractionProcessor : MonoBehaviour
{
    public int ObjectID { get; private set; }
    public Collision2D LastCollisionEnterData { get; private set; }

    [SerializeField] [GUIColor("GetActiveToDetectCollisions")]
    protected bool isActiveToDetectCollisions;

    private Collider2D _interactionCollider;
    private event Action<InteractionProcessor> EnteredTriggerCollider;
    private event Action<InteractionProcessor> ExitedTriggerCollider;
    private event Action<InteractionProcessor> EnteredCollider;
    private event Action<InteractionProcessor> ExitedCollider;

    protected virtual void Awake()
    {
        ObjectID = gameObject.GetHashCode();
        _interactionCollider = GetComponent<Collider2D>();
    }

    protected virtual void Start() { }

    public void SubscribeToEnterTriggerCollider(Action<InteractionProcessor> onEnteredCollisionAction) => EnteredTriggerCollider += onEnteredCollisionAction;
    public void UnsubscribeFromEnterTriggerCollider(Action<InteractionProcessor> onEnteredCollisionAction) => EnteredTriggerCollider -= onEnteredCollisionAction;

    public void SubscribeToExitTriggerCollider(Action<InteractionProcessor> onExitedCollisionAction) => ExitedTriggerCollider += onExitedCollisionAction;
    public void UnsubscribeFromExitTriggerCollider(Action<InteractionProcessor> onExitedCollisionAction) => ExitedTriggerCollider -= onExitedCollisionAction;
    
    public void SubscribeToEnterCollider(Action<InteractionProcessor> onEnteredCollisionAction) => EnteredCollider += onEnteredCollisionAction;
    public void UnsubscribeFromEnterCollider(Action<InteractionProcessor> onEnteredCollisionAction) => EnteredCollider -= onEnteredCollisionAction;

    public void SubscribeToExitCollider(Action<InteractionProcessor> onExitedCollisionAction) =>  ExitedCollider += onExitedCollisionAction;
    public void UnsubscribeFromExitCollider(Action<InteractionProcessor> onExitedCollisionAction) =>  ExitedCollider -= onExitedCollisionAction;

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (isActiveToDetectCollisions && collider.TryGetComponent(out InteractionProcessor interactionProcessor))
            EnteredTriggerCollider?.Invoke(interactionProcessor);
    }

    private void OnTriggerExit2D(Collider2D collider)
    {
        if (isActiveToDetectCollisions && collider.TryGetComponent(out InteractionProcessor interactionProcessor))
            ExitedTriggerCollider?.Invoke(interactionProcessor);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (isActiveToDetectCollisions && collision.collider.gameObject.TryGetComponent(out InteractionProcessor interactionProcessor))
        {
            LastCollisionEnterData = collision;
            EnteredCollider?.Invoke(interactionProcessor);
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (isActiveToDetectCollisions && collision.collider.gameObject.TryGetComponent(out InteractionProcessor interactionProcessor))
            ExitedCollider?.Invoke(interactionProcessor);
    }

    protected void CallOnTriggerEnter2D(InteractionProcessor interactionProcessor) => EnteredTriggerCollider?.Invoke(interactionProcessor);

    public void SetDetectionColliderStatus(bool isActive)
    {
        if (!_interactionCollider) return;
        _interactionCollider.enabled = isActive;
    }

    public void SetDetectionColliderStatusForOneFrame(bool isActive)
    {
        if (!_interactionCollider) return;
        _interactionCollider.enabled = isActive;
        StartCoroutine(ResetCollider());

        IEnumerator ResetCollider()
        {
            yield return 0;
            _interactionCollider.enabled = !isActive;
        }
    }

    private Color GetActiveToDetectCollisions => isActiveToDetectCollisions? Color.white : GUIColors.Red;
}