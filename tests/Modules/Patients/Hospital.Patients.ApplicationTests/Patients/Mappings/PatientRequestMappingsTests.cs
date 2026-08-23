using Hospital.Patients.Application.Patients.Mappings;
using Hospital.Patients.Contracts.Patients;
using Hospital.Patients.Domain.Patients;

namespace Hospital.Patients.ApplicationTests.Patients.Mappings;

public sealed class PatientRequestMappingsTests
{
    [Fact]
    public void ToCommand_Should_Map_CreatePatientRequest()
    {
        var request =
            new CreatePatientRequest(
                "João da Silva",
                new DateOnly(1990, 5, 10),
                (int)Gender.Male,
                "SALUX",
                "12345");

        var command =
            request.ToCommand();

        Assert.Equal(
            request.Name,
            command.Name);

        Assert.Equal(
            request.BirthDate,
            command.BirthDate);

        Assert.Equal(
            Gender.Male,
            command.Gender);

        Assert.Equal(
            "SALUX",
            command.SourceSystem);

        Assert.Equal(
            "12345",
            command.ExternalId);
    }

    [Fact]
    public void ToCommand_Should_Map_UpdatePatientRequest()
    {
        var patientId =
            PatientId.New();

        var request =
            new UpdatePatientRequest(
                "Maria Souza",
                new DateOnly(1988, 3, 20),
                (int)Gender.Female);

        var command =
            request.ToCommand(patientId);

        Assert.Equal(
            patientId,
            command.PatientId);

        Assert.Equal(
            "Maria Souza",
            command.Name);

        Assert.Equal(
            Gender.Female,
            command.Gender);
    }

    [Fact]
    public void ToCommand_Should_Throw_When_Gender_Is_Invalid()
    {
        var request =
            new CreatePatientRequest(
                "João da Silva",
                new DateOnly(1990, 5, 10),
                999,
                null,
                null);

        Assert.Throws<ArgumentOutOfRangeException>(() =>
            request.ToCommand());
    }
}