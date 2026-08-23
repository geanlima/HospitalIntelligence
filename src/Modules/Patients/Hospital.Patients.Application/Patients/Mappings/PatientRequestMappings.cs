using Hospital.Patients.Application.Patients.CreatePatient;
using Hospital.Patients.Application.Patients.SearchPatients;
using Hospital.Patients.Application.Patients.SynchronizeExternalPatient;
using Hospital.Patients.Application.Patients.UpdatePatient;
using Hospital.Patients.Contracts.Patients;
using Hospital.Patients.Domain.Patients;

namespace Hospital.Patients.Application.Patients.Mappings;

public static class PatientRequestMappings
{
    public static CreatePatientCommand ToCommand(
        this CreatePatientRequest request)
    {
        return new CreatePatientCommand(
            request.Name,
            request.BirthDate,
            ToGender(request.Gender),
            request.SourceSystem,
            request.ExternalId);
    }

    public static UpdatePatientCommand ToCommand(
        this UpdatePatientRequest request,
        PatientId patientId)
    {
        return new UpdatePatientCommand(
            patientId,
            request.Name,
            request.BirthDate,
            ToGender(request.Gender));
    }

    public static SearchPatientsQuery ToQuery(
        this SearchPatientsRequest request)
    {
        return new SearchPatientsQuery(
            request.Name);
    }

    public static SynchronizeExternalPatientCommand ToCommand(
        this SynchronizeExternalPatientRequest request)
    {
        return new SynchronizeExternalPatientCommand(
            request.SourceSystem,
            request.ExternalId,
            request.Name,
            request.BirthDate,
            ToGender(request.Gender));
    }

    private static Gender ToGender(int value)
    {
        if (!Enum.IsDefined(typeof(Gender), value))
        {
            throw new ArgumentOutOfRangeException(
                nameof(value),
                value,
                "Invalid gender value.");
        }

        return (Gender)value;
    }
}