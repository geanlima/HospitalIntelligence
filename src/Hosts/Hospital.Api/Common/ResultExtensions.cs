using Hospital.SharedKernel.Application;

namespace Hospital.Api.Common;

public static class ResultExtensions
{
    public static IResult ToProblem(
        this Result result)
    {
        return result.Error.Code switch
        {
            "Patient.NotFound" =>
                Results.NotFound(
                    new
                    {
                        result.Error.Code,
                        result.Error.Description
                    }),

            "Patient.ExternalIdentifier.Invalid" =>
                Results.BadRequest(
                    new
                    {
                        result.Error.Code,
                        result.Error.Description
                    }),

            "Patient.ExternalIdentifier.AlreadyExists" =>
                Results.Conflict(
                    new
                    {
                        result.Error.Code,
                        result.Error.Description
                    }),

            "AI.Prompt.NotFound" =>
                Results.NotFound(
                    new
                    {
                        result.Error.Code,
                        result.Error.Description
                    }),

            _ =>
                Results.BadRequest(
                    new
                    {
                        result.Error.Code,
                        result.Error.Description
                    })
        };
    }
}