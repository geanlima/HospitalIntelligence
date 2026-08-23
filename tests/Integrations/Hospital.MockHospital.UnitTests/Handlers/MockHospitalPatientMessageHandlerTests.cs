using Hospital.MockHospital.Contracts;
using Hospital.MockHospital.Handlers;
using Hospital.MockHospital.Mappers;
using Hospital.Patients.Application.Patients.SynchronizeExternalPatient;
using Hospital.Patients.Infrastructure.Persistence;
using Hospital.Patients.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Hospital.MockHospital.UnitTests.Handlers;

public sealed class MockHospitalPatientMessageHandlerTests
{
    private const string ConnectionString =
        "Host=localhost;Port=5432;Database=hospital_intelligence;Username=postgres;Password=postgres";

    private static PatientsDbContext CreateDbContext()
    {
        var options =
            new DbContextOptionsBuilder<PatientsDbContext>()
                .UseNpgsql(ConnectionString)
                .Options;

        return new PatientsDbContext(options);
    }

    [Fact]
    public async Task HandleAsync_ShouldSynchronizePatientIntoDatabase()
    {
        await using var dbContext =
            CreateDbContext();

        var repository =
            new PatientRepository(dbContext);

        var synchronizeHandler =
            new SynchronizeExternalPatientHandler(repository);

        var handler =
            new MockHospitalPatientMessageHandler(
                synchronizeHandler);

        var mapper =
            new MockHospitalPatientMapper();

        var externalId =
            $"MOCK-{Guid.NewGuid():N}";

        var externalPatient =
            new MockHospitalPatientMessage(
                externalId,
                "Paciente Mock Hospital",
                new DateOnly(1985, 6, 20),
                1);

        var integrationMessage =
            mapper.Map(externalPatient);

        await handler.HandleAsync(
            integrationMessage);

        dbContext.ChangeTracker.Clear();

        var persistedPatient =
            await repository.GetByExternalIdAsync(
                "MOCK_HOSPITAL",
                externalId);

        Assert.NotNull(persistedPatient);

        Assert.Equal(
            "Paciente Mock Hospital",
            persistedPatient.Name);

        Assert.Equal(
            externalId,
            persistedPatient.ExternalIdentifier!.ExternalId);

        Assert.Equal(
            "MOCK_HOSPITAL",
            persistedPatient.ExternalIdentifier.SourceSystem);
    }

    [Fact]
    public async Task HandleAsync_WhenPatientAlreadyExists_ShouldUpdatePatient()
    {
        await using var dbContext =
            CreateDbContext();

        var repository =
            new PatientRepository(dbContext);

        var synchronizeHandler =
            new SynchronizeExternalPatientHandler(repository);

        var handler =
            new MockHospitalPatientMessageHandler(
                synchronizeHandler);

        var mapper =
            new MockHospitalPatientMapper();

        var externalId =
            $"MOCK-{Guid.NewGuid():N}";

        var firstMessage =
            mapper.Map(
                new MockHospitalPatientMessage(
                    externalId,
                    "Paciente Antes",
                    new DateOnly(1990, 1, 10),
                    1));

        await handler.HandleAsync(
            firstMessage);

        dbContext.ChangeTracker.Clear();

        var secondMessage =
            mapper.Map(
                new MockHospitalPatientMessage(
                    externalId,
                    "Paciente Depois",
                    new DateOnly(1990, 1, 10),
                    1));

        await handler.HandleAsync(
            secondMessage);

        dbContext.ChangeTracker.Clear();

        var persistedPatient =
            await repository.GetByExternalIdAsync(
                "MOCK_HOSPITAL",
                externalId);

        Assert.NotNull(persistedPatient);

        Assert.Equal(
            "Paciente Depois",
            persistedPatient.Name);
    }

    [Fact]
    public async Task HandleAsync_WithInvalidSourceSystem_ShouldThrow()
    {
        await using var dbContext =
            CreateDbContext();

        var repository =
            new PatientRepository(dbContext);

        var synchronizeHandler =
            new SynchronizeExternalPatientHandler(repository);

        var handler =
            new MockHospitalPatientMessageHandler(
                synchronizeHandler);

        var mapper =
            new MockHospitalPatientMapper();

        var message =
            mapper.Map(
                new MockHospitalPatientMessage(
                    "PAC-INVALID",
                    "Paciente Inválido",
                    new DateOnly(1990, 1, 1),
                    1));

        var invalidMessage =
            message with
            {
                SourceSystem = "OTHER_SYSTEM"
            };

        await Assert.ThrowsAsync<InvalidOperationException>(
            () => handler.HandleAsync(invalidMessage));
    }

    [Fact]
    public async Task HandleAsync_WithInvalidGender_ShouldThrow()
    {
        await using var dbContext =
            CreateDbContext();

        var repository =
            new PatientRepository(dbContext);

        var synchronizeHandler =
            new SynchronizeExternalPatientHandler(repository);

        var handler =
            new MockHospitalPatientMessageHandler(
                synchronizeHandler);

        var mapper =
            new MockHospitalPatientMapper();

        var message =
            mapper.Map(
                new MockHospitalPatientMessage(
                    $"MOCK-{Guid.NewGuid():N}",
                    "Paciente Gender Inválido",
                    new DateOnly(1990, 1, 1),
                    999));

        await Assert.ThrowsAsync<InvalidOperationException>(
            () => handler.HandleAsync(message));
    }
}