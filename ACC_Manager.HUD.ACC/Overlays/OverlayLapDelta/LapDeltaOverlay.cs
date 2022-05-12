﻿using ACCManager.HUD.Overlay.Internal;
using ACCManager.HUD.Overlay.OverlayUtil;
using ACCManager.HUD.Overlay.Util;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ACCManager.HUD.ACC.Overlays.OverlayLapDelta
{
    internal class LapDeltaOverlay : AbstractOverlay
    {
        private const int overlayWidth = 200;
        private int overlayHeight = 150;

        private LapTimeTracker collector;
        private LapTimingData lastLap = null;

        InfoPanel panel = new InfoPanel(10, overlayWidth);
        public LapDeltaOverlay(Rectangle rectangle) : base(rectangle, "Lap Delta Overlay")
        {
            overlayHeight = panel.FontHeight * 4;

            this.Width = overlayWidth + 1;
            this.Height = overlayHeight + 1;
            RefreshRateHz = 10;
        }

        public override void BeforeStart()
        {
            collector = LapTimeTracker.Instance;
            collector.LapFinished += Collector_LapFinished;
        }

        public override void BeforeStop()
        {
            collector.LapFinished -= Collector_LapFinished;
            collector.Stop();
        }

        private void Collector_LapFinished(object sender, LapTimingData e)
        {
            lastLap = e;
        }

        public override void Render(Graphics g)
        {
            string deltaString = pageGraphics.IsDeltaPositive ? "+" : "-";
            panel.AddLine("Delta", $"{deltaString}{pageGraphics.DeltaLapTime}");

            string sector1 = "-";
            string sector2 = "-";
            string sector3 = "-";
            if (collector.CurrentLap.Sector1 > -1)
            {
                sector1 = $"{((float)collector.CurrentLap.Sector1 / 1000):F3}";
            }
            else if (pageGraphics.CurrentSectorIndex == 0)
                sector1 = $"{((float)pageGraphics.CurrentTimeMs / 1000):F3}";


            if (collector.CurrentLap.Sector2 > -1)
                sector2 = $"{((float)collector.CurrentLap.Sector2 / 1000):F3}";
            else if (collector.CurrentLap.Sector1 > -1)
            {
                sector2 = $"{(((float)pageGraphics.CurrentTimeMs - collector.CurrentLap.Sector1) / 1000):F3}";
            }

            if (collector.CurrentLap.Sector3 > -1)
                sector3 = $"{((float)collector.CurrentLap.Sector3 / 1000):F3}";
            else if (collector.CurrentLap.Sector2 > -1)
            {
                sector3 = $"{(((float)pageGraphics.CurrentTimeMs - collector.CurrentLap.Sector2 - collector.CurrentLap.Sector1) / 1000):F3}";
            }

            panel.AddLine("S1", $"{sector1}");
            panel.AddLine("S2", $"{sector2}");
            panel.AddLine("S3", $"{sector3}");
            panel.Draw(g);


            Pen isbetterPen = Pens.Green;
            if (pageGraphics.IsDeltaPositive || !pageGraphics.IsValidLap)
                isbetterPen = Pens.Red;

            g.DrawRoundedRectangle(isbetterPen, new Rectangle(0, 0, overlayWidth, overlayHeight), 3);
        }




        //public bool IsLastSectorFastest(Dictionary<int, int> sectorTimes)
        //{
        //    if (sectorTimes.Count == 0) return false;
        //    if (!pageGraphics.IsValidLap) return false;

        //    int sectorTime = sectorTimes.Last().Value;
        //    if (sectorTime < 0) { return false; }

        //    foreach (KeyValuePair<int, int> kvp in sectorTimes)
        //    {
        //        if (collector.LapValids.ContainsKey(kvp.Key))
        //            if (collector.LapValids[kvp.Key])
        //                if (sectorTime > kvp.Value)
        //                    return false;
        //    }

        //    return true;
        //}

        public override bool ShouldRender()
        {
#if DEBUG
            return true;
#endif
            return false;
        }
    }
}
