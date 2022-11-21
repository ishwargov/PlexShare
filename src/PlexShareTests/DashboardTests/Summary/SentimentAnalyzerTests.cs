/// <author>Jayanth Kumar</author>
/// <created>10/11/2021</created>
/// <summary>
///		This is the unit testing file
///		for the sentiment analyser logic module
/// </summary>
using Microsoft.ML;
using PlexShareDashboard.Dashboard.Server.Summary;
using System.Diagnostics;

namespace PlexShareFacts.DashboardFacts.Summary
{
    public class SentimentAnalyzerFacts
    {

        MLContext _mlContext;   
        ITransformer _trainedModel;

        //Machine Learning model to load and use for predictions
        private static string MODEL_FILEPATH = Directory.GetParent(Directory.GetParent(Directory.GetParent(Directory.GetParent(Environment.CurrentDirectory).ToString()).ToString()).ToString()) + "/PlexShareDashboard/Dashboard/Server/Summary/Resources/MLModel.zip";

        private static string UNIT_Fact_DATA_FILEPATH = Directory.GetParent(Directory.GetParent(Directory.GetParent(Directory.GetParent(Environment.CurrentDirectory).ToString()).ToString()).ToString()) + "/PlexShareTests/DashboardTests/Summary/Resources/unit_test_data_baseline.tsv";

        private static string EVALUATION_DATA_FILEPATH = Directory.GetParent(Directory.GetParent(Directory.GetParent(Directory.GetParent(Environment.CurrentDirectory).ToString()).ToString()).ToString()) + "/PlexShareTests/DashboardTests/Summary/Resources/evaluation_dataset.tsv";

        public SentimentAnalyzerFacts()
        {
            _mlContext = new MLContext();
            _trainedModel = _mlContext.Model.Load(MODEL_FILEPATH, out var modelInputSchema);
        }

        [Fact]
        public void CreateModel()
        {
            SentimentAnalyzer sent = new SentimentAnalyzer();
        }

        [Fact]
        public void FactPositiveSentimentStatement()
        {
            ModelInput sampleStatement = new ModelInput { Text = "ML.NET is awesome!" };

            var predEngine = _mlContext.Model.CreatePredictionEngine<ModelInput, ModelOutput>(_trainedModel);

            var resultprediction = predEngine.Predict(sampleStatement);

            Assert.Equal(true, Convert.ToBoolean(resultprediction.Prediction));
        }

        [Fact]
        public void FactNegativeSentimentStatement()
        {
            string FactStatament = "This movie was very boring...";
            ModelInput sampleStatement = new ModelInput { Text = FactStatament };

            var predEngine = _mlContext.Model.CreatePredictionEngine<ModelInput, ModelOutput>(_trainedModel);

            var resultprediction = predEngine.Predict(sampleStatement);

            Assert.Equal(false, Convert.ToBoolean(resultprediction.Prediction));
        }


        [Fact]
        public void FactAccuracyHigherThan70()
        {
            Debug.WriteLine("===== Evaluating Model's accuracy with Evaluation/Fact dataset =====");

            // Read dataset to get a single row for trying a prediction          
            IDataView FactDataView = _mlContext.Data.LoadFromTextFile<ModelInput>(
                                            path: EVALUATION_DATA_FILEPATH,
                                            hasHeader: true,
                                            separatorChar: '\t');

            IEnumerable<ModelInput> samplesForPrediction = _mlContext.Data.CreateEnumerable<ModelInput>(FactDataView, false);

            //DO BULK PREDICTIONS
            IDataView predictionsDataView = _trainedModel.Transform(FactDataView);

            var predictions = _trainedModel.Transform(FactDataView);
            var metrics = _mlContext.BinaryClassification.Evaluate(data: predictionsDataView, labelColumnName: "sentiment", scoreColumnName: "Score");

            double accuracy = metrics.Accuracy;
            Debug.WriteLine($"Accuracy of model in this validation '{accuracy * 100}'%");

            Assert.True(accuracy >=0.70);
        }

        [Fact]
        //Generate many Fact cases with a bulk prediction approach
        public static void FactCases()
        {
                MLContext mlContext = new MLContext();
                ITransformer trainedModel = mlContext.Model.Load(MODEL_FILEPATH, out var modelInputSchema);

                // Read dataset to get a single row for trying a prediction          
                IDataView FactDataView = mlContext.Data.LoadFromTextFile<ModelInput>(
                                                path: UNIT_Fact_DATA_FILEPATH,
                                                hasHeader: true,
                                                separatorChar: '\t');

                IEnumerable<ModelInput> samplesForPrediction = mlContext.Data.CreateEnumerable<ModelInput>(FactDataView, false);
                ModelInput[] arraysamplesForPrediction = samplesForPrediction.ToArray();

                //DO BULK PREDICTIONS
                IDataView predictionsDataView = trainedModel.Transform(FactDataView);
                IEnumerable<ModelOutput> predictions = mlContext.Data.CreateEnumerable<ModelOutput>(predictionsDataView, false);
                ModelOutput[] arrayPredictions = predictions.ToArray();

            List<float> arr = new List<float>();

            for (int i = 0; i < arraysamplesForPrediction.Length; i++)
                {
                    try
                    {
                    Debug.WriteLine($"Text {arraysamplesForPrediction[i].Text} predicted as {arrayPredictions[i].Prediction} should be {arraysamplesForPrediction[i].Sentiment}");
                    arr.Add(arrayPredictions[i].Score);
                    Assert.Equal(arrayPredictions[i].Prediction, arraysamplesForPrediction[i].Sentiment);
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine(ex.Message);
                    }
                }
            Debug.WriteLine($"max == {arr.Max()}");
            Debug.WriteLine($"min == {arr.Min()}");
        }

    }
}
