using Hospital.Patients.Application.Patients.SynchronizeExternalPatient;
using Hospital.Patients.Infrastructure.Persistence;
using Hospital.Patients.Infrastructure.Repositories;
using Hospital.Salux.Contracts;
using Hospital.Salux.Handlers;
using Hospital.Salux.Mappers;
using Microsoft.EntityFrameworkCore;

namespace Hospital.Salux.UnitTests.Handlers;

public sealed class SaluxPatientMessageHandlerTests
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
            new SaluxPatientMessageHandler(
                synchronizeHandler);

        var mapper =
            new SaluxPatientMapper();

        var patientCode =
            $"SALUX-{Guid.NewGuid():N}";

        var saluxPatient =
            new SaluxPatientRecord(
                patientCode,
                "Paciente Salux Integrado",
                new DateOnly(1986, 7, 15),
                1);

        var message =
            mapper.Map(saluxPatient);

        await handler.HandleAsync(message);

        dbContext.ChangeTracker.Clear();

        var persistedPatient =
            await repository.GetByExternalIdAsync(
                "SALUX",
                patientCode);

        Assert.NotNull(persistedPatient);

        Assert.Equal(
            "Paciente Salux Integrado",
            persistedPatient.Name);

        Assert.Equal(
            patientCode,
            persistedPatient.ExternalIdentifier!.ExternalId);

        Assert.Equal(
            "SALUX",
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
            new SaluxPatientMessageHandler(
                synchronizeHandler);

        var mapper =
            new SaluxPatientMapper();

        var patientCode =
            $"SALUX-{Guid.NewGuid():N}";

        var firstMessage =
            mapper.Map(
                new SaluxPatientRecord(
                    patientCode,
                    "Paciente Antes",
                    new DateOnly(1990, 2, 10),
                    1));

        await handler.HandleAsync(firstMessage);

        dbContext.ChangeTracker.Clear();

        var secondMessage =
            mapper.Map(
                new SaluxPatientRecord(
                    patientCode,
                    "Paciente Depois",
                    new DateOnly(1990, 2, 10),
                    1));

        await handler.HandleAsync(secondMessage);

        dbContext.ChangeTracker.Clear();

        var persistedPatient =
            await repository.GetByExternalIdAsync(
                "SALUX",
                patientCode);

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
            new SaluxPatientMessageHandler(
                synchronizeHandler);

        var mapper =
            new SaluxPatientMapper();

        var message =
            mapper.Map(
                new SaluxPatientRecord(
                    "SALUX-INVALID",
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
            new SaluxPatientMessageHandler(
                synchronizeHandler);

        var mapper =
            new SaluxPatientMapper();

        var message =
            mapper.Map(
                new SaluxPatientRecord(
                    $"SALUX-{Guid.NewGuid():N}",
                    "Paciente Gender Inválido",
                    new DateOnly(1990, 1, 1),
                    999));

        await Assert.ThrowsAsync<InvalidOperationException>(
            () => handler.HandleAsync(message));
    }
}