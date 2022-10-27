using System.Collections.Generic;
using System.Net;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using Dynastream.Fit;
using DateTime = Dynastream.Fit.DateTime;
using System.IO;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Threading.Tasks;
using System;

namespace GetFIT
{
    public class FitRequestStep
    {
        public string intensity { get; set; }
        public Nullable<int> duration { get; set; }
        public Nullable<int> distance { get; set; }
        public Nullable<float> targetSpeedLow { get; set; }
        public Nullable<float> targetSpeedHigh { get; set; }
    }

    public class FitRequest
    {
        public string name { get; set; }
        public FitRequestStep[] steps { get; set; }
    }

    public class GetFitFromJson
    {
        private readonly ILogger _logger;

        public GetFitFromJson(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<GetFitFromJson>();
        }

        [Function("GetFitFromJson")]
        public async Task<HttpResponseData> RunAsync([HttpTrigger(AuthorizationLevel.Anonymous, "post")] HttpRequestData req)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");

            FitRequest requestData = await req.ReadFromJsonAsync<FitRequest>();

            // 1. Create the output stream, this can be any type of stream, including a file or memory stream. Must have read/write access.
            MemoryStream fitDest = new MemoryStream();

            // 2. Create a FIT Encode object.
            Encode encoder = new Encode(ProtocolVersion.V10);

            // 3. Write the FIT header to the output stream.
            encoder.Open(fitDest);

            // The timestamp for the workout file
            var timeCreated = new Dynastream.Fit.DateTime(System.DateTime.UtcNow);

            // 4. Every FIT file MUST contain a File ID message as the first message
            var fileIdMesg = new FileIdMesg();
            fileIdMesg.SetType(Dynastream.Fit.File.Workout);
            fileIdMesg.SetManufacturer(Manufacturer.Development);
            fileIdMesg.SetProduct(1);
            fileIdMesg.SetSerialNumber(timeCreated.GetTimeStamp());
            fileIdMesg.SetTimeCreated(timeCreated);
            encoder.Write(fileIdMesg);

            // 5. Every FIT Workout file MUST contain a Workout message as the second message
            var workoutMesg = new WorkoutMesg();
            workoutMesg.SetWktName(requestData.name);
            workoutMesg.SetSport(Sport.Running);
            workoutMesg.SetSubSport(SubSport.Invalid);
            workoutMesg.SetNumValidSteps((ushort?)requestData.steps.Length);
            encoder.Write(workoutMesg);

            // 6. Every FIT Workout file MUST contain one or more Workout Step messages
            ushort stepIndex = 0;
            foreach (FitRequestStep step in requestData.steps) {
                var workoutStepMesg = new WorkoutStepMesg();
                workoutStepMesg.SetMessageIndex(stepIndex);

                if (step.duration != null) {
                    workoutStepMesg.SetDurationType(WktStepDuration.Time);    // seconds
                    workoutStepMesg.SetDurationTime(step.duration);
                } else {
                    workoutStepMesg.SetDurationType(WktStepDuration.Distance);
                    workoutStepMesg.SetDurationDistance(step.distance);
                }

                // switching for the different intensities
                if (step.intensity == "warmup") {
                    workoutStepMesg.SetIntensity(Intensity.Warmup);
                } else if (step.intensity == "cooldown") {
                    workoutStepMesg.SetIntensity(Intensity.Cooldown);
                } else if (step.intensity == "recovery") {
                    workoutStepMesg.SetIntensity(Intensity.Recovery);
                } else if (step.intensity == "interval") {
                    workoutStepMesg.SetIntensity(Intensity.Interval);
                } else {
                    workoutStepMesg.SetIntensity(Intensity.Active);
                }

                // switching for which type of target
                if (step.targetSpeedLow != null) {
                    workoutStepMesg.SetTargetValue(0);  // seems to be required
                    workoutStepMesg.SetTargetType(WktStepTarget.Speed);
                    workoutStepMesg.SetCustomTargetSpeedLow(step.targetSpeedLow);    // m/s
                    workoutStepMesg.SetCustomTargetSpeedHigh(step.targetSpeedHigh);    // m/s
                } else {
                    workoutStepMesg.SetTargetValue(0);
                    workoutStepMesg.SetTargetType(WktStepTarget.Open);
                }
                
                Console.WriteLine($"{stepIndex}: {workoutStepMesg.GetIntensity()}");
                encoder.Write(workoutStepMesg);
                stepIndex++;
            }

            // 7. Update the data size in the header and calculate the CRC
            encoder.Close();

            // // 8. Close the output stream
            // fitDest.Close();

            var response = req.CreateResponse(HttpStatusCode.OK);
            response.Headers.Add("Content-Type", "application/octet-stream");
            response.Headers.Add("Content-Disposition", "attachment; filename=\"fartlek.fit\"");
            response.WriteBytes(fitDest.ToArray());
            return response;
        }
    }
}
