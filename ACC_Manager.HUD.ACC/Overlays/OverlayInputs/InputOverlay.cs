﻿using ACCManager.Data.ACC.Cars;
using ACCManager.HUD.Overlay.Configuration;
using ACCManager.HUD.Overlay.Internal;
using ACCManager.HUD.Overlay.OverlayUtil;
using ACCManager.HUD.Overlay.Util;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using static ACCManager.ACCSharedMemory;

namespace ACCManager.HUD.ACC.Overlays.OverlayInputs
{
    internal sealed class InputsOverlay : AbstractOverlay
    {
        private const int _size = 150;
        private const int wheelWidth = 15;
        private const int indicatorWidth = 15;
        private const int inputCircleMinAngle = 135;
        private const int inputCircleSweepAngle = 270;
        private const int absIndicatorWidth = 60;

        private readonly int innerWheelWidth = 0;
        private readonly SolidBrush background;
        private readonly Pen wheelPen;
        private readonly Pen indicatorPen;
        private readonly Pen throttleBackground;
        private readonly Pen throttleForeground;
        private readonly Pen brakingBackground;
        private readonly Color brakeColor;
        private readonly Color throttleColor;

        private readonly Pen brakingForeground;

        private Font _gearIndicatorFont;

        private readonly SteeringWheelConfig _config = new SteeringWheelConfig();
        private sealed class SteeringWheelConfig : OverlayConfiguration
        {
            [ToolTip("Show throttle input indicator.")]
            public bool ShowThrottleInput { get; set; } = true;
            [ToolTip("Show brake input indicator.")]
            public bool ShowBrakeInput { get; set; } = true;
            [ToolTip("Show selected gear.")]
            public bool ShowCurrentGear { get; set; } = true;

            [ToolTip("Show a background")]
            public bool DrawBackground { get; set; } = true;

            public SteeringWheelConfig()
            {
                this.AllowRescale = true;
            }
        }

        public InputsOverlay(Rectangle rectangle) : base(rectangle, "Inputs Overlay")
        {
            this.Height = _size + 1;
            this.Width = _size + 1;

            this.innerWheelWidth = wheelWidth / 2;
            this.background = new SolidBrush(Color.FromArgb(100, Color.Black));
            this.wheelPen = new Pen(Color.FromArgb(100, 150, 150, 150), wheelWidth);
            this.indicatorPen = new Pen(Color.White, wheelWidth);

            throttleColor = Color.FromArgb(255, 10, 255, 10);
            Color throttleBackgroundColor = Color.FromArgb(50, throttleColor);
            brakeColor = Color.FromArgb(255, 255, 10, 10);
            Color brakeBackgroundColor = Color.FromArgb(50, brakeColor);

            this.throttleBackground = new Pen(throttleBackgroundColor, this.innerWheelWidth);
            this.throttleForeground = new Pen(throttleColor, this.innerWheelWidth);
            this.brakingBackground = new Pen(brakeBackgroundColor, this.innerWheelWidth);
            this.brakingForeground = new Pen(brakeColor, this.innerWheelWidth);

        }

        public sealed override void BeforeStart()
        {
            if (this._config.ShowCurrentGear)
                _gearIndicatorFont = FontUtil.FontOrbitron(40);
        }
        public sealed override void BeforeStop() { }

        public sealed override void Render(Graphics g)
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;

            if (this._config.DrawBackground)
                g.FillEllipse(background, new Rectangle(0, 0, _size, _size));

            DrawSteeringIndicator(g);

            if (this._config.ShowThrottleInput)
                DrawThrottleIndicator(g);

            if (this._config.ShowBrakeInput)
                DrawBrakingIndicator(g);

            if (this._config.ShowCurrentGear)
                DrawGearIndicator(g);
        }

