using Hospital.Patients.Application.Patients.SynchronizeExternalPatient;
using Hospital.Patients.ApplicationTests.Fakes;
using Hospital.Patients.Domain.Patients;

namespace Hospital.Patients.ApplicationTests.Patients.SynchronizeExternalPatient;

public sealed class SynchronizeExternalPatientHandlerTests
{
    [Fact]
    public async Task HandleAsync_Should_Create_Patient_When_External_Patient_Does_Not_Exist()
    {
        // Arrange
        var repository =
            new FakePatientRepository();

        var handler =
            new SynchronizeExternalPatientHandler(repository);

        var command =
            new SynchronizeExternalPatientCommand(
                "SALUX",
                "12345",
                "João da Silva",
                new DateOnly(1990, 5, 10),
                Gender.Male);

        // Act
        var result =
            await handler.HandleAsync(command);

        // Assert
        Assert.True(result.IsSuccess);

        Assert.Single(
            repository.Patients);

        var patient =
            repository.Patients.Single();

        Assert.Equal(
            result.Value,
            patient.Id);

        Assert.Equal(
            "João da Silva",
            patient.Name);

        Assert.NotNull(
            patient.ExternalIdentifier);

        Assert.Equal(
            "SALUX",
            patient.ExternalIdentifier.SourceSystem);

        Assert.Equal(
            "12345",
            patient.ExternalIdentifier.ExternalId);
    }

    [Fact]
    public async Task HandleAsync_Should_Update_Patient_When_External_Patient_Already_Exists()
    {
        // Arrange
        var repository =
            new FakePatientRepository();

        var externalIdentifier =
            ExternalPatientIdentifier.Create(
                "SALUX",
                "12345");

        var existingPatient =
            Patient.Create(
                "Nome Antigo",
                new DateOnly(1985, 3, 15),
                Gender.Male,
                externalIdentifier);

        await repository.AddAsync(
            existingPatient);

        var handler =
            new SynchronizeExternalPatientHandler(repository);

        var command =
            new SynchronizeExternalPatientCommand(
                "SALUX",
                "12345",
                "Nome Atualizado",
                new DateOnly(1986, 4, 20),
                Gender.Other);

        // Act
        var result =
            await handler.HandleAsync(command);

        // Assert
        Assert.True(result.IsSuccess);

        Assert.Single(
            repository.Patients);

        var updatedPatient =
            repository.Patients.Single();

        Assert.Equal(
            "Nome Atualizado",
            updatedPatient.Name);

        Assert.Equal(
            new DateOnly(1986, 4, 20),
            updatedPatient.BirthDate);

        Assert.Equal(
            Gender.Other,
            updatedPatient.Gender);
    }

    [Fact]
    public async Task HandleAsync_Should_Keep_Same_PatientId_When_External_Patient_Already_Exists()
    {
        // Arrange
        var repository =
            new FakePatientRepository();

        var externalIdentifier =
            ExternalPatientIdentifier.Create(
                "SALUX",
                "12345");

        var existingPatient =
            Patient.Create(
                "João da Silva",
                new DateOnly(1990, 5, 10),
                Gender.Male,
                externalIdentifier);

        var originalPatientId =
            existingPatient.Id;

        await repository.AddAsync(
            existingPatient);

        var handler =
            new SynchronizeExternalPatientHandler(repository);

        var command =
            new SynchronizeExternalPatientCommand(
                "SALUX",
                "12345",
                "João Santos",
                new DateOnly(1991, 8, 20),
                Gender.Other);

        // Act
        var result =
            await handler.HandleAsync(command);

        // Assert
        Assert.True(result.IsSuccess);

        Assert.Equal(
            originalPatientId,
            result.Value);

        Assert.Single(
            repository.Patients);

        Assert.Equal(
            originalPatientId,
            repository.Patients.Single().Id);
    }

    [Fact]
    public async Task HandleAsync_Should_Throw_When_SourceSystem_Is_Invalid()
    {
        // Arrange
        var repository =
            new FakePatientRepository();

        var handler =
            new SynchronizeExternalPatientHandler(repository);

        var command =
            new SynchronizeExternalPatientCommand(
                "",
                "12345",
                "João da Silva",
                new DateOnly(1990, 5, 10),
                Gender.Male);

        // Act
        var exception =
            await Assert.ThrowsAsync<PatientDomainException>(() =>
                handler.HandleAsync(command));

        // Assert
        Assert.Equal(
            "SourceSystem cannot be empty.",
            exception.Message);

        Assert.Empty(
            repository.Patients);
    }

    [Fact]
    public async Task HandleAsync_Should_Throw_When_ExternalId_Is_Invalid()
    {
        // Arrange
        var repository =
            new FakePatientRepository();

        var handler =
            new SynchronizeExternalPatientHandler(repository);

        var command =
            new SynchronizeExternalPatientCommand(
                "SALUX",
                "",
                "João da Silva",
                new DateOnly(1990, 5, 10),
                Gender.Male);

        // Act
        var exception =
            await Assert.ThrowsAsync<PatientDomainException>(() =>
                handler.HandleAsync(command));

        // Assert
        Assert.Equal(
            "ExternalId cannot be empty.",
            exception.Message);

        Assert.Empty(
            repository.Patients);
    }
}