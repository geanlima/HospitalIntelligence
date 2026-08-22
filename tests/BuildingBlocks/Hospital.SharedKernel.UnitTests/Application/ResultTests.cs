using Hospital.SharedKernel.Application;

namespace Hospital.SharedKernel.UnitTests.Application;

public sealed class ResultTests
{
    [Fact]
    public void Success_Should_Create_Success_Result()
    {
        var result = Result.Success();

        Assert.True(result.IsSuccess);
        Assert.False(result.IsFailure);
        Assert.Equal(
            Error.None,
            result.Error);
    }

    [Fact]
    public void Failure_Should_Create_Failure_Result()
    {
        var error = new Error(
            "Test.Error",
            "Test error.");

        var result = Result.Failure(error);

        Assert.True(result.IsFailure);
        Assert.Equal(
            error,
            result.Error);
    }

    [Fact]
    public void Generic_Success_Should_Return_Value()
    {
        var result =
            Result<int>.Success(10);

        Assert.True(result.IsSuccess);
        Assert.Equal(
            10,
            result.Value);
    }

    [Fact]
    public void Generic_Failure_Should_Not_Allow_Value()
    {
        var result =
            Result<int>.Failure(
                new Error(
                    "Test.Error",
                    "Test error."));

        Assert.Throws<InvalidOperationException>(
            () => result.Value);
    }
}