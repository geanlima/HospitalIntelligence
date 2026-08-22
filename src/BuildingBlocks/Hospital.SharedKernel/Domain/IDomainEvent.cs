namespace Hospital.SharedKernel.Domain;

public interface IDomainEvent
{
    DateTimeOffset OccurredOnUtc { get; }
}