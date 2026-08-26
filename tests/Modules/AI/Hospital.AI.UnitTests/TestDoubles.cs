using Hospital.AI.Application.Abstractions;
using Hospital.SharedKernel.Application;

namespace Hospital.AI.UnitTests;

internal sealed class FakeClinicalRecordSource : IClinicalRecordSource
{
    private readonly Dictionary<Guid, List<ClinicalRecordSnapshot>> _records = new();
    private readonly HashSet<Guid> _existingPatients = [];

    public void AddPatient(Guid patientId)
    {
        _existingPatients.Add(patientId);
        _records.TryAdd(patientId, []);
    }

    public void AddRecord(ClinicalRecordSnapshot record)
    {
        _existingPatients.Add(record.PatientId);
        if (!_records.TryGetValue(record.PatientId, out var list))
        {
            list = [];
            _records[record.PatientId] = list;
        }

        list.Add(record);
    }

    public Task<bool> PatientExistsAsync(
        Guid patientId,
        CancellationToken cancellationToken = default)
    {
        return Task.FromResult(_existingPatients.Contains(patientId));
    }

    public Task<IReadOnlyList<ClinicalRecordSnapshot>> GetByPatientIdAsync(
        Guid patientId,
        CancellationToken cancellationToken = default)
    {
        if (!_records.TryGetValue(patientId, out var list))
        {
            return Task.FromResult<IReadOnlyList<ClinicalRecordSnapshot>>([]);
        }

        return Task.FromResult<IReadOnlyList<ClinicalRecordSnapshot>>(list);
    }
}

internal sealed class AllowAllAiAccessPolicy : IAiAccessPolicy
{
    public Task<Result> EnsureCanAccessPatientAsync(
        Guid patientId,
        CancellationToken cancellationToken = default)
    {
        if (patientId == Guid.Empty)
        {
            return Task.FromResult(
                Result.Failure(
                    new Error(
                        "AI.Access.PatientIdRequired",
                        "PatientId é obrigatório.")));
        }

        return Task.FromResult(Result.Success());
    }
}

internal sealed class DenyAiAccessPolicy : IAiAccessPolicy
{
    public Task<Result> EnsureCanAccessPatientAsync(
        Guid patientId,
        CancellationToken cancellationToken = default)
    {
        return Task.FromResult(
            Result.Failure(
                new Error(
                    "AI.Access.PatientNotFound",
                    "Paciente não encontrado ou sem permissão de acesso.")));
    }
}
