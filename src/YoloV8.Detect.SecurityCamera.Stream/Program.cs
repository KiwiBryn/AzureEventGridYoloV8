﻿//---------------------------------------------------------------------------------
// Copyright (c) March 2023, devMobile Software
//
// http://www.apache.org/licenses/LICENSE-2.0
//
//---------------------------------------------------------------------------------
using System.Net;

using Microsoft.Extensions.Configuration;

using Compunet.YoloV8;
using Compunet.YoloV8.Data;
using Compunet.YoloV8.Plotting;

using SixLabors.ImageSharp;


namespace devMobile.IoT.YoloV8.Detect.SecurityCamera.Stream
{
   class Program
   {
      private static Model.ApplicationSettings _applicationSettings;
      private static bool _cameraBusy = false;
      private static HttpClient _httpClient;
      private static YoloV8Predictor _predictor;

      static async Task Main(string[] args)
      {
         Console.WriteLine($"{DateTime.UtcNow:yy-MM-dd HH:mm:ss} SecurityCameraImageStream starting");

         try
         {
            // load the app settings into configuration
            var configuration = new ConfigurationBuilder()
                 .AddJsonFile("appsettings.json", false, true)
                 .AddUserSecrets<Program>()
            .Build();

            _applicationSettings = configuration.GetSection("ApplicationSettings").Get<Model.ApplicationSettings>();

            NetworkCredential networkCredential = new NetworkCredential(_applicationSettings.CameraUserName, _applicationSettings.CameraUserPassword);

            using (_httpClient = new HttpClient(new HttpClientHandler { PreAuthenticate = true, Credentials = networkCredential }))
            using (_predictor = YoloV8Predictor.Create(_applicationSettings.ModelPath))
            {
              Timer imageUpdatetimer = new Timer(ImageUpdateTimerCallback, null, _applicationSettings.ImageTimerDue, _applicationSettings.ImageTimerPeriod);

               Console.WriteLine($" {DateTime.UtcNow:yy-MM-dd HH:mm:ss} press <ctrl^c> to exit");

               try
               {
                  await Task.Delay(Timeout.Infinite);
               }
               catch (TaskCanceledException)
               {
                  Console.WriteLine($"{DateTime.UtcNow:yy-MM-dd HH:mm:ss} Application shutown requested");
               }
            }
         }
         catch (Exception ex)
         {
            Console.WriteLine($"{DateTime.UtcNow:yy-MM-dd HH:mm:ss} Application shutown failure {ex.Message}", ex);
         }
      }

      private static async void ImageUpdateTimerCallback(object state)
      {
         // Just incase - stop code being called while photo already in progress
         if (_cameraBusy)
         {
            return;
         }
         _cameraBusy = true;

         try
         {
            Console.WriteLine($"{DateTime.UtcNow:yy-MM-dd HH:mm:ss:fff} YoloV8 Image processing start");

            DetectionResult result;

            using (System.IO.Stream cameraStream = await _httpClient.GetStreamAsync(_applicationSettings.CameraUrl))
            using (var imageInput = await Image.LoadAsync(cameraStream))
            {
               result = await _predictor.DetectAsync(imageInput);

               await imageInput.SaveAsJpegAsync(_applicationSettings.ImageInputPath);

               using (var imageOutput = await result.PlotImageAsync(imageInput))
               {
                  await imageOutput.SaveAsJpegAsync(_applicationSettings.ImageOutputPath);
               }
            }

            Console.WriteLine($"Speed: {result.Speed}");

            foreach (var prediction in result.Boxes)
            {
               Console.WriteLine($" Class {prediction.Class} {(prediction.Confidence * 100.0):f1}% X:{prediction.Bounds.X} Y:{prediction.Bounds.Y} Width:{prediction.Bounds.Width} Height:{prediction.Bounds.Height}");
            }

            Console.WriteLine($"{DateTime.UtcNow:yy-MM-dd HH:mm:ss:fff} YoloV8 Image processing done");
         }
         catch (Exception ex)
         {
            Console.WriteLine($"{DateTime.UtcNow:yy-MM-dd HH:mm:ss} YoloV8 Security camera image download or YoloV8 prediction failed {ex.Message}");
         }
         finally
         {
            _cameraBusy = false;
         }
      }
   }
}
