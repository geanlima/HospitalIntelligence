using System.Net;
using System.Net.Http.Json;
using Hospital.Patients.Contracts.Patients;

namespace Hospital.Patients.IntegrationTests.Api;

public sealed class PatientsApiTests
    : IClassFixture<HospitalApiFactory>
{
    private readonly HttpClient _client;

    public PatientsApiTests(
        HospitalApiFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task CreatePatient_ShouldReturnCreated()
    {
        var externalId =
            $"API-{Guid.NewGuid():N}";

        var request =
            new CreatePatientRequest(
                "Paciente API Integration Test",
                new DateOnly(1990, 5, 10),
                1,
                "API_TEST",
                externalId);

        var response =
            await _client.PostAsJsonAsync(
                "/patients",
                request);

        Assert.Equal(
            HttpStatusCode.Created,
            response.StatusCode);
    }

    [Fact]
    public async Task CreateAndGetPatient_ShouldReturnPersistedPatient()
    {
        var externalId =
            $"API-{Guid.NewGuid():N}";

        var request =
            new CreatePatientRequest(
                "Paciente Consulta API",
                new DateOnly(1985, 10, 20),
                1,
                "API_TEST",
                externalId);

        var createResponse =
            await _client.PostAsJsonAsync(
                "/patients",
                request);

        Assert.Equal(
            HttpStatusCode.Created,
            createResponse.StatusCode);

        var created =
            await createResponse.Content.ReadFromJsonAsync<
                CreatedPatientResponse>();

        Assert.NotNull(created);

        var getResponse =
            await _client.GetAsync(
                $"/patients/{created.Id}");

        Assert.Equal(
            HttpStatusCode.OK,
            getResponse.StatusCode);

        var patient =
            await getResponse.Content.ReadFromJsonAsync<
                PatientResponse>();

        Assert.NotNull(patient);

        Assert.Equal(
            "Paciente Consulta API",
            patient.Name);

        Assert.Equal(
            externalId,
            patient.ExternalId);
    }

    [Fact]
    public async Task SearchPatients_ShouldReturnPatient()
    {
        var externalId =
            $"SEARCH-{Guid.NewGuid():N}";

        var patientName =
            $"Paciente Busca {Guid.NewGuid():N}";

        var request =
            new CreatePatientRequest(
                patientName,
                new DateOnly(1991, 4, 15),
                1,
                "API_TEST",
                externalId);

        var createResponse =
            await _client.PostAsJsonAsync(
                "/patients",
                request);

        Assert.Equal(
            HttpStatusCode.Created,
            createResponse.StatusCode);

        var searchResponse =
            await _client.GetAsync(
                $"/patients?name={Uri.EscapeDataString(patientName)}");

        Assert.Equal(
            HttpStatusCode.OK,
            searchResponse.StatusCode);

        var patients =
            await searchResponse.Content.ReadFromJsonAsync<
                List<PatientResponse>>();

        Assert.NotNull(patients);

        Assert.Contains(
            patients,
            patient => patient.Name == patientName);
    }

    [Fact]
    public async Task UpdatePatient_ShouldReturnNoContent()
    {
        var externalId =
            $"UPDATE-{Guid.NewGuid():N}";

        var createRequest =
            new CreatePatientRequest(
                "Paciente Antes Update",
                new DateOnly(1987, 8, 10),
                1,
                "API_TEST",
                externalId);

        var createResponse =
            await _client.PostAsJsonAsync(
                "/patients",
                createRequest);

        Assert.Equal(
            HttpStatusCode.Created,
            createResponse.StatusCode);

        var created =
            await createResponse.Content.ReadFromJsonAsync<
                CreatedPatientResponse>();

        Assert.NotNull(created);

        var updateRequest =
            new UpdatePatientRequest(
                "Paciente Depois Update",
                new DateOnly(1987, 8, 10),
                1);

        var updateResponse =
            await _client.PutAsJsonAsync(
                $"/patients/{created.Id}",
                updateRequest);

        Assert.Equal(
            HttpStatusCode.NoContent,
            updateResponse.StatusCode);

        var getResponse =
            await _client.GetAsync(
                $"/patients/{created.Id}");

        Assert.Equal(
            HttpStatusCode.OK,
            getResponse.StatusCode);

        var patient =
            await getResponse.Content.ReadFromJsonAsync<
                PatientResponse>();

        Assert.NotNull(patient);

        Assert.Equal(
            "Paciente Depois Update",
            patient.Name);
    }

    [Fact]
    public async Task SynchronizeExternalPatient_ShouldReturnOk()
    {
        var externalId =
            $"SYNC-{Guid.NewGuid():N}";

        var request =
            new SynchronizeExternalPatientRequest(
                "API_TEST",
                externalId,
                "Paciente Sincronizado",
                new DateOnly(1993, 2, 20),
                2);

        var response =
            await _client.PostAsJsonAsync(
                "/patients/synchronize",
                request);

        Assert.Equal(
            HttpStatusCode.OK,
            response.StatusCode);
    }

    [Fact]
    public async Task GetPatientById_WhenPatientDoesNotExist_ShouldReturnNotFound()
    {
        var id =
            Guid.NewGuid();

        var response =
            await _client.GetAsync(
                $"/patients/{id}");

        Assert.Equal(
            HttpStatusCode.NotFound,
            response.StatusCode);
    }

    [Fact]
    public async Task CreatePatient_WithDuplicateExternalIdentifier_ShouldReturnConflict()
    {
        var externalId =
            $"DUP-{Guid.NewGuid():N}";

        var firstRequest =
            new CreatePatientRequest(
                "Paciente Original",
                new DateOnly(1980, 1, 1),
                1,
                "API_TEST",
                externalId);

        var firstResponse =
            await _client.PostAsJsonAsync(
                "/patients",
                firstRequest);

        Assert.Equal(
            HttpStatusCode.Created,
            firstResponse.StatusCode);

        var duplicateRequest =
            new CreatePatientRequest(
                "Paciente Duplicado",
                new DateOnly(1985, 1, 1),
                2,
                "API_TEST",
                externalId);

        var duplicateResponse =
            await _client.PostAsJsonAsync(
                "/patients",
                duplicateRequest);

        Assert.Equal(
            HttpStatusCode.Conflict,
            duplicateResponse.StatusCode);
    }

    private sealed record CreatedPatientResponse(
        Guid Id);
}