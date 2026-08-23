using Hospital.Patients.Application.Patients.SearchPatients;
using Hospital.Patients.ApplicationTests.Fakes;
using Hospital.Patients.Domain.Patients;

namespace Hospital.Patients.ApplicationTests.Patients.SearchPatients;

public sealed class SearchPatientsHandlerTests
{
    [Fact]
    public async Task HandleAsync_Should_Return_All_Patients_When_Name_Is_Null()
    {
        // Arrange
        var repository =
            new FakePatientRepository();

        await repository.AddAsync(
            Patient.Create(
                "João da Silva",
                new DateOnly(1990, 5, 10),
                Gender.Male));

        await repository.AddAsync(
            Patient.Create(
                "Maria Souza",
                new DateOnly(1988, 3, 20),
                Gender.Female));

        var handler =
            new SearchPatientsHandler(repository);

        var query =
            new SearchPatientsQuery(null);

        // Act
        var result =
            await handler.HandleAsync(query);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(2, result.Value.Count);
    }

    [Fact]
    public async Task HandleAsync_Should_Return_Patients_Matching_Name()
    {
        // Arrange
        var repository =
            new FakePatientRepository();

        await repository.AddAsync(
            Patient.Create(
                "João da Silva",
                new DateOnly(1990, 5, 10),
                Gender.Male));

        await repository.AddAsync(
            Patient.Create(
                "Maria Souza",
                new DateOnly(1988, 3, 20),
                Gender.Female));

        var handler =
            new SearchPatientsHandler(repository);

        var query =
            new SearchPatientsQuery("João");

        // Act
        var result =
            await handler.HandleAsync(query);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Single(result.Value);
        Assert.Equal(
            "João da Silva",
            result.Value.Single().Name);
    }

    [Fact]
    public async Task HandleAsync_Should_Return_Empty_List_When_No_Patient_Matches()
    {
        // Arrange
        var repository =
            new FakePatientRepository();

        await repository.AddAsync(
            Patient.Create(
                "João da Silva",
                new DateOnly(1990, 5, 10),
                Gender.Male));

        var handler =
            new SearchPatientsHandler(repository);

        var query =
            new SearchPatientsQuery("Carlos");

        // Act
        var result =
            await handler.HandleAsync(query);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Empty(result.Value);
    }
}