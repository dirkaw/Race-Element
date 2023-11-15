﻿using RaceElement.Data.ACC.Cars;
using RaceElement.Data.ACC.Database.LapDataDB;
using RaceElement.Data.ACC.Tracker.Laps;
using RaceElement.HUD.Overlay.Configuration;
using RaceElement.HUD.Overlay.Internal;
using RaceElement.HUD.Overlay.OverlayUtil;
using RaceElement.HUD.Overlay.OverlayUtil.Drawing;
using RaceElement.HUD.Overlay.Util;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace RaceElement.HUD.ACC.Overlays.OverlayCarInfo
{
    [Overlay(Name = "Car Info", Version = 1.00, OverlayType = OverlayType.Release,
        Description = "A panel showing the damage time. Optionally showing current tyre set, fuel per lap, exhaust temp and water temp.",
        OverlayCategory = OverlayCategory.Car)]
    internal sealed class CarInfoOverlay : AbstractOverlay
    {
        private readonly CarInfoConfiguration _config = new CarInfoConfiguration();
        private sealed class CarInfoConfiguration : OverlayConfiguration
        {
            [ConfigGrouping("Info Panel", "Show or hide additional information in the panel.")]
            public InfoPanelGrouping InfoPanel { get; set; } = new InfoPanelGrouping();
            public class InfoPanelGrouping
            {
                [ToolTip("Displays your current tyre set")]
                public bool TyreSet { get; set; } = true;

                [ToolTip("Displays the average fuel usage over the past 3 laps.\n(uses last lap info if not enough data)")]
                public bool FuelPerLap { get; set; } = false;

                [ToolTip("Displays the exhaust temperature.")]
                public bool ExhaustTemp { get; set; } = false;

                [ToolTip("Displays the water temperature of the engine.")]
                public bool WaterTemp { get; set; } = false;
            }

            public CarInfoConfiguration() => this.AllowRescale = true;
        }

        private Font _font;
        private DrawableTextCell _damageValue1;
        private DrawableTextCell _tyreSetValue1;
        private DrawableTextCell _fuelPerLapValue1;
        private DrawableTextCell _exhaustValue1;
        private DrawableTextCell _waterValue1;

        private GraphicsGrid _graphicsGrid;

        public CarInfoOverlay(Rectangle rectangle) : base(rectangle, "Car Info")
        {
            this.RefreshRateHz = 1;
        }

        public sealed override void BeforeStart()
        {
            _font = FontUtil.FontSegoeMono(10f * this.Scale);

            int rows = 1;

            if (_config.InfoPanel.TyreSet) rows++;
            if (_config.InfoPanel.FuelPerLap) rows++;
            if (_config.InfoPanel.ExhaustTemp) rows++;
            if (_config.InfoPanel.WaterTemp) rows++;
            _graphicsGrid = new GraphicsGrid(rows, 2);

            float fontHeight = (int)(_font.GetHeight(120));
            int columnHeight = (int)(fontHeight - 2f * Scale);
            int headerColumnWidth = (int)Math.Ceiling(76f * Scale);
            int valueColumnWidth = (int)Math.Ceiling(56f * Scale);
            float roundingRadius = 6f * Scale;
            RectangleF headerRectangle = new RectangleF(0, 0, headerColumnWidth, columnHeight);
            RectangleF valueRectangle = new RectangleF(headerColumnWidth, 0, valueColumnWidth, columnHeight);

            // create value and header backgrounds
            Color accentColor = Color.FromArgb(25, 255, 0, 0);
            CachedBitmap headerBackground = new CachedBitmap(headerColumnWidth, columnHeight, g =>
            {
                Rectangle panelRect = new Rectangle(0, 0, headerColumnWidth, columnHeight);
                using GraphicsPath path = GraphicsExtensions.CreateRoundedRectangle(panelRect, 0, 0, 0, (int)roundingRadius);
                g.FillPath(new SolidBrush(Color.FromArgb(225, 10, 10, 10)), path);
                g.DrawLine(new Pen(accentColor), 0 + roundingRadius / 2, columnHeight, headerColumnWidth, columnHeight - 1 * Scale);
            });
            CachedBitmap valueBackground = new CachedBitmap(valueColumnWidth, columnHeight, g =>
            {
                Rectangle panelRect = new Rectangle(0, 0, valueColumnWidth, columnHeight);
                using GraphicsPath path = GraphicsExtensions.CreateRoundedRectangle(panelRect, 0, (int)roundingRadius, 0, 0);
                g.FillPath(new SolidBrush(Color.FromArgb(225, 0, 0, 0)), path);
                g.DrawLine(new Pen(accentColor), 0, columnHeight - 1 * Scale, valueColumnWidth, columnHeight - 1 * Scale);
            });

            // add data rows

            // Damage
            int currentRow = 0;
            DrawableTextCell damageHeader = new DrawableTextCell(headerRectangle, _font);
            damageHeader.CachedBackground = headerBackground;
            damageHeader.StringFormat.Alignment = StringAlignment.Near;
            damageHeader.UpdateText("Damage");
            _graphicsGrid.Grid[currentRow][0] = damageHeader;
            _damageValue1 = new DrawableTextCell(valueRectangle, _font);
            _damageValue1.CachedBackground = valueBackground;
            _damageValue1.StringFormat.Alignment = StringAlignment.Far;
            _graphicsGrid.Grid[currentRow][1] = _damageValue1;

            if (_config.InfoPanel.TyreSet)
            {
                currentRow++;

                headerRectangle.Offset(0, columnHeight);
                valueRectangle.Offset(0, columnHeight);

                DrawableTextCell header = new DrawableTextCell(headerRectangle, _font);
                header.CachedBackground = headerBackground;
                header.StringFormat.Alignment = StringAlignment.Near;
                header.UpdateText("Tyre set");
                _graphicsGrid.Grid[currentRow][0] = header;

                _tyreSetValue1 = new DrawableTextCell(valueRectangle, _font);
                _tyreSetValue1.CachedBackground = valueBackground;
                _tyreSetValue1.StringFormat.Alignment = StringAlignment.Far;
                _graphicsGrid.Grid[currentRow][1] = _tyreSetValue1;
            }

            if (_config.InfoPanel.FuelPerLap)
            {
                currentRow++;
                headerRectangle.Offset(0, columnHeight);
                valueRectangle.Offset(0, columnHeight);

                DrawableTextCell header = new DrawableTextCell(headerRectangle, _font);
                header.CachedBackground = headerBackground;
                header.StringFormat.Alignment = StringAlignment.Near;
                header.UpdateText("Fuel/Lap");
                _graphicsGrid.Grid[currentRow][0] = header;

                _fuelPerLapValue1 = new DrawableTextCell(valueRectangle, _font);
                _fuelPerLapValue1.CachedBackground = valueBackground;
                _fuelPerLapValue1.StringFormat.Alignment = StringAlignment.Far;
                _graphicsGrid.Grid[currentRow][1] = _fuelPerLapValue1;
            }

            if (_config.InfoPanel.ExhaustTemp)
            {
                currentRow++;
                headerRectangle.Offset(0, columnHeight);
                valueRectangle.Offset(0, columnHeight);

                DrawableTextCell header = new DrawableTextCell(headerRectangle, _font);
                header.CachedBackground = headerBackground;
                header.StringFormat.Alignment = StringAlignment.Near;
                header.UpdateText("Exhaust");
                _graphicsGrid.Grid[currentRow][0] = header;

                _exhaustValue1 = new DrawableTextCell(valueRectangle, _font);
                _exhaustValue1.CachedBackground = valueBackground;
                _exhaustValue1.StringFormat.Alignment = StringAlignment.Far;
                _graphicsGrid.Grid[currentRow][1] = _exhaustValue1;
            }

            if (_config.InfoPanel.WaterTemp)
            {
                currentRow++;
                headerRectangle.Offset(0, columnHeight);
                valueRectangle.Offset(0, columnHeight);

                DrawableTextCell header = new DrawableTextCell(headerRectangle, _font);
                header.CachedBackground = headerBackground;
                header.StringFormat.Alignment = StringAlignment.Near;
                header.UpdateText("Water");
                _graphicsGrid.Grid[currentRow][0] = header;

                _waterValue1 = new DrawableTextCell(valueRectangle, _font);
                _waterValue1.CachedBackground = valueBackground;
                _waterValue1.StringFormat.Alignment = StringAlignment.Far;
                _graphicsGrid.Grid[currentRow][1] = _waterValue1;
            }

            // set HUD Width + Height based on amount of rows and columns
            Width = (int)(headerColumnWidth + valueColumnWidth);
            Height = (int)(rows * columnHeight);
        }

        public override void BeforeStop()
        {
            _font?.Dispose();
            _graphicsGrid?.Dispose();
        }

        public sealed override void Render(Graphics g)
        {
            float totalRepairTime = Damage.GetTotalRepairTime(pagePhysics);
            _damageValue1.TextBrush = totalRepairTime > 0 ? Brushes.OrangeRed : Brushes.White;
            _damageValue1.UpdateText($"{totalRepairTime:F1}");

            if (_config.InfoPanel.TyreSet)
                _tyreSetValue1.UpdateText($"{pageGraphics.currentTyreSet}");

            if (_config.InfoPanel.FuelPerLap)
            {
                float fuelXLap = LapTracker.Instance.Laps.GetAverageFuelUsage(3);
                if (fuelXLap != -1)
                    fuelXLap /= 1000f;
                else fuelXLap = pageGraphics.FuelXLap;
                _fuelPerLapValue1.UpdateText($"{fuelXLap:F3}");
            }

            if (_config.InfoPanel.ExhaustTemp)
                _exhaustValue1.UpdateText($"{pageGraphics.ExhaustTemperature:F0} C");

            if (_config.InfoPanel.WaterTemp)
                _waterValue1.UpdateText($"{pagePhysics.WaterTemp:F0} C");

            _graphicsGrid?.Draw(g);
        }
    }
}
