namespace ToDoApi.ServiceBus.Events
{
    public enum EventStates
    {
        NotPublished = 0,
        InProgress = 1,
        Published = 2,
        PublishedFailed = 3
    }
}