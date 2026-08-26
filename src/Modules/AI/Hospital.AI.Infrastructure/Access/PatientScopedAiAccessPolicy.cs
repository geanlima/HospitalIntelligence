using Hospital.AI.Application.Abstractions;
using Hospital.SharedKernel.Application;

namespace Hospital.AI.Infrastructure.Access;

/// <summary>
/// Controle de acesso da Fase 15: exige PatientId válido e paciente existente.
/// Autenticação/autorização real fica para a Fase 20.
/// </summary>
public sealed class PatientScopedAiAccessPolicy : IAiAccessPolicy
{
    private readonly IClinicalRecordSource _clinicalRecordSource;

    public PatientScopedAiAccessPolicy(
        IClinicalRecordSource clinicalRecordSource)
    {
        _clinicalRecordSource = clinicalRecordSource;
    }

    public async Task<Result> EnsureCanAccessPatientAsync(
        Guid patientId,
        CancellationToken cancellationToken = default)
    {
        if (patientId == Guid.Empty)
        {
            return Result.Failure(
                new Error(
                    "AI.Access.PatientIdRequired",
                    "PatientId é obrigatório para acessar o prontuário."));
        }

        var exists =
            await _clinicalRecordSource.PatientExistsAsync(
                patientId,
                cancellationToken);

        if (!exists)
        {
            return Result.Failure(
                new Error(
                    "AI.Access.PatientNotFound",
                    "Paciente não encontrado ou sem permissão de acesso."));
        }

        return Result.Success();
    }
}
