using Hospital.Patients.Application.Patients.UpdatePatient;
using Hospital.Patients.ApplicationTests.Fakes;
using Hospital.Patients.Domain.Patients;

namespace Hospital.Patients.ApplicationTests.Patients.UpdatePatient;

public sealed class UpdatePatientHandlerTests
{
    [Fact]
    public async Task HandleAsync_Should_Update_Patient_When_Patient_Exists()
    {
        // Arrange
        var repository =
            new FakePatientRepository();

        var patient =
            Patient.Create(
                "João da Silva",
                new DateOnly(1990, 5, 10),
                Gender.Male);

        await repository.AddAsync(patient);

        var handler =
            new UpdatePatientHandler(repository);

        var command =
            new UpdatePatientCommand(
                patient.Id,
                "João Santos",
                new DateOnly(1991, 8, 20),
                Gender.Other);

        // Act
        var result =
            await handler.HandleAsync(command);

        // Assert
        Assert.True(result.IsSuccess);

        var updatedPatient =
            await repository.GetByIdAsync(patient.Id);

        Assert.NotNull(updatedPatient);

        Assert.Equal(
            "João Santos",
            updatedPatient.Name);

        Assert.Equal(
            new DateOnly(1991, 8, 20),
            updatedPatient.BirthDate);

        Assert.Equal(
            Gender.Other,
            updatedPatient.Gender);
    }

    [Fact]
    public async Task HandleAsync_Should_Return_Failure_When_Patient_Does_Not_Exist()
    {
        // Arrange
        var repository =
            new FakePatientRepository();

        var handler =
            new UpdatePatientHandler(repository);

        var command =
            new UpdatePatientCommand(
                new PatientId(Guid.NewGuid()),
                "João Santos",
                new DateOnly(1991, 8, 20),
                Gender.Other);

        // Act
        var result =
            await handler.HandleAsync(command);

        // Assert
        Assert.True(result.IsFailure);

        Assert.Equal(
            "Patient.NotFound",
            result.Error.Code);

        Assert.Equal(
            "Patient was not found.",
            result.Error.Description);
    }

    [Fact]
    public async Task HandleAsync_Should_Throw_When_Name_Is_Invalid()
    {
        // Arrange
        var repository =
            new FakePatientRepository();

        var patient =
            Patient.Create(
                "João da Silva",
                new DateOnly(1990, 5, 10),
                Gender.Male);

        await repository.AddAsync(patient);

        var handler =
            new UpdatePatientHandler(repository);

        var command =
            new UpdatePatientCommand(
                patient.Id,
                "",
                new DateOnly(1991, 8, 20),
                Gender.Other);

        // Act
        var exception =
            await Assert.ThrowsAsync<PatientDomainException>(() =>
                handler.HandleAsync(command));

        // Assert
        Assert.Equal(
            "Patient name cannot be empty.",
            exception.Message);
    }

    [Fact]
    public async Task HandleAsync_Should_Throw_When_BirthDate_Is_In_The_Future()
    {
        // Arrange
        var repository =
            new FakePatientRepository();

        var patient =
            Patient.Create(
                "João da Silva",
                new DateOnly(1990, 5, 10),
                Gender.Male);

        await repository.AddAsync(patient);

        var handler =
            new UpdatePatientHandler(repository);

        var futureDate =
            DateOnly.FromDateTime(
                DateTime.UtcNow.AddDays(1));

        var command =
            new UpdatePatientCommand(
                patient.Id,
                "João Santos",
                futureDate,
                Gender.Other);

        // Act
        var exception =
            await Assert.ThrowsAsync<PatientDomainException>(() =>
                handler.HandleAsync(command));

        // Assert
        Assert.Equal(
            "Patient birth date cannot be in the future.",
            exception.Message);
    }
}