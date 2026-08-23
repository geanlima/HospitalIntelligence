using Hospital.Patients.Application.Patients.GetPatientById;
using Hospital.Patients.ApplicationTests.Fakes;
using Hospital.Patients.Domain.Patients;

namespace Hospital.Patients.ApplicationTests.Patients.GetPatientById;

public sealed class GetPatientByIdHandlerTests
{
    [Fact]
    public async Task HandleAsync_Should_Return_Patient_When_Patient_Exists()
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
            new GetPatientByIdHandler(repository);

        var query =
            new GetPatientByIdQuery(
                patient.Id);

        // Act
        var result =
            await handler.HandleAsync(query);

        // Assert
        Assert.True(result.IsSuccess);

        Assert.Equal(
            patient.Id,
            result.Value.Id);

        Assert.Equal(
            "João da Silva",
            result.Value.Name);
    }

    [Fact]
    public async Task HandleAsync_Should_Return_Failure_When_Patient_Does_Not_Exist()
    {
        // Arrange
        var repository =
            new FakePatientRepository();

        var handler =
            new GetPatientByIdHandler(repository);

        var query =
            new GetPatientByIdQuery(
                new PatientId(Guid.NewGuid()));

        // Act
        var result =
            await handler.HandleAsync(query);

        // Assert
        Assert.True(result.IsFailure);

        Assert.Equal(
            "Patient.NotFound",
            result.Error.Code);

        Assert.Equal(
            "Patient was not found.",
            result.Error.Description);
    }
}