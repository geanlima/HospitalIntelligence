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

    private sealed record CreatedPatientResponse(
        Guid Id);
}