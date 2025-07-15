using Microsoft.ML;
using Microsoft.ML.Data;
using MindfulEase.Models.ML;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace MindfulEase.Services
{
    public class BrainDamagePredictionService
    {
        private readonly MLContext _mlContext;
        private readonly string _trainingDataPath;
        private readonly string _modelPath;
        private ITransformer _trainedModel;
        private readonly string[] _brainRegions = new string[]
        {
            "VentromedialPrefrontalCortex",
            "NucleusAccumbens",
            "Amygdala",
            "AnteriorCingulateCortex",
            "Insula",
            "Hypothalamus",
            "DorsolateralPrefrontalCortex",
            "OrbitofrontalCortex",
            "Striatum",
            "Hippocampus",
            "SuperiorParietalCortex",
            "BasalGanglia"
        };

        public BrainDamagePredictionService(string appDataPath)
        {
            _mlContext = new MLContext(seed: 0);
            _trainingDataPath = Path.Combine(appDataPath, "trainingData.csv");
            _modelPath = Path.Combine(appDataPath, "BrainDamageModel");
        }


        /*
        private ITransformer BuildAndTrainModel(IDataView dataView)
        {
            var dataPrepPipeline = _mlContext.Transforms
                .Concatenate("Features",
                    "Age", "Joy", "Sadness", "Anger", "Love", "Fear", "Surprise", "Disgust", "YearsSinceStart")
                .Append(_mlContext.Transforms.Categorical.OneHotEncoding("SexEncoded", "Sex"))
                .Append(_mlContext.Transforms.Concatenate("FinalFeatures", "Features", "SexEncoded"));

            // Create a multi-output model by combining predictions for all brain regions
            var brainRegionEstimators = new List<IEstimator<ITransformer>>();

            foreach (var region in _brainRegions)
            {
                var estimator = _mlContext.Regression.Trainers.FastForest(
                    labelColumnName: region,
                    featureColumnName: "FinalFeatures");
                brainRegionEstimators.Add(estimator);
            }


            IEstimator<ITransformer> pipeline = dataPrepPipeline;
            foreach (var estimator in brainRegionEstimators)
            {
                pipeline = pipeline.Append((IEstimator<ITransformer>)estimator);
            }


            // Train the model
            var model = pipeline.Fit(dataView);
            return model;
        }
        */

        private float CalculateRSquared(List<float> actual, List<float> predicted)
        {
            var meanActual = actual.Average();
            var ssTotal = actual.Select(val => (val - meanActual) * (val - meanActual)).Sum();
            var ssResidual = actual.Zip(predicted, (a, p) => (a - p) * (a - p)).Sum();
            return 1 - (ssResidual / ssTotal);
        }

        private float CalculateRMSE(List<float> actual, List<float> predicted)
        {
            var mse = actual.Zip(predicted, (a, p) => (a - p) * (a - p)).Average();
            return MathF.Sqrt(mse);
        }



        public Dictionary<string, float> CalculateDamageProgressionFromUserHistory(
            List<UserEmotionData> userEmotionalHistory,
            float age,
            string sex,
            int yearsSinceStart)
        {
            // Calculate average emotion values from user history
            var joyAvg = userEmotionalHistory
                .Where(e => e.EmotionLabel.ToLower() == "joy")
                .Select(e => e.MoodValue)
                .DefaultIfEmpty(5)
                .Average();

            var sadnessAvg = userEmotionalHistory
                .Where(e => e.EmotionLabel.ToLower() == "sadness")
                .Select(e => e.MoodValue)
                .DefaultIfEmpty(5)
                .Average();

            var angerAvg = userEmotionalHistory
                .Where(e => e.EmotionLabel.ToLower() == "anger")
                .Select(e => e.MoodValue)
                .DefaultIfEmpty(5)
                .Average();

            var loveAvg = userEmotionalHistory
                .Where(e => e.EmotionLabel.ToLower() == "love")
                .Select(e => e.MoodValue)
                .DefaultIfEmpty(5)
                .Average();

            var fearAvg = userEmotionalHistory
                .Where(e => e.EmotionLabel.ToLower() == "fear")
                .Select(e => e.MoodValue)
                .DefaultIfEmpty(5)
                .Average();

            var surpriseAvg = userEmotionalHistory
                .Where(e => e.EmotionLabel.ToLower() == "surprise")
                .Select(e => e.MoodValue)
                .DefaultIfEmpty(5)
                .Average();

            var disgustAvg = userEmotionalHistory
                .Where(e => e.EmotionLabel.ToLower() == "disgust")
                .Select(e => e.MoodValue)
                .DefaultIfEmpty(5)
                .Average();

            // Create emotional input data based on user's average emotional state
            var emotionalData = new EmotionalInputData
            {
                Age = age + yearsSinceStart,
                Sex = sex,
                Joy = (float)joyAvg,
                Sadness = (float)sadnessAvg,
                Anger = (float)angerAvg,
                Love = (float)loveAvg,
                Fear = (float)fearAvg,
                Surprise = (float)surpriseAvg,
                Disgust = (float)disgustAvg,
                YearsSinceStart = yearsSinceStart
            };

            var prediction = PredictBrainDamageProgression(emotionalData);

            // Convert the prediction to a dictionary for easier handling in UI
            var results = new Dictionary<string, float>();

            // Add each brain region's prediction to the results
            var predictionType = typeof(BrainRegionPrediction);
            foreach (var region in _brainRegions)
            {
                var property = predictionType.GetProperty(region);
                if (property != null)
                {
                    var value = (float)property.GetValue(prediction);
                    results.Add(region, value);
                }
            }

            return results;
        }

        // Original method kept for compatibility
        public Dictionary<string, float> CalculateDamageProgression(EmotionalInputData currentData, int yearsSinceStart)
        {
            // Get a list of the user's projected brain damage after the specified years
            // Create a copy of the input data to avoid modifying the original
            var projectedData = new EmotionalInputData
            {
                Age = currentData.Age + yearsSinceStart,
                Sex = currentData.Sex,
                Joy = currentData.Joy,
                Sadness = currentData.Sadness,
                Anger = currentData.Anger,
                Love = currentData.Love,
                Fear = currentData.Fear,
                Surprise = currentData.Surprise,
                Disgust = currentData.Disgust,
                YearsSinceStart = yearsSinceStart
            };

            var prediction = PredictBrainDamageProgression(projectedData);

            // Convert the prediction to a dictionary for easier handling in UI
            var results = new Dictionary<string, float>();

            // Add each brain region's prediction to the results
            var predictionType = typeof(BrainRegionPrediction);
            foreach (var region in _brainRegions)
            {
                var property = predictionType.GetProperty(region);
                if (property != null)
                {
                    var value = (float)property.GetValue(prediction);
                    results.Add(region, value);
                }
            }

            return results;
        }

        public void TrainModel()
        {
            Console.WriteLine("=== TrainModel START ===");

            Directory.CreateDirectory(_modelPath);

            if (!File.Exists(_trainingDataPath))
            {
                Console.WriteLine($"ERROR: Fisierul de training nu exista la: {_trainingDataPath}");
                return;
            }

            var data = _mlContext.Data.LoadFromTextFile<EmotionalBrainData>(
                _trainingDataPath, hasHeader: true, separatorChar: ',');

            // Split data: 80% train, 20% test
            var split = _mlContext.Data.TrainTestSplit(data, testFraction: 0.2, seed: 1);
            var trainData = split.TrainSet;
            var testData = split.TestSet;

            var dataPrepPipeline = _mlContext.Transforms
                .Concatenate("Features", "Age", "YearsSinceStart", "Joy", "Sadness", "Anger", "Love", "Fear", "Surprise", "Disgust")
                .Append(_mlContext.Transforms.Categorical.OneHotEncoding("SexEncoded", "Sex"))
                .Append(_mlContext.Transforms.Concatenate("FinalFeatures", "Features", "SexEncoded"))
                .Fit(trainData);

            var transformedTrain = dataPrepPipeline.Transform(trainData);
            var transformedTest = dataPrepPipeline.Transform(testData);

            Console.WriteLine("Datele au fost prelucrate si impartite in train/test.");

            foreach (var region in _brainRegions)
            {
                try
                {
                    Console.WriteLine($"\n--- Regiune: {region} ---");

                    var trainer = _mlContext.Regression.Trainers.FastForest(
                        labelColumnName: region,
                        featureColumnName: "FinalFeatures");

                    var model = trainer.Fit(transformedTrain);
                    var modelPath = Path.Combine(_modelPath, $"{region}_model.zip");
                    _mlContext.Model.Save(model, transformedTrain.Schema, modelPath);
                    Console.WriteLine($"Model salvat: {modelPath}");

                    var predictions = model.Transform(transformedTest);
                    var actual = _mlContext.Data.CreateEnumerable<EmotionalBrainData>(testData, reuseRowObject: false)
                                    .Select(d => (float)d.GetType().GetProperty(region)?.GetValue(d))
                                    .ToList();

                    var predicted = _mlContext.Data.CreateEnumerable<PredictionScore>(predictions, reuseRowObject: false)
                                      .Select(p => p.Score)
                                      .ToList();

                    float rmse = CalculateRMSE(actual, predicted);
                    float r2 = CalculateRSquared(actual, predicted);

                    Console.WriteLine($"RMSE: {rmse:F4}");
                    Console.WriteLine($"R²: {r2:F4}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"EROARE la regiunea {region}: {ex.Message}");
                }
            }

            var prepPath = Path.Combine(_modelPath, "DataPrepPipeline.zip");
            _mlContext.Model.Save(dataPrepPipeline, trainData.Schema, prepPath);
            Console.WriteLine($"\nPipeline-ul de preprocesare salvat la: {prepPath}");

            Console.WriteLine("=== TrainModel END ===");
            /*
             RMSE: 0.0747
            R²: 0.7334

            --- Regiune: NucleusAccumbens ---
            Model salvat: C:\Users\Miru\Desktop\Licenta\MindfulEase\MindfulEase\App_Data\BrainDamageModel\NucleusAccumbens_model.zip
            RMSE: 0.0804
            R²: 0.7174

            --- Regiune: Amygdala ---
            Model salvat: C:\Users\Miru\Desktop\Licenta\MindfulEase\MindfulEase\App_Data\BrainDamageModel\Amygdala_model.zip
            RMSE: 0.1258
            R²: 0.7340

            --- Regiune: AnteriorCingulateCortex ---
            Model salvat: C:\Users\Miru\Desktop\Licenta\MindfulEase\MindfulEase\App_Data\BrainDamageModel\AnteriorCingulateCortex_model.zip
            RMSE: 0.1225
            R²: 0.7176

            --- Regiune: Insula ---
            Model salvat: C:\Users\Miru\Desktop\Licenta\MindfulEase\MindfulEase\App_Data\BrainDamageModel\Insula_model.zip
            RMSE: 0.1415
            R²: 0.7010

            --- Regiune: Hypothalamus ---
            Model salvat: C:\Users\Miru\Desktop\Licenta\MindfulEase\MindfulEase\App_Data\BrainDamageModel\Hypothalamus_model.zip
            RMSE: 0.0897
            R²: 0.6703

            --- Regiune: DorsolateralPrefrontalCortex ---
            Model salvat: C:\Users\Miru\Desktop\Licenta\MindfulEase\MindfulEase\App_Data\BrainDamageModel\DorsolateralPrefrontalCortex_model.zip
            RMSE: 0.1074
            R²: 0.7213

            --- Regiune: OrbitofrontalCortex ---
            Model salvat: C:\Users\Miru\Desktop\Licenta\MindfulEase\MindfulEase\App_Data\BrainDamageModel\OrbitofrontalCortex_model.zip
            RMSE: 0.1045
            R²: 0.6725

            --- Regiune: Striatum ---
            Model salvat: C:\Users\Miru\Desktop\Licenta\MindfulEase\MindfulEase\App_Data\BrainDamageModel\Striatum_model.zip
            RMSE: 0.0747
            R²: 0.6542

            --- Regiune: Hippocampus ---
            Model salvat: C:\Users\Miru\Desktop\Licenta\MindfulEase\MindfulEase\App_Data\BrainDamageModel\Hippocampus_model.zip
            RMSE: 0.0970
            R²: 0.7215

            --- Regiune: SuperiorParietalCortex ---
            Model salvat: C:\Users\Miru\Desktop\Licenta\MindfulEase\MindfulEase\App_Data\BrainDamageModel\SuperiorParietalCortex_model.zip
            RMSE: 0.0847
            R²: 0.6621

            --- Regiune: BasalGanglia ---
            Model salvat: C:\Users\Miru\Desktop\Licenta\MindfulEase\MindfulEase\App_Data\BrainDamageModel\BasalGanglia_model.zip
            RMSE: 0.0923
            R²: 0.6889
             */
        }



        public BrainRegionPrediction PredictBrainDamageProgression(EmotionalInputData input)
        {
            Console.WriteLine("=== PredictBrainDamageProgression ===");
            Console.WriteLine($"Input: Age={input.Age}, Sex={input.Sex}, Joy={input.Joy}, Sadness={input.Sadness}, Anger={input.Anger}, Love={input.Love}, Fear={input.Fear}, Surprise={input.Surprise}, Disgust={input.Disgust}, YearsSinceStart={input.YearsSinceStart}");

            var pipelinePath = Path.Combine(_modelPath, "DataPrepPipeline.zip");
            if (!File.Exists(pipelinePath))
            {
                Console.WriteLine("ERROR: DataPrepPipeline.zip nu a fost gasit.");
                return new BrainRegionPrediction();
            }

            var dataPrepPipeline = _mlContext.Model.Load(pipelinePath, out _);
            var inputList = new List<EmotionalInputData> { input };
            var inputDataView = _mlContext.Data.LoadFromEnumerable(inputList);
            var transformedData = dataPrepPipeline.Transform(inputDataView);

            var result = new BrainRegionPrediction();

            foreach (var region in _brainRegions)
            {
                var modelPath = Path.Combine(_modelPath, $"{region}_model.zip");
                if (!File.Exists(modelPath))
                {
                    Console.WriteLine($"Model missing for region: {region}");
                    continue;
                }

                var model = _mlContext.Model.Load(modelPath, out _);
                var predictions = model.Transform(transformedData);

                try
                {
                    var score = _mlContext.Data
                        .CreateEnumerable<PredictionScore>(predictions, reuseRowObject: false)
                        .FirstOrDefault()?.Score ?? -1f;

                    Console.WriteLine($"Prediction for {region}: {score}");
                    typeof(BrainRegionPrediction).GetProperty(region)?.SetValue(result, score);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[ERROR] Nu pot extrage scorul pentru {region}: {ex.Message}");
                }
            }



            return result;
        }


    }


    public class TransformedInput
    {
        [VectorType(10)]
        public float[] FinalFeatures { get; set; }
    }

    public class RegionOutput
    {
        public float Value { get; set; }
    }
    public class RegionScore
    {
        public float Score { get; set; }  // not used in metoda actuala, dar poate fi extinsa
    }
    public class PredictionScore
    {
        public float Score { get; set; } // aici ML.NET pune predictia by default
    }


}
