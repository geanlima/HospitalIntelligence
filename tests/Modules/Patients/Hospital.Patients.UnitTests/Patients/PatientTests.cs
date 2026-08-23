using Hospital.Patients.Domain.Patients;
using Hospital.Patients.Domain.Patients.Events;

namespace Hospital.Patients.UnitTests.Patients;

public sealed class PatientTests
{
    [Fact]
    public void Create_Should_Create_Patient_When_Data_Is_Valid()
    {
        const string name = "João da Silva";
        var birthDate = new DateOnly(1990, 5, 10);
        const Gender gender = Gender.Male;

        var patient = Patient.Create(
            name,
            birthDate,
            gender);

        Assert.NotEqual(default, patient.Id);
        Assert.Equal(name, patient.Name);
        Assert.Equal(birthDate, patient.BirthDate);
        Assert.Equal(gender, patient.Gender);
        Assert.Null(patient.ExternalIdentifier);
    }

    [Fact]
    public void Create_Should_Trim_Name()
    {
        var patient = Patient.Create(
            "  João da Silva  ",
            new DateOnly(1990, 5, 10),
            Gender.Male);

        Assert.Equal("João da Silva", patient.Name);
    }

    [Fact]
    public void Create_Should_Throw_When_Name_Is_Empty()
    {
        var exception = Assert.Throws<PatientDomainException>(() =>
            Patient.Create(
                "",
                new DateOnly(1990, 5, 10),
                Gender.Male));

        Assert.Equal(
            "Patient name cannot be empty.",
            exception.Message);
    }

    [Fact]
    public void Create_Should_Throw_When_Name_Is_Whitespace()
    {
        var exception = Assert.Throws<PatientDomainException>(() =>
            Patient.Create(
                "   ",
                new DateOnly(1990, 5, 10),
                Gender.Male));

        Assert.Equal(
            "Patient name cannot be empty.",
            exception.Message);
    }

    [Fact]
    public void Create_Should_Throw_When_Name_Has_Less_Than_Two_Characters()
    {
        var exception = Assert.Throws<PatientDomainException>(() =>
            Patient.Create(
                "A",
                new DateOnly(1990, 5, 10),
                Gender.Male));

        Assert.Equal(
            "Patient name must have at least 2 characters.",
            exception.Message);
    }

    [Fact]
    public void Create_Should_Throw_When_Name_Has_More_Than_200_Characters()
    {
        var name = new string('A', 201);

        var exception = Assert.Throws<PatientDomainException>(() =>
            Patient.Create(
                name,
                new DateOnly(1990, 5, 10),
                Gender.Male));

        Assert.Equal(
            "Patient name cannot exceed 200 characters.",
            exception.Message);
    }

    [Fact]
    public void Create_Should_Throw_When_BirthDate_Is_In_The_Future()
    {
        var futureDate =
            DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1));

        var exception = Assert.Throws<PatientDomainException>(() =>
            Patient.Create(
                "João da Silva",
                futureDate,
                Gender.Male));

        Assert.Equal(
            "Patient birth date cannot be in the future.",
            exception.Message);
    }

    [Fact]
    public void Create_Should_Create_Patient_When_External_Identifier_Is_Valid()
    {
        var externalIdentifier =
            ExternalPatientIdentifier.Create(
                "SALUX",
                "12345");

        var patient = Patient.Create(
            "João da Silva",
            new DateOnly(1990, 5, 10),
            Gender.Male,
            externalIdentifier);

        Assert.NotNull(patient.ExternalIdentifier);

        Assert.Equal(
            "SALUX",
            patient.ExternalIdentifier.SourceSystem);

        Assert.Equal(
            "12345",
            patient.ExternalIdentifier.ExternalId);
    }

    [Fact]
    public void Create_Should_Register_PatientCreatedDomainEvent()
    {
        var patient = Patient.Create(
            "João da Silva",
            new DateOnly(1990, 5, 10),
            Gender.Male);

        Assert.Single(patient.DomainEvents);

        var domainEvent =
            Assert.IsType<PatientCreatedDomainEvent>(
                patient.DomainEvents.First());

        Assert.Equal(
            patient.Id,
            domainEvent.PatientId);
    }

    [Fact]
    public void Create_Should_Register_PatientCreatedDomainEvent_With_Correct_Date()
    {
        var beforeCreation =
            DateTimeOffset.UtcNow;

        var patient = Patient.Create(
            "João da Silva",
            new DateOnly(1990, 5, 10),
            Gender.Male);

        var afterCreation =
            DateTimeOffset.UtcNow;

        var domainEvent =
            Assert.IsType<PatientCreatedDomainEvent>(
                patient.DomainEvents.First());

        Assert.InRange(
            domainEvent.OccurredOnUtc,
            beforeCreation,
            afterCreation);
    }

    [Fact]
    public void ChangeName_Should_Update_Name_When_Name_Is_Valid()
    {
        var patient =
            CreateDefaultPatient();

        patient.ChangeName(
            "João Santos");

        Assert.Equal(
            "João Santos",
            patient.Name);
    }

    [Fact]
    public void ChangeName_Should_Trim_Name()
    {
        var patient =
            CreateDefaultPatient();

        patient.ChangeName(
            "  João Santos  ");

        Assert.Equal(
            "João Santos",
            patient.Name);
    }

    [Fact]
    public void ChangeName_Should_Throw_When_Name_Is_Invalid()
    {
        var patient =
            CreateDefaultPatient();

        var exception =
            Assert.Throws<PatientDomainException>(() =>
                patient.ChangeName(""));

        Assert.Equal(
            "Patient name cannot be empty.",
            exception.Message);
    }

    [Fact]
    public void ChangeName_Should_Update_UpdatedAtUtc()
    {
        var patient =
            CreateDefaultPatient();

        var originalUpdatedAt =
            patient.UpdatedAtUtc;

        patient.ChangeName(
            "João Santos");

        Assert.True(
            patient.UpdatedAtUtc >= originalUpdatedAt);
    }

    [Fact]
    public void ChangeName_Should_Not_Update_UpdatedAtUtc_When_Name_Does_Not_Change()
    {
        var patient =
            CreateDefaultPatient();

        var originalUpdatedAt =
            patient.UpdatedAtUtc;

        patient.ChangeName(
            "João da Silva");

        Assert.Equal(
            originalUpdatedAt,
            patient.UpdatedAtUtc);
    }

    [Fact]
    public void ChangeBirthDate_Should_Update_BirthDate_When_Date_Is_Valid()
    {
        var patient =
            CreateDefaultPatient();

        var newBirthDate =
            new DateOnly(1991, 8, 20);

        patient.ChangeBirthDate(
            newBirthDate);

        Assert.Equal(
            newBirthDate,
            patient.BirthDate);
    }

    [Fact]
    public void ChangeBirthDate_Should_Throw_When_Date_Is_In_The_Future()
    {
        var patient =
            CreateDefaultPatient();

        var futureDate =
            DateOnly.FromDateTime(
                DateTime.UtcNow.AddDays(1));

        var exception =
            Assert.Throws<PatientDomainException>(() =>
                patient.ChangeBirthDate(
                    futureDate));

        Assert.Equal(
            "Patient birth date cannot be in the future.",
            exception.Message);
    }

    [Fact]
    public void ChangeBirthDate_Should_Not_Update_When_Date_Does_Not_Change()
    {
        var patient =
            CreateDefaultPatient();

        var originalUpdatedAt =
            patient.UpdatedAtUtc;

        var birthDate =
            patient.BirthDate;

        patient.ChangeBirthDate(
            birthDate);

        Assert.Equal(
            originalUpdatedAt,
            patient.UpdatedAtUtc);
    }

    [Fact]
    public void ChangeGender_Should_Update_Gender()
    {
        var patient =
            CreateDefaultPatient();

        patient.ChangeGender(
            Gender.Other);

        Assert.Equal(
            Gender.Other,
            patient.Gender);
    }

    [Fact]
    public void ChangeGender_Should_Not_Update_When_Gender_Does_Not_Change()
    {
        var patient =
            CreateDefaultPatient();

        var originalUpdatedAt =
            patient.UpdatedAtUtc;

        patient.ChangeGender(
            Gender.Male);

        Assert.Equal(
            originalUpdatedAt,
            patient.UpdatedAtUtc);
    }

    [Fact]
    public void UpdateExternalIdentifier_Should_Update_Identifier()
    {
        var patient =
            CreateDefaultPatient();

        var externalIdentifier =
            ExternalPatientIdentifier.Create(
                "SALUX",
                "98765");

        patient.UpdateExternalIdentifier(
            externalIdentifier);

        Assert.Equal(
            externalIdentifier,
            patient.ExternalIdentifier);
    }

    [Fact]
    public void UpdateExternalIdentifier_Should_Clear_Identifier_When_Value_Is_Null()
    {
        var externalIdentifier =
            ExternalPatientIdentifier.Create(
                "SALUX",
                "12345");

        var patient =
            Patient.Create(
                "João da Silva",
                new DateOnly(1990, 5, 10),
                Gender.Male,
                externalIdentifier);

        patient.UpdateExternalIdentifier(
            null);

        Assert.Null(
            patient.ExternalIdentifier);
    }

    [Fact]
    public void UpdateExternalIdentifier_Should_Not_Update_When_Identifier_Does_Not_Change()
    {
        var externalIdentifier =
            ExternalPatientIdentifier.Create(
                "SALUX",
                "12345");

        var patient =
            Patient.Create(
                "João da Silva",
                new DateOnly(1990, 5, 10),
                Gender.Male,
                externalIdentifier);

        var originalUpdatedAt =
            patient.UpdatedAtUtc;

        patient.UpdateExternalIdentifier(
            externalIdentifier);

        Assert.Equal(
            originalUpdatedAt,
            patient.UpdatedAtUtc);
    }

    [Fact]
    public void UpdateExternalIdentifier_Should_Update_When_Identifier_Changes()
    {
        var originalIdentifier =
            ExternalPatientIdentifier.Create(
                "SALUX",
                "12345");

        var patient =
            Patient.Create(
                "João da Silva",
                new DateOnly(1990, 5, 10),
                Gender.Male,
                originalIdentifier);

        var newIdentifier =
            ExternalPatientIdentifier.Create(
                "SALUX",
                "98765");

        patient.UpdateExternalIdentifier(
            newIdentifier);

        Assert.Equal(
            newIdentifier,
            patient.ExternalIdentifier);
    }

    [Fact]
    public void CreatedAtUtc_Should_Be_Set_When_Patient_Is_Created()
    {
        var beforeCreation =
            DateTimeOffset.UtcNow;

        var patient =
            CreateDefaultPatient();

        var afterCreation =
            DateTimeOffset.UtcNow;

        Assert.InRange(
            patient.CreatedAtUtc,
            beforeCreation,
            afterCreation);
    }

    [Fact]
    public void UpdatedAtUtc_Should_Be_Set_When_Patient_Is_Created()
    {
        var beforeCreation =
            DateTimeOffset.UtcNow;

        var patient =
            CreateDefaultPatient();

        var afterCreation =
            DateTimeOffset.UtcNow;

        Assert.InRange(
            patient.UpdatedAtUtc,
            beforeCreation,
            afterCreation);
    }

    private static Patient CreateDefaultPatient()
    {
        return Patient.Create(
            "João da Silva",
            new DateOnly(1990, 5, 10),
            Gender.Male);
    }
}