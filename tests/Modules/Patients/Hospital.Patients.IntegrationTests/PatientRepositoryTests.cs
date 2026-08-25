using Hospital.Patients.Domain.Patients;
using Hospital.Patients.Infrastructure.Persistence;
using Hospital.Patients.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Hospital.Patients.IntegrationTests.Patients;

public sealed class PatientRepositoryTests
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
    public async Task AddAsync_ShouldPersistPatient()
    {
        await using var dbContext =
            CreateDbContext();

        var repository =
            new PatientRepository(dbContext);

        var externalId =
            ExternalPatientIdentifier.Create(
                "INTEGRATION_TEST",
                $"PAT-{Guid.NewGuid()}");

        var patient =
            Patient.Create(
                "Paciente Teste Integração",
                new DateOnly(1990, 1, 1),
                Gender.Male,
                externalId);

        await repository.AddAsync(patient);

        dbContext.ChangeTracker.Clear();

        var persistedPatient =
            await repository.GetByIdAsync(patient.Id);

        Assert.NotNull(persistedPatient);

        Assert.Equal(
            patient.Id,
            persistedPatient.Id);

        Assert.Equal(
            "Paciente Teste Integração",
            persistedPatient.Name);

        Assert.Equal(
            "INTEGRATION_TEST",
            persistedPatient.ExternalIdentifier!.SourceSystem);
    }

    [Fact]
    public async Task GetByExternalIdAsync_ShouldReturnPatient()
    {
        await using var dbContext =
            CreateDbContext();

        var repository =
            new PatientRepository(dbContext);

        var externalId =
            ExternalPatientIdentifier.Create(
                "INTEGRATION_TEST",
                $"EXT-{Guid.NewGuid()}");

        var patient =
            Patient.Create(
                "Paciente Busca Externa",
                new DateOnly(1988, 6, 15),
                Gender.Female,
                externalId);

        await repository.AddAsync(patient);

        dbContext.ChangeTracker.Clear();

        var result =
            await repository.GetByExternalIdAsync(
                externalId.SourceSystem,
                externalId.ExternalId);

        Assert.NotNull(result);

        Assert.Equal(
            patient.Id,
            result.Id);
    }

    [Fact]
    public async Task SearchAsync_ShouldReturnPatientByName()
    {
        await using var dbContext =
            CreateDbContext();

        var repository =
            new PatientRepository(dbContext);

        var uniqueName =
            $"Paciente Busca {Guid.NewGuid():N}";

        var patient =
            Patient.Create(
                uniqueName,
                new DateOnly(1992, 3, 20),
                Gender.Male);

        await repository.AddAsync(patient);

        dbContext.ChangeTracker.Clear();

        var result =
            await repository.SearchAsync(
                uniqueName,
                null);

        Assert.Contains(
            result,
            x => x.Id == patient.Id);
    }

    [Fact]
    public async Task SearchAsync_ShouldReturnPatientBySourceSystem()
    {
        await using var dbContext =
            CreateDbContext();

        var repository =
            new PatientRepository(dbContext);

        var uniqueExternalId =
            $"SALUX-{Guid.NewGuid():N}";

        var patient =
            Patient.Create(
                "Paciente Filtro Sistema",
                new DateOnly(1990, 4, 10),
                Gender.Male,
                ExternalPatientIdentifier.Create(
                    "SALUX",
                    uniqueExternalId));

        await repository.AddAsync(patient);

        dbContext.ChangeTracker.Clear();

        var result =
            await repository.SearchAsync(
                null,
                "SALUX");

        Assert.Contains(
            result,
            x => x.Id == patient.Id);

        Assert.DoesNotContain(
            result,
            x =>
                x.ExternalIdentifier != null &&
                x.ExternalIdentifier.SourceSystem == "API_TEST");
    }

    [Fact]
    public async Task SearchAsync_ShouldReturnPatientByNameAndSourceSystem()
    {
        await using var dbContext =
            CreateDbContext();

        var repository =
            new PatientRepository(dbContext);

        var uniqueName =
            $"Paciente Combinado {Guid.NewGuid():N}";

        var expectedPatient =
            Patient.Create(
                uniqueName,
                new DateOnly(1990, 5, 10),
                Gender.Male,
                ExternalPatientIdentifier.Create(
                    "SALUX",
                    $"EXT-{Guid.NewGuid():N}"));

        var otherPatient =
            Patient.Create(
                uniqueName,
                new DateOnly(1991, 6, 15),
                Gender.Female,
                ExternalPatientIdentifier.Create(
                    "API_TEST",
                    $"EXT-{Guid.NewGuid():N}"));

        await repository.AddAsync(expectedPatient);
        await repository.AddAsync(otherPatient);

        dbContext.ChangeTracker.Clear();

        var result =
            await repository.SearchAsync(
                uniqueName,
                "SALUX");

        Assert.Contains(
            result,
            x => x.Id == expectedPatient.Id);

        Assert.DoesNotContain(
            result,
            x => x.Id == otherPatient.Id);
    }

    [Fact]
    public async Task UpdateAsync_ShouldPersistChanges()
    {
        await using var dbContext =
            CreateDbContext();

        var repository =
            new PatientRepository(dbContext);

        var patient =
            Patient.Create(
                "Paciente Antes",
                new DateOnly(1995, 7, 12),
                Gender.Male);

        await repository.AddAsync(patient);

        patient.ChangeName(
            "Paciente Depois");

        await repository.UpdateAsync(patient);

        dbContext.ChangeTracker.Clear();

        var persisted =
            await repository.GetByIdAsync(patient.Id);

        Assert.NotNull(persisted);

        Assert.Equal(
            "Paciente Depois",
            persisted.Name);
    }

    [Fact]
    public async Task AddAsync_WithDuplicateExternalIdentifier_ShouldFail()
    {
        await using var dbContext =
            CreateDbContext();

        var repository =
            new PatientRepository(dbContext);

        var externalIdValue =
            $"DUP-{Guid.NewGuid()}";

        var patient1 =
            Patient.Create(
                "Paciente Um",
                new DateOnly(1980, 1, 1),
                Gender.Male,
                ExternalPatientIdentifier.Create(
                    "INTEGRATION_TEST",
                    externalIdValue));

        var patient2 =
            Patient.Create(
                "Paciente Dois",
                new DateOnly(1981, 1, 1),
                Gender.Female,
                ExternalPatientIdentifier.Create(
                    "INTEGRATION_TEST",
                    externalIdValue));

        await repository.AddAsync(patient1);

        await Assert.ThrowsAnyAsync<DbUpdateException>(
            () => repository.AddAsync(patient2));
    }
}