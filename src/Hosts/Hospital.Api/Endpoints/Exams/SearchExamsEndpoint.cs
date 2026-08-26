using Hospital.Exams.Application.Exams.SearchExams;
using Hospital.Exams.Domain.Exams;
using Hospital.Patients.Application.Abstractions;
using Hospital.Patients.Domain.Patients;

namespace Hospital.Api.Endpoints.Exams;

public static class SearchExamsEndpoint
{
    public static IEndpointRouteBuilder MapSearchExamsEndpoint(
        this IEndpointRouteBuilder app)
    {
        app.MapGet(
                "/exams",
                async (
                    ExamStatus? status,
                    string? name,
                    SearchExamsHandler handler,
                    IPatientRepository patientRepository,
                    CancellationToken cancellationToken) =>
                {
                    var query = new SearchExamsQuery(
                        status,
                        name);

                    var exams =
                        await handler.HandleAsync(
                            query,
                            cancellationToken);

                    var response =
                        new List<ExamListResponse>();

                    foreach (var exam in exams)
                    {
                        var patient =
                            await patientRepository.GetByIdAsync(
                                new PatientId(exam.PatientId),
                                cancellationToken);

                        response.Add(
                            new ExamListResponse(
                                exam.Id,
                                exam.PatientId,
                                patient?.Name ?? "Paciente não encontrado",
                                exam.Name,
                                exam.RequestedAtUtc,
                                exam.ResultedAtUtc,
                                exam.Status,
                                exam.Result));
                    }

                    return Results.Ok(response);
                })
            .WithTags("Exams")
            .WithName("SearchExams")
            .WithSummary("Lista exames")
            .WithDescription(
                "Lista exames com filtros opcionais por status e nome.");

        return app;
    }
}