        private void DrawGearIndicator(Graphics g)
        {
            string gear;
            switch (pagePhysics.Gear)
            {
                case 0: gear = "R"; break;
                case 1: gear = "N"; break;
                default: gear = $"{pagePhysics.Gear - 1}"; break;
            }

            g.TextRenderingHint = TextRenderingHint.AntiAlias;
            float gearStringWidth = g.MeasureString(gear, _gearIndicatorFont).Width;
            int xPos = (int)(_size / 2 - gearStringWidth / 2);
            int yPos = _size / 2 - _gearIndicatorFont.Height / 2;
            g.DrawStringWithShadow(gear, _gearIndicatorFont, Color.White, new PointF(xPos, yPos), 2.5f);
        }

        private void DrawBrakingIndicator(Graphics g)
        {
            int x = wheelWidth * 2 + (_config.ShowThrottleInput ? this.innerWheelWidth : 0);
            int y = wheelWidth * 2 + (_config.ShowThrottleInput ? this.innerWheelWidth : 0);
            int width = _size - (4 * wheelWidth) - (2 * (_config.ShowThrottleInput ? this.innerWheelWidth : 0));
            int height = _size - (4 * wheelWidth) - (2 * (_config.ShowThrottleInput ? this.innerWheelWidth : 0));

            DrivingAssistanceIndicator((pagePhysics.Abs == 1), this.brakingForeground, brakeColor);

            Rectangle rect = new Rectangle(x, y, width, height);
            g.DrawArc(brakingBackground, rect, inputCircleMinAngle, inputCircleSweepAngle);
            float brakeAngle = Rescale(1, inputCircleSweepAngle, pagePhysics.Brake);
            g.DrawArc(brakingForeground, rect, inputCircleMinAngle, brakeAngle);
        }

        private void DrawThrottleIndicator(Graphics g)
        {
            DrivingAssistanceIndicator((pagePhysics.TC == 1), this.throttleForeground, throttleColor);

            Rectangle rect = new Rectangle(0 + wheelWidth * 2, 0 + wheelWidth * 2, _size - (4 * wheelWidth), _size - (4 * wheelWidth));
            g.DrawArc(throttleBackground, rect, inputCircleMinAngle, inputCircleSweepAngle);
            float throttleAngle = Rescale(1, inputCircleSweepAngle, pagePhysics.Gas);
            g.DrawArc(throttleForeground, rect, inputCircleMinAngle, throttleAngle);
        }

        private void DrawSteeringIndicator(Graphics g)
        {
            g.DrawEllipse(wheelPen, _size / 2, _size / 2, _size / 2 - wheelWidth);

            float accSteering = (pagePhysics.SteerAngle / 2 + 1) * 100; // map acc value to 0 - 200
            float angle = Rescale(200, SteeringLock.Get(pageStatic.CarModel) * 2, accSteering) - (SteeringLock.Get(pageStatic.CarModel));

            // steering indicator
            Rectangle rect = new Rectangle(0 + wheelWidth, 0 + wheelWidth, _size - (2 * wheelWidth), _size - (2 * wheelWidth));
            float drawAngle = angle + 270 - (indicatorWidth / 2);
            g.DrawArc(indicatorPen, rect, drawAngle, indicatorWidth);
        }

        // map value input from range 0 - fromMax into range 0 - toMax
        private float Rescale(float fromMax, float toMax, float input)
        {
            return ((toMax) / (fromMax) * (input));
        }

        private Color SetRandomTransparency(int minTransparanxy, int maxTransparancy, Color color)
        {
            Random rd = new Random();
            int randomTransparancy = rd.Next(minTransparanxy, maxTransparancy);
            return Color.FromArgb(randomTransparancy, color);
        }

        private void DrivingAssistanceIndicator(bool active, Pen pen, Color inactiveColor)
        {
            if (active)
            {
                pen.Color = SetRandomTransparency(50, 255, inactiveColor);
            }
            else
            {
                if (pen.Color.A != 255)
                    pen.Color = inactiveColor;
            }
        }


        public sealed override bool ShouldRender()
        {
#if DEBUG
            return true;
#endif

            bool shouldRender = true;
            if (pageGraphics.Status == AcStatus.AC_OFF || pageGraphics.Status == AcStatus.AC_PAUSE || (pageGraphics.IsInPitLane == true && !pagePhysics.IgnitionOn))
                shouldRender = false;

            return shouldRender;
        }

    }
}
