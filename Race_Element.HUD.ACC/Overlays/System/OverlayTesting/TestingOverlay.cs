﻿using Newtonsoft.Json;
using RaceElement.Core.Jobs.LoopJob;
using RaceElement.Data.Common;
using RaceElement.HUD.Overlay.Internal;
using RaceElement.HUD.Overlay.Util;
using System.Diagnostics;
using System.Drawing;

namespace RaceElement.HUD.ACC.Overlays.System.OverlayTesting
{
    [Overlay(Name = "Testing", Description = "some testing ")]
    internal class TestingOverlay : AbstractOverlay
    {
        private readonly AbstractLoopJob _job;
        private readonly InfoPanel _panel;

        public TestingOverlay(Rectangle rectangle) : base(rectangle, "Testing")
        {
            Width = 500;
            Height = 100;
            RefreshRateHz = 50;
            SubscribeToACCData = false;

            _panel = new InfoPanel(10, 500);
            _job = new SimpleLoopJob() { Action = () => SimDataProvider.Update(), IntervalMillis = 1000 / 50 };
        }

        public override void BeforeStart()
        {
            _job.Run();
        }

        public override void BeforeStop()
        {
            _job.CancelJoin();
        }

        public override bool ShouldRender() => true;// SimDataProvider.LocalCar.Engine.RPM > 0;
        public override void Render(Graphics g)
        {
            var localCar = SimDataProvider.LocalCar;
            _panel.AddLine($"Speed", $"{localCar.Physics.Velocity:F2}");
            _panel.AddLine($"Location", $"{localCar.Physics.Location}");

            var session = SimDataProvider.Session;
            _panel.AddLine($"Temps", $"Air: {session.Weather.AirTemperature:F1}, Track: {session.Track.Temperature}{(session.Weather.AirPressure > 0 ? $", Pressure: {session.Weather.AirPressure}" : "")}");
            _panel.AddLine($"Track", $"{session.Track.GameName}");
            _panel.AddLine($"Car", $"{localCar.CarModel.GameName}");
            _panel.AddLine("RPM", $"{localCar.Engine.RPM}/{localCar.Engine.MaxRPM}");

            _panel.AddLine($"WindHeading", $"{session.Weather.AirDirection}");

            _panel.Draw(g);

            Debug.WriteLine(JsonConvert.SerializeObject(SimDataProvider.GameData));
        }
    }
}
