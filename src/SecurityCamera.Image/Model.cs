﻿//---------------------------------------------------------------------------------
// Copyright (c) February 2023, devMobile Software
//
// http://www.apache.org/licenses/LICENSE-2.0
//
//---------------------------------------------------------------------------------
using System;

namespace devMobile.IoT.SecurityCameraImage.Model
{
	public class ApplicationSettings
	{
		public TimeSpan ImageTimerDue { get; set; }
		public TimeSpan ImageTimerPeriod { get; set; }

		public string CameraUrl { get; set; }
		public string CameraUserName { get; set; }
		public string CameraUserPassword { get; set; }

		public string ImageFilepathLocal { get; set; }
	}
}
