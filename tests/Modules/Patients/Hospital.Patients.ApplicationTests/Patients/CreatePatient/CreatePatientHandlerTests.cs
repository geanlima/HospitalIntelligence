using Hospital.Patients.Application.Patients.CreatePatient;
using Hospital.Patients.ApplicationTests.Fakes;
using Hospital.Patients.Domain.Patients;

namespace Hospital.Patients.ApplicationTests.Patients.CreatePatient;

public sealed class CreatePatientHandlerTests
{
    [Fact]
    public async Task HandleAsync_Should_Create_Patient_When_Command_Is_Valid()
    {
        // Arrange
        var repository =
            new FakePatientRepository();

        var handler =
            new CreatePatientHandler(repository);

        var command =
            new CreatePatientCommand(
                "João da Silva",
                new DateOnly(1990, 5, 10),
                Gender.Male);

        // Act
        var result =
            await handler.HandleAsync(command);

        // Assert
        Assert.True(result.IsSuccess);

        Assert.Single(repository.Patients);

        var patient =
            repository.Patients.Single();

        Assert.Equal(
            "João da Silva",
            patient.Name);

        Assert.Equal(
            result.Value,
            patient.Id);
    }

    [Fact]
    public async Task HandleAsync_Should_Return_Failure_When_External_Identifier_Already_Exists()
    {
        // Arrange
        var repository =
            new FakePatientRepository();

        var existingIdentifier =
            ExternalPatientIdentifier.Create(
                "SALUX",
                "12345");

        var existingPatient =
            Patient.Create(
                "Paciente Existente",
                new DateOnly(1985, 3, 15),
                Gender.Male,
                existingIdentifier);

        await repository.AddAsync(
            existingPatient);

        var handler =
            new CreatePatientHandler(repository);

        var command =
            new CreatePatientCommand(
                "Novo Paciente",
                new DateOnly(1990, 5, 10),
                Gender.Female,
                "SALUX",
                "12345");

        // Act
        var result =
            await handler.HandleAsync(command);

        // Assert
        Assert.True(result.IsFailure);

        Assert.Equal(
            "Patient.ExternalIdentifier.AlreadyExists",
            result.Error.Code);

        Assert.Single(
            repository.Patients);
    }

    [Fact]
    public async Task HandleAsync_Should_Return_Failure_When_SourceSystem_Is_Provided_Without_ExternalId()
    {
        // Arrange
        var repository =
            new FakePatientRepository();

        var handler =
            new CreatePatientHandler(repository);

        var command =
            new CreatePatientCommand(
                "João da Silva",
                new DateOnly(1990, 5, 10),
                Gender.Male,
                "SALUX",
                null);

        // Act
        var result =
            await handler.HandleAsync(command);

        // Assert
        Assert.True(result.IsFailure);

        Assert.Equal(
            "Patient.ExternalIdentifier.Invalid",
            result.Error.Code);

        Assert.Empty(
            repository.Patients);
    }

    [Fact]
    public async Task HandleAsync_Should_Return_Failure_When_ExternalId_Is_Provided_Without_SourceSystem()
    {
        // Arrange
        var repository =
            new FakePatientRepository();

        var handler =
            new CreatePatientHandler(repository);

        var command =
            new CreatePatientCommand(
                "João da Silva",
                new DateOnly(1990, 5, 10),
                Gender.Male,
                null,
                "12345");

        // Act
        var result =
            await handler.HandleAsync(command);

        // Assert
        Assert.True(result.IsFailure);

        Assert.Equal(
            "Patient.ExternalIdentifier.Invalid",
            result.Error.Code);

        Assert.Empty(
            repository.Patients);
    }
}