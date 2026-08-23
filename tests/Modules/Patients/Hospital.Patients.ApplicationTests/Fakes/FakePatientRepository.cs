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
            _patients.FirstOrDefault(x => x.Id == id);

        return Task.FromResult(patient);
    }

    public Task<Patient?> GetByExternalIdAsync(
        string sourceSystem,
        string externalId,
        CancellationToken cancellationToken = default)
    {
        var patient =
            _patients.FirstOrDefault(x =>
                x.ExternalIdentifier != null &&
                x.ExternalIdentifier.SourceSystem == sourceSystem &&
                x.ExternalIdentifier.ExternalId == externalId);

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
    CancellationToken cancellationToken = default)
    {
        IEnumerable<Patient> query =
            _patients;

        if (!string.IsNullOrWhiteSpace(name))
        {
            query = query.Where(x =>
                x.Name.Contains(
                    name,
                    StringComparison.OrdinalIgnoreCase));
        }

        IReadOnlyCollection<Patient> result =
            query.ToList();

        return Task.FromResult(result);
    }
    public Task UpdateAsync(
    Patient patient,
    CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }
}