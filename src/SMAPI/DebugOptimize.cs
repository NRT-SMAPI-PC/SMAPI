using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using Microsoft.Xna.Framework;
using StardewValley;

namespace StardewModdingAPI;

[HarmonyPatch]
static class DebugOptimize
{
    //static Stopwatch st = new();
    //[HarmonyPrefix]
    //[HarmonyPatch(typeof(GameRunner), MethodType.Constructor)]
    //static void GameRunnerCtor()
    //{
    //    st.Restart();
    //    Console.WriteLine("On GameRunner Ctor");
    //    if (!StardewValley.Program.releaseBuild)
    //    {
    //        Console.WriteLine("on debug mode");
    //    }
    //}

    //[HarmonyPostfix]
    //[HarmonyPatch(typeof(GameRunner), MethodType.Constructor)]
    //static void Postfix_GameRunnerCtor()
    //{
    //    st.Stop();
    //    Console.WriteLine($"On Post GameRunner Ctor total time: {st.Elapsed.TotalMilliseconds}ms");
    //}

    static Stopwatch st3 = new();
    public static bool m_skipLocalMultiplayerInitialize = true;

    [HarmonyPrefix]
    [HarmonyPatch(typeof(LocalMultiplayer), "GetStaticFieldsAndDefaults")]
    static bool LocalMultiplayer_GetStaticFieldsAndDefaults(LocalMultiplayer __instance,
        ref List<FieldInfo> ___staticFields,
        ref List<object> ___staticDefaults)
    {
        st3.Restart();
        ___staticFields = new List<FieldInfo>();
        ___staticDefaults = new List<object>();

        if (m_skipLocalMultiplayerInitialize)
        {
            Console.WriteLine("Skip LocalMultiplayer_GetStaticFieldsAndDefaults!!");
            return false;
        }

        return true;
    }

    //[HarmonyPostfix]
    //[HarmonyPatch(typeof(LocalMultiplayer), "GetStaticFieldsAndDefaults")]
    //static void Postfix_LocalMultiplayer_GetStaticFieldsAndDefaults()
    //{
    //    st3.Stop();
    //    Console.WriteLine($"postfix GetStaticFieldsAndDefaults total time: {st3.Elapsed.TotalMilliseconds}ms");
    //}

    //[HarmonyPrefix]
    //[HarmonyPatch(typeof(LocalMultiplayer), "GenerateDynamicMethodsForStatics")]
    //static bool LocalMultiplayer_GenerateDynamicMethodsForStatics()
    //{
    //    Console.WriteLine("Skip LocalMultiplayer_GenerateDynamicMethodsForStatics!!");
    //    return false;
    //}
}
