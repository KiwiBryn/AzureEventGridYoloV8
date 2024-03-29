﻿//---------------------------------------------------------------------------------
// Copyright (c) February 2023, devMobile Software
//
// http://www.apache.org/licenses/LICENSE-2.0
//
//---------------------------------------------------------------------------------
namespace devMobile.IoT.Azure.EventGrid.Image.Model
{
   using System;

   using HiveMQtt.MQTT5.Types;

   public class ApplicationSettings
	{
		public string DeviceId { get; set; }

		public TimeSpan ImageTimerDue { get; set; }
		public TimeSpan ImageTimerPeriod { get; set; }

      public string UserName { get; set; }

      public string ClientCertificateFileName { get; set; }
      public string ClientCertificatePassword { get; set; }

      public int Port { get; set; }
      public string Host { get; set; }
      public string ClientId { get; set; }
      public bool CleanStart { get; set; }


      public string PublishTopic { get; set; }

      public QualityOfService PublishQualityOfService { get; set; }

      public string CameraUrl { get; set; }
      public string CameraUserName { get; set; }
      public string CameraUserPassword { get; set; }

      public string ModelPath { get; set; }
   }

#if POSE
   public enum PoseMarker
   {
      Nose,
      LeftEye,
      RightEye,
      LeftEar,
      RightEar,
      LeftShoulder,
      RightShoulder,
      LeftElbow,
      RightElbow,
      LeftWrist,
      RightWrist,
      LeftHip,
      RightHip,
      LeftKnee,
      RightKnee,
      LeftAnkle,
      RightAnkle
   }
#endif
}
