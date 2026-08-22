namespace Hospital.SharedKernel.Application;

public sealed record Error(
    string Code,
    string Description)
{
    public static readonly Error None =
        new(string.Empty, string.Empty);
}