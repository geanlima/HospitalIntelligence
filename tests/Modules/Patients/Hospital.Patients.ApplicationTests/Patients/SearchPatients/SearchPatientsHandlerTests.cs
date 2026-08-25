using Hospital.Patients.Application.Patients.SearchPatients;
using Hospital.Patients.ApplicationTests.Fakes;
using Hospital.Patients.Domain.Patients;

namespace Hospital.Patients.ApplicationTests.Patients.SearchPatients;

public sealed class SearchPatientsHandlerTests
{
    [Fact]
    public async Task HandleAsync_Should_Return_All_Patients_When_Filters_Are_Null()
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
            new SearchPatientsQuery(
                null,
                null);

        // Act
        var result =
            await handler.HandleAsync(query);

        // Assert
        Assert.True(result.IsSuccess);

        Assert.Equal(
            2,
            result.Value.Count);
    }

    [Fact]
    public async Task HandleAsync_Should_Return_Patients_Matching_Name()
    {
        // Arrange
        var repository =
            new FakePatientRepository();

        var joao =
            Patient.Create(
                "João da Silva",
                new DateOnly(1990, 5, 10),
                Gender.Male);

        await repository.AddAsync(joao);

        await repository.AddAsync(
            Patient.Create(
                "Maria Souza",
                new DateOnly(1988, 3, 20),
                Gender.Female));

        var handler =
            new SearchPatientsHandler(repository);

        var query =
            new SearchPatientsQuery(
                "João",
                null);

        // Act
        var result =
            await handler.HandleAsync(query);

        // Assert
        Assert.True(result.IsSuccess);

        Assert.Single(result.Value);

        var response =
            result.Value.Single();

        Assert.Equal(
            joao.Id.Value,
            response.Id);

        Assert.Equal(
            "João da Silva",
            response.Name);

        Assert.Equal(
            joao.BirthDate,
            response.BirthDate);

        Assert.Equal(
            joao.Gender.ToString(),
            response.Gender);
    }

    [Fact]
    public async Task HandleAsync_Should_Return_Patients_Matching_SourceSystem()
    {
        // Arrange
        var repository =
            new FakePatientRepository();

        var saluxPatient =
            Patient.Create(
                "Paciente Salux",
                new DateOnly(1990, 1, 1),
                Gender.Male,
                ExternalPatientIdentifier.Create(
                    "SALUX",
                    "PAC-001"));

        var apiTestPatient =
            Patient.Create(
                "Paciente API",
                new DateOnly(1991, 1, 1),
                Gender.Female,
                ExternalPatientIdentifier.Create(
                    "API_TEST",
                    "PAC-002"));

        await repository.AddAsync(saluxPatient);
        await repository.AddAsync(apiTestPatient);

        var handler =
            new SearchPatientsHandler(repository);

        var query =
            new SearchPatientsQuery(
                null,
                "SALUX");

        // Act
        var result =
            await handler.HandleAsync(query);

        // Assert
        Assert.True(result.IsSuccess);

        Assert.Single(result.Value);

        var response =
            result.Value.Single();

        Assert.Equal(
            saluxPatient.Id.Value,
            response.Id);

        Assert.Equal(
            "SALUX",
            response.SourceSystem);
    }

    [Fact]
    public async Task HandleAsync_Should_Return_Patients_Matching_Name_And_SourceSystem()
    {
        // Arrange
        var repository =
            new FakePatientRepository();

        var expectedPatient =
            Patient.Create(
                "João da Silva",
                new DateOnly(1990, 5, 10),
                Gender.Male,
                ExternalPatientIdentifier.Create(
                    "SALUX",
                    "PAC-001"));

        await repository.AddAsync(expectedPatient);

        await repository.AddAsync(
            Patient.Create(
                "João da Silva",
                new DateOnly(1991, 6, 15),
                Gender.Male,
                ExternalPatientIdentifier.Create(
                    "API_TEST",
                    "PAC-002")));

        await repository.AddAsync(
            Patient.Create(
                "Maria Souza",
                new DateOnly(1988, 3, 20),
                Gender.Female,
                ExternalPatientIdentifier.Create(
                    "SALUX",
                    "PAC-003")));

        var handler =
            new SearchPatientsHandler(repository);

        var query =
            new SearchPatientsQuery(
                "João",
                "SALUX");

        // Act
        var result =
            await handler.HandleAsync(query);

        // Assert
        Assert.True(result.IsSuccess);

        Assert.Single(result.Value);

        var response =
            result.Value.Single();

        Assert.Equal(
            expectedPatient.Id.Value,
            response.Id);

        Assert.Equal(
            "SALUX",
            response.SourceSystem);
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
            new SearchPatientsQuery(
                "Carlos",
                null);

        // Act
        var result =
            await handler.HandleAsync(query);

        // Assert
        Assert.True(result.IsSuccess);

        Assert.Empty(result.Value);
    }
}