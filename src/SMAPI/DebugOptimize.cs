using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Serialization;
using HarmonyLib;
using Microsoft.Xna.Framework;
using StardewModdingAPI.Framework;
using StardewValley;
using StardewValley.SaveSerialization;

namespace StardewModdingAPI;

[HarmonyPatch]
static class DebugOptimize
{
    public static bool EnableOptimizeStartup = false;


    static Stopwatch st3 = new();
    public static bool EnableSkipLocalMultiplayerInitialize = true;

    [HarmonyPrefix]
    [HarmonyPatch(typeof(LocalMultiplayer), "GetStaticFieldsAndDefaults")]
    static bool LocalMultiplayer_GetStaticFieldsAndDefaults(LocalMultiplayer __instance,
        ref List<FieldInfo> ___staticFields,
        ref List<object> ___staticDefaults)
    {
        st3.Restart();
        ___staticFields = new List<FieldInfo>();
        ___staticDefaults = new List<object>();

        if (EnableSkipLocalMultiplayerInitialize)
        {
            Console.WriteLine("Skip LocalMultiplayer_GetStaticFieldsAndDefaults!!");
            return false;
        }

        return true;
    }

    static Task? m_taskFarmerXmlSerializer;

    static IMonitor monitor => SCore.Instance.GetMonitorForGame();
    [HarmonyPrefix]
    [HarmonyPatch(typeof(Game1), nameof(Game1.InitializeSerializers))]
    public static bool Prefix_Game1_InitializeSerializers()
    {
        if (!EnableOptimizeStartup)
            return false;

        var mon = monitor;
        mon.Log("calling Prefix_Game1_InitializeSerializers...");
        if (m_taskFarmerXmlSerializer != null)
            return false;

        mon.Log("running m_taskFarmerXmlSerializer...");
        m_taskFarmerXmlSerializer = Task.Run(() =>
        {
            var st = Stopwatch.StartNew();
            var fs = SaveSerializer.GetSerializer(typeof(Farmer));
            mon.Log($"done m_taskFarmerXmlSerializer time: {st.Elapsed.TotalMilliseconds}ms");
        });
        StartupPreferences.serializer = SaveSerializer.GetSerializer(typeof(StartupPreferences));
        return false;
    }

    internal static void WaitTaskFarmerXmlSerializer()
    {
        if (!EnableOptimizeStartup)
            return;

        var mon = monitor;
        mon.Log("WaitTaskFarmerXmlSerializer...");
        m_taskFarmerXmlSerializer?.Wait();
        Game1.otherFarmers.Serializer = SaveSerializer.GetSerializer(typeof(Farmer));
        mon.Log("success apply otherFarmers.Serializer!");
    }
}
