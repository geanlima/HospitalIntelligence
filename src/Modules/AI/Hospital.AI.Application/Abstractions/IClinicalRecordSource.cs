using Hospital.SharedKernel.Application;

namespace Hospital.AI.Application.Abstractions;

public sealed record ClinicalRecordSnapshot(
    string SourceId,
    string RecordType,
    string Title,
    string Content,
    Guid PatientId,
    DateTimeOffset OccurredAtUtc,
    string? Status = null,
    string? SubType = null);

/// <summary>
/// Porta para ler o prontuário canônico.
/// Implementada no Host (composição de módulos clínicos).
/// </summary>
public interface IClinicalRecordSource
{
    Task<bool> PatientExistsAsync(
        Guid patientId,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<ClinicalRecordSnapshot>> GetByPatientIdAsync(
        Guid patientId,
        CancellationToken cancellationToken = default);
}

public interface IAiAccessPolicy
{
    Task<Result> EnsureCanAccessPatientAsync(
        Guid patientId,
        CancellationToken cancellationToken = default);
}
