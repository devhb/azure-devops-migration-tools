﻿using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Serilog;
using System;
using System.Diagnostics;
using System.Net.NetworkInformation;

namespace MigrationTools.Services
{
    public class DetectOnlineService : IDetectOnlineService
    {
        private readonly TelemetryClient _Telemetry;

        public DetectOnlineService(TelemetryClient telemetry)
        {
            _Telemetry = telemetry;
        }

        public bool IsOnline()
        {
            DateTime startTime = DateTime.Now;
            Stopwatch mainTimer = Stopwatch.StartNew();
            //////////////////////////////////
            bool isOnline = false;
            string responce = "none";
            try
            {
                Ping myPing = new Ping();
                String host = "8.8.4.4";
                byte[] buffer = new byte[32];
                int timeout = 1000;
                PingOptions pingOptions = new PingOptions();
                PingReply reply = myPing.Send(host, timeout, buffer, pingOptions);
                responce = reply.Status.ToString();
                if (reply.Status == IPStatus.Success)
                {
                    isOnline = true;
                }
            }
            catch (Exception ex)
            {
                // Likley no network is even available
                Log.Error(ex, "Error checking if we are online.");
                responce = "error";
                isOnline = false;
            }
            /////////////////
            mainTimer.Stop();
            _Telemetry.TrackDependency(new DependencyTelemetry("Ping", "GoogleDNS", "IsOnline", null, startTime, mainTimer.Elapsed, responce, true));
            return isOnline;
        }
    }
}