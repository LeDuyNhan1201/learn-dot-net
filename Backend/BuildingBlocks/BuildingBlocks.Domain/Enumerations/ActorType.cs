namespace BuildingBlocks.Domain.Enumerations;

public enum ActorType
{
    User,
    System,
    BackgroundJob,
    KafkaConsumer,
    Scheduler,
    Unknown
}