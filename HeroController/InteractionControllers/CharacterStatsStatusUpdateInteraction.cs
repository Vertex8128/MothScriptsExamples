public sealed class CharacterStatsStatusUpdateInteraction : BaseController
{
    private readonly ISubscriptionReactiveProperty<StatsData> _receivedStatsData;
    private readonly ISubscriptionReactiveProperty<StatusData> _receivedStatusData;
    
    public CharacterStatsStatusUpdateInteraction(ISubscriptionReactiveProperty<StatsData> receivedStatsData, 
        ISubscriptionReactiveProperty<StatusData> receivedStatusData)
    {
        _receivedStatsData = receivedStatsData;
        _receivedStatusData = receivedStatusData;
    }

    public void ProcessInteraction(InteractionProcessor targetInteractionProcessor)
    {
        if (!(targetInteractionProcessor is ImpactDataInteractionProcessor statsUpdateInteractionHandler)) return;

        foreach (var interactionData in statsUpdateInteractionHandler.InteractionDataList)
        {
            switch (interactionData.impactTypeID)
            {
                case ImpactTypeID.OneTimeImpact:
                    _receivedStatsData.Value = interactionData.statsData;
                    break;
                case ImpactTypeID.Status when _receivedStatusData.Value == null:
                    _receivedStatusData.Value = interactionData.statusData;
                    break;
            }
        }
    }
}