using Hospital.Patients.Application.Abstractions;
using Hospital.Patients.Domain.Patients;

namespace Hospital.Patients.ApplicationTests.Fakes;

public sealed class FakePatientRepository : IPatientRepository
{
    private readonly List<Patient> _patients = [];

    public Task<Patient?> GetByIdAsync(
        PatientId id,
        CancellationToken cancellationToken = default)
    {
        var patient =
            _patients.FirstOrDefault(
                x => x.Id == id);

        return Task.FromResult(patient);
    }

    public Task<Patient?> GetByExternalIdAsync(
        string sourceSystem,
        string externalId,
        CancellationToken cancellationToken = default)
    {
        var patient =
            _patients.FirstOrDefault(
                x =>
                    x.ExternalIdentifier != null &&
                    string.Equals(
                        x.ExternalIdentifier.SourceSystem,
                        sourceSystem,
                        StringComparison.OrdinalIgnoreCase) &&
                    string.Equals(
                        x.ExternalIdentifier.ExternalId,
                        externalId,
                        StringComparison.OrdinalIgnoreCase));

        return Task.FromResult(patient);
    }

    public Task AddAsync(
        Patient patient,
        CancellationToken cancellationToken = default)
    {
        _patients.Add(patient);

        return Task.CompletedTask;
    }

    public IReadOnlyCollection<Patient> Patients =>
        _patients.AsReadOnly();

    public Task<IReadOnlyCollection<Patient>> SearchAsync(
        string? name,
        string? sourceSystem,
        CancellationToken cancellationToken = default)
    {
        IEnumerable<Patient> query =
            _patients;

        if (!string.IsNullOrWhiteSpace(name))
        {
            var normalizedName =
                name.Trim();

            query = query.Where(
                x =>
                    x.Name.Contains(
                        normalizedName,
                        StringComparison.OrdinalIgnoreCase));
        }

        if (!string.IsNullOrWhiteSpace(sourceSystem))
        {
            var normalizedSourceSystem =
                sourceSystem.Trim();

            query = query.Where(
                x =>
                    x.ExternalIdentifier != null &&
                    string.Equals(
                        x.ExternalIdentifier.SourceSystem,
                        normalizedSourceSystem,
                        StringComparison.OrdinalIgnoreCase));
        }

        IReadOnlyCollection<Patient> result =
            query
                .OrderBy(x => x.Name)
                .ToList()
                .AsReadOnly();

        return Task.FromResult(result);
    }

    public Task UpdateAsync(
        Patient patient,
        CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }
}