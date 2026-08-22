using Hospital.SharedKernel.Domain;

namespace Hospital.SharedKernel.UnitTests.Domain;

public sealed class AggregateRootTests
{
    [Fact]
    public void RaiseDomainEvent_Should_Add_Event()
    {
        var aggregate =
            new TestAggregateRoot(Guid.NewGuid());

        aggregate.AddEvent();

        Assert.Single(aggregate.DomainEvents);

        Assert.IsType<TestDomainEvent>(
            aggregate.DomainEvents.First());
    }

    [Fact]
    public void ClearDomainEvents_Should_Remove_All_Events()
    {
        var aggregate =
            new TestAggregateRoot(Guid.NewGuid());

        aggregate.AddEvent();

        Assert.Single(aggregate.DomainEvents);

        aggregate.ClearDomainEvents();

        Assert.Empty(aggregate.DomainEvents);
    }

    private sealed class TestAggregateRoot
        : AggregateRoot<Guid>
    {
        public TestAggregateRoot(Guid id)
            : base(id)
        {
        }

        public void AddEvent()
        {
            RaiseDomainEvent(
                new TestDomainEvent(
                    DateTimeOffset.UtcNow));
        }
    }

    private sealed record TestDomainEvent(
        DateTimeOffset OccurredOnUtc)
        : IDomainEvent;
}