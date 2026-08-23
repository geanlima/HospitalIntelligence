using Hospital.Patients.Domain.Patients;

namespace Hospital.Patients.UnitTests.Patients;

public sealed class ExternalPatientIdentifierTests
{
    [Fact]
    public void Create_Should_Create_Identifier_When_Data_Is_Valid()
    {
        var identifier =
            ExternalPatientIdentifier.Create(
                "SALUX",
                "12345");

        Assert.Equal(
            "SALUX",
            identifier.SourceSystem);

        Assert.Equal(
            "12345",
            identifier.ExternalId);
    }

    [Fact]
    public void Create_Should_Trim_Values()
    {
        var identifier =
            ExternalPatientIdentifier.Create(
                " SALUX ",
                " 12345 ");

        Assert.Equal(
            "SALUX",
            identifier.SourceSystem);

        Assert.Equal(
            "12345",
            identifier.ExternalId);
    }

    [Fact]
    public void Create_Should_Throw_When_SourceSystem_Is_Empty()
    {
        var exception =
            Assert.Throws<PatientDomainException>(() =>
                ExternalPatientIdentifier.Create(
                    "",
                    "12345"));

        Assert.Equal(
            "SourceSystem cannot be empty.",
            exception.Message);
    }

    [Fact]
    public void Create_Should_Throw_When_ExternalId_Is_Empty()
    {
        var exception =
            Assert.Throws<PatientDomainException>(() =>
                ExternalPatientIdentifier.Create(
                    "SALUX",
                    ""));

        Assert.Equal(
            "ExternalId cannot be empty.",
            exception.Message);
    }

    [Fact]
    public void Identifiers_With_Same_Values_Should_Be_Equal()
    {
        var first =
            ExternalPatientIdentifier.Create(
                "SALUX",
                "12345");

        var second =
            ExternalPatientIdentifier.Create(
                "SALUX",
                "12345");

        Assert.Equal(first, second);
    }

    [Fact]
    public void Identifiers_With_Different_Values_Should_Not_Be_Equal()
    {
        var first =
            ExternalPatientIdentifier.Create(
                "SALUX",
                "12345");

        var second =
            ExternalPatientIdentifier.Create(
                "SALUX",
                "99999");

        Assert.NotEqual(first, second);
    }
}