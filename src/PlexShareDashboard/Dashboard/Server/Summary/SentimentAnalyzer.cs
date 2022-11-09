using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.ML;
using Microsoft.ML.Data;
using System.Diagnostics;
using Microsoft.ML.Trainers;

namespace PlexShareDashboard.Dashboard.Server.Summary
{
    public class SentimentAnalyzer
    {
        //Machine Learning model to load and use for predictions
        private const string MODEL_FILEPATH = @"../../../../PlexShareDashboard/Dashboard/Server/Summary/Resources/MLModel.zip";

        public SentimentAnalyzer()
        {
            MLContext mlContext = new MLContext();
            // Training code used by ML.NET CLI and AutoML to generate the model
            ModelBuilder.CreateModel();
            ITransformer mlModel = mlContext.Model.Load(GetAbsolutePath(MODEL_FILEPATH), out DataViewSchema inputSchema);
            var predEngine = mlContext.Model.CreatePredictionEngine<ModelInput, ModelOutput>(mlModel);

            // Create sample data to do a single prediction with it 
            string inputSampleText = "ML.NET is awesome!";
            ModelInput sampleData = CreateSingleDataSample(inputSampleText);

            // Try a single prediction
            ModelOutput predictionResult = predEngine.Predict(sampleData);
            Debug.WriteLine($"Single Prediction --> Prediction for '{inputSampleText}' was PositiveSentiment = {predictionResult.Prediction}");
            Debug.WriteLine("=============== End of process, hit any key to finish ===============");
        }

        // Here I create your my sample hard-coded data (Could be coming from an end-user app)
        private static ModelInput CreateSingleDataSample(string inputTextStatement)
        {
            // Here (ModelInput object) you could provide new test data, hardcoded or from the end-user application, instead of the row from the file.
            ModelInput sampleForPrediction = new ModelInput { Text = inputTextStatement };
            return sampleForPrediction;
        }

        public static string GetAbsolutePath(string relativePath)
        {
            FileInfo _dataRoot = new FileInfo(typeof(SentimentAnalyzer).Assembly.Location);
            string assemblyFolderPath = _dataRoot.Directory.FullName;

            string fullPath = Path.Combine(assemblyFolderPath, relativePath);

            return fullPath;
        }
    }
    public class ModelInput
    {
        [ColumnName("text"), LoadColumn(0)]
        public string Text { get; set; }

        [ColumnName("sentiment"), LoadColumn(1)]
        public bool Sentiment { get; set; }
    }
    public class ModelOutput
    {
        // ColumnName attribute is used to change the column name from
        // its default value, which is the name of the field.
        [ColumnName("PredictedLabel")]
        public bool Prediction { get; set; }

        public float Score { get; set; }
    }
    public static class ModelBuilder
    {
        // SMALL DATASET in GitHub repo: 
        private static string TRAIN_DATA_FILEPATH = Directory.GetParent(Directory.GetParent(Directory.GetParent(Directory.GetParent(Environment.CurrentDirectory).ToString()).ToString()).ToString()) + "/PlexShareDashboard/Dashboard/Server/Summary/Resources/yelp_labelled.tsv";

        // SMALL DATASET in AZURE FILES: private static string TRAIN_DATA_FILEPATH = @"X:\\yelp\\yelp_labelled.tsv";      
        // LARGE DATASET in AZURE FILES: private static string TRAIN_DATA_FILEPATH = @"X:\\twitter\\Twittersentiment-1Million.tsv";

        private static string MODEL_FILEPATH = Directory.GetParent(Directory.GetParent(Directory.GetParent(Directory.GetParent(Environment.CurrentDirectory).ToString()).ToString()).ToString()) + "/PlexShareDashboard/Dashboard/Server/Summary/Resources/MLModel.zip";

        // Create MLContext to be shared across the model creation workflow objects 
        // Set a random seed for repeatable/deterministic results across multiple trainings.
        private static MLContext mlContext = new MLContext(seed: 1);

        public static void CreateModel()
        {

            // Load Data
            IDataView trainingDataView = mlContext.Data.LoadFromTextFile<ModelInput>(
                                            path: TRAIN_DATA_FILEPATH,
                                            hasHeader: true,
                                            separatorChar: '\t',
                                            allowQuoting: true,
                                            allowSparse: false);

            // Build training pipeline
            IEstimator<ITransformer> trainingPipeline = BuildTrainingPipeline(mlContext);

            // Evaluate quality of Model
            Evaluate(mlContext, trainingDataView, trainingPipeline);

            // Train Model
            ITransformer mlModel = TrainModel(mlContext, trainingDataView, trainingPipeline);

            // Save model
            SaveModel(mlContext, mlModel, MODEL_FILEPATH, trainingDataView.Schema);
        }

        public static IEstimator<ITransformer> BuildTrainingPipeline(MLContext mlContext)
        {
            // Data process configuration with pipeline data transformations 
            var dataProcessPipeline = mlContext.Transforms.Text.FeaturizeText("text_tf", "text")
                                      .Append(mlContext.Transforms.CopyColumns("Features", "text_tf"))
                                      .Append(mlContext.Transforms.NormalizeMinMax("Features", "Features"))
                                      .AppendCacheCheckpoint(mlContext);

            // Set the training algorithm 
            var trainer = mlContext.BinaryClassification.Trainers.SgdCalibrated(new SgdCalibratedTrainer.Options()
            {
                L2Regularization = 1E-05f * 5,
                ConvergenceTolerance = 2e-5,
                NumberOfIterations = 100,
                Shuffle = true,
                LabelColumnName = "sentiment",
                FeatureColumnName = "Features"
            });


            var trainingPipeline = dataProcessPipeline.Append(trainer);

            return trainingPipeline;
        }

        public static ITransformer TrainModel(MLContext mlContext, IDataView trainingDataView, IEstimator<ITransformer> trainingPipeline)
        {
            Debug.WriteLine("=============== Training  model ===============");

            ITransformer model = trainingPipeline.Fit(trainingDataView);

            Debug.WriteLine("=============== End of training process ===============");
            return model;
        }

        private static void Evaluate(MLContext mlContext, IDataView trainingDataView, IEstimator<ITransformer> trainingPipeline)
        {
            // Cross-Validate with single dataset (since we don't have two datasets, one for training and for evaluate)
            // in order to evaluate and get the model's accuracy metrics
            Debug.WriteLine("=============== Cross-validating to get model's accuracy metrics ===============");
            var crossValidationResults = mlContext.BinaryClassification.CrossValidateNonCalibrated(trainingDataView, trainingPipeline, numberOfFolds: 5, labelColumnName: "sentiment");
            PrintBinaryClassificationFoldsAverageMetrics(crossValidationResults);
        }
        private static void SaveModel(MLContext mlContext, ITransformer mlModel, string modelRelativePath, DataViewSchema modelInputSchema)
        {
            // Save/persist the trained model to a .ZIP file
            Debug.WriteLine($"=============== Saving the model  ===============");
            mlContext.Model.Save(mlModel, modelInputSchema, modelRelativePath);
            Debug.WriteLine("The model is saved to {0}", modelRelativePath);
        }

        public static string GetAbsolutePath(string relativePath)
        {
            FileInfo _dataRoot = new FileInfo(typeof(SentimentAnalyzer).Assembly.Location);
            string assemblyFolderPath = _dataRoot.Directory.FullName;

            string fullPath = Path.Combine(assemblyFolderPath, relativePath);

            return fullPath;
        }

        public static void PrintBinaryClassificationMetrics(BinaryClassificationMetrics metrics)
        {
            Debug.WriteLine($"************************************************************");
            Debug.WriteLine($"*       Metrics for binary classification model      ");
            Debug.WriteLine($"*-----------------------------------------------------------");
            Debug.WriteLine($"*       Accuracy: {metrics.Accuracy:P2}");
            Debug.WriteLine($"*       Auc:      {metrics.AreaUnderRocCurve:P2}");
            Debug.WriteLine($"************************************************************");
        }


        public static void PrintBinaryClassificationFoldsAverageMetrics(IEnumerable<TrainCatalogBase.CrossValidationResult<BinaryClassificationMetrics>> crossValResults)
        {
            var metricsInMultipleFolds = crossValResults.Select(r => r.Metrics);

            var AccuracyValues = metricsInMultipleFolds.Select(m => m.Accuracy);
            var AccuracyAverage = AccuracyValues.Average();
            var AccuraciesStdDeviation = CalculateStandardDeviation(AccuracyValues);
            var AccuraciesConfidenceInterval95 = CalculateConfidenceInterval95(AccuracyValues);


            Debug.WriteLine($"*************************************************************************************************************");
            Debug.WriteLine($"*       Metrics for Binary Classification model      ");
            Debug.WriteLine($"*------------------------------------------------------------------------------------------------------------");
            Debug.WriteLine($"*       Average Accuracy:    {AccuracyAverage:0.###}  - Standard deviation: ({AccuraciesStdDeviation:#.###})  - Confidence Interval 95%: ({AccuraciesConfidenceInterval95:#.###})");
            Debug.WriteLine($"*************************************************************************************************************");
        }

        public static double CalculateStandardDeviation(IEnumerable<double> values)
        {
            double average = values.Average();
            double sumOfSquaresOfDifferences = values.Select(val => (val - average) * (val - average)).Sum();
            double standardDeviation = Math.Sqrt(sumOfSquaresOfDifferences / (values.Count() - 1));
            return standardDeviation;
        }

        public static double CalculateConfidenceInterval95(IEnumerable<double> values)
        {
            double confidenceInterval95 = 1.96 * CalculateStandardDeviation(values) / Math.Sqrt((values.Count() - 1));
            return confidenceInterval95;
        }
    }
}
