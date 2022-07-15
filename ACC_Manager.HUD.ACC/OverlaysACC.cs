﻿using ACCManager.HUD.ACC.Overlays.OverlayAccelerometer;
using ACCManager.HUD.ACC.Overlays.OverlayCarInfo;
using ACCManager.HUD.ACC.Overlays.OverlayDebugInfo.OverlayBroadcastRealtime;
using ACCManager.HUD.ACC.Overlays.OverlayEcuMapInfo;
using ACCManager.HUD.ACC.Overlays.OverlayFuelInfo;
using ACCManager.HUD.ACC.Overlays.OverlayGraphicsInfo;
using ACCManager.HUD.ACC.Overlays.OverlayInputTrace;
using ACCManager.HUD.ACC.Overlays.OverlayLapDelta;
using ACCManager.HUD.ACC.Overlays.OverlayPhysicsInfo;
using ACCManager.HUD.ACC.Overlays.OverlayPressureTrace;
using ACCManager.HUD.ACC.Overlays.OverlayStaticInfo;
using ACCManager.HUD.ACC.Overlays.OverlayInputs;
using ACCManager.HUD.ACC.Overlays.OverlayTrackInfo;
using ACCManager.HUD.ACC.Overlays.OverlayTyreInfo;
using ACCManager.HUD.ACC.Overlays.OverlayWeather;
using ACCManager.HUD.Overlay.Internal;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ACCManager.HUD.ACC.Overlays.OverlayDebugInfo.OverlayEntryList;
using ACCManager.HUD.ACC.Overlays.OverlayDebugInfo.OverlayDebugOutput;
using ACCManager.HUD.ACC.Overlays.OverlayShiftIndicator;
using ACCManager.HUD.ACC.Overlays.OverlayRefuel;
using System.Reflection;
using System.Diagnostics;

namespace ACCManager.HUD.ACC
{
    public class OverlaysACC
    {
        public static SortedDictionary<string, Type> AbstractOverlays = new SortedDictionary<string, Type>()
        {
            {"Accelerometer", typeof(AccelerometerOverlay) },
            {"Car Info", typeof(CarInfoOverlay) },
            {"ECU Maps", typeof(EcuMapOverlay) },
            {"Fuel Info", typeof(FuelInfoOverlay) },
            {"Inputs", typeof(InputsOverlay) },
            {"Input Trace", typeof(InputTraceOverlay) },
            {"Lap Delta", typeof(LapDeltaOverlay) },
            {"Shift Indicator", typeof(ShiftIndicatorOverlay) },
            {"Track Info", typeof(TrackInfoOverlay) },
            {"Tyre Info", typeof(TyreInfoOverlay) },
            {"Tyre Pressure Trace", typeof(PressureTraceOverlay) },


            {"Debug Output", typeof(DebugOutputOverlay) },
#if DEBUG
            {"Weather Info", typeof(WeatherOverlay) },

            // yea this shit has to be at the bottom...
            {"Page Static", typeof(StaticInfoOverlay) },
            {"Page Physics", typeof(PhysicsInfoOverlay) },
            {"Page Graphics", typeof(GraphicsInfoOverlay) },
            {"Broadcast Entrylist", typeof(EntryListOverlay) },
            {"Broadcast Realtime", typeof(BroadcastRealtimeOverlay) },
            {"Broadcast Trackdata", typeof(BroadcastTrackDataOverlay) },
            {"Refuel Info", typeof(RefuelInfoOverlay) },
#endif
        };

        public static List<AbstractOverlay> ActiveOverlays = new List<AbstractOverlay>();

        public static void GenerateDictionary()
        {
            AbstractOverlays.Clear();

            foreach (var type in Assembly.GetExecutingAssembly().GetTypes().Where(x => x.IsDefined(typeof(OverlayAttribute))))
            {
                var overlayType = type.GetCustomAttribute<OverlayAttribute>();
                if (overlayType != null)
                {
                    if (!AbstractOverlays.ContainsKey(overlayType.Name))
                    {
                        Debug.WriteLine($"Found {overlayType.Name} - {overlayType.OverlayType}");
                        AbstractOverlays.Add(overlayType.Name, type);
                    }
                }
            }
        }

        public static void CloseAll()
        {
            lock (ActiveOverlays)
                while (ActiveOverlays.Count > 0)
                {
                    ActiveOverlays.ElementAt(0).EnableReposition(false);
                    ActiveOverlays.ElementAt(0).Stop();
                    ActiveOverlays.ElementAt(0).Dispose();
                    ActiveOverlays.Remove(ActiveOverlays.ElementAt(0));
                }
        }
    }
}
