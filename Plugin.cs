﻿#if BEPINEX6
using BepInEx.Unity.Mono;
#endif
using BepInEx.Logging;
using BepInEx;
using HarmonyLib;
using System.Reflection;
using System.Linq;
using System.Collections.Generic;

namespace Wayz.CS2.SchoolCapacityBalancer;

[BepInPlugin("Wayz.CS2.SchoolCapacityBalancer", "SchoolCapacityBalancer", "0.1.1")]
public class SchoolCapacityBalancer : BaseUnityPlugin
{
    public static ManualLogSource GameLogger = null!;

    public static IReadOnlyDictionary<string, SchoolOptions> SchoolOptions { get; private set; } = null!;

    private void Awake()
    {
        GameLogger = Logger;

        SchoolCapacityBalancerOptions options;
        if (!WayzSettingsManager.TryGetSettings<SchoolCapacityBalancerOptions>("SchoolCapacityBalancer_Wayz", "settings", out options))
        {
            options = SchoolCapacityBalancerOptions.Default;
            try
            {
                WayzSettingsManager.SaveSettings("SchoolCapacityBalancer_Wayz", "settings", options);
            }
            catch
            {
                Logger.LogWarning("Failed to save default config to settings file, using in-memory default config.");
            }
        }

        var filtered = options.RemoveBadEntires();
        if(filtered != 0)
        {
            Logger.LogWarning($"Removed {filtered} bad entries loaded from settings file!");
        }
        SchoolCapacityBalancer.SchoolOptions = options.ToDictionary();

        var harmony = Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly(), MyPluginInfo.PLUGIN_GUID + "_Cities2Harmony");

        var patchedMethods = harmony.GetPatchedMethods();

        Logger.LogInfo($"Plugin {MyPluginInfo.PLUGIN_GUID} {MyPluginInfo.PLUGIN_VERSION} is loaded! Patched methods: " + patchedMethods.Count());

        foreach (var patchedMethod in patchedMethods)
        {
            Logger.LogInfo($"Patched method: {patchedMethod.Module.Name}:{patchedMethod.Name}");
        }
    }
}