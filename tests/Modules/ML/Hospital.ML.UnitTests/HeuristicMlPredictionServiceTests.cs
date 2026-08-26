using Hospital.ML.Application.Abstractions;
using Hospital.ML.Infrastructure.Predict;

namespace Hospital.ML.UnitTests;

public class HeuristicMlPredictionServiceTests
{
    [Fact]
    public async Task PredictDeterioration_LowSpo2_IsElevated()
    {
        var service = new HeuristicMlPredictionService();

        var prediction = await service.PredictDeteriorationAsync(
            new MlFeatureVector(
                Guid.NewGuid(),
                70,
                2,
                1,
                0,
                LatestSpo2: 88,
                LatestHeartRate: 120,
                ActivePrescriptionCount: 1,
                HasMedicalNote: true));

        Assert.Equal("elevated", prediction.Label);
        Assert.True(prediction.Score >= 0.5);
    }

    [Fact]
    public async Task Registry_Exposes_Versioned_Models()
    {
        var service = new HeuristicMlPredictionService();
        var models = await service.GetModelRegistryAsync();

        Assert.Contains(models, m => m.Name == "no-show");
        Assert.Contains(models, m => m.Name == "discharge");
        Assert.Contains(models, m => m.Name == "deterioration");
        Assert.All(models, m => Assert.False(string.IsNullOrWhiteSpace(m.Version)));
    }
}
