using BepInEx;

using BepInEx.Configuration;
using Receiver2;
using HarmonyLib;
using UnityEngine;
using System.Collections.Generic;
using SimpleJSON;
using System;
using UnityEngine.Events;
using System.Collections;

namespace Bloons;

[BepInPlugin("Bloons", "Bloons", "1.0.0")]
public class Plugin : BaseUnityPlugin
{
    private static ConfigEntry<bool> allBalloons;
    private void Awake()
    {
        // Plugin startup logic
        Logger.LogInfo($"Balloons !");
        Harmony.CreateAndPatchAll(GetType());
        allBalloons = Config.Bind("General.Toggles", 
            "SpawnAllBalloons",
            true,
            "Spawn all balloon types in all levels.");
        
    }

    [HarmonyPatch(typeof(LocalAimHandler), "Awake")]
    [HarmonyPostfix]
    public static void LahAwake(LocalAimHandler __instance) {
        LocalAimHandler.player_instance.gameObject.AddComponent<QuickDrawListener>();
    }
    [HarmonyPatch(typeof(LocalAimHandler), "DrawMainItem")]
    [HarmonyPostfix]
    public static void DrawItem(LocalAimHandler __instance, InventoryItem.Type type) {
        QuickDrawListener qdl = __instance.gameObject.GetComponent<QuickDrawListener>();
        qdl.startQuickdraw();
    }

    private static List<string> rankSpawnTable = new List<string>{
        ".",
        ".",
        "lgn...",
        "lgna....",
        "lgsan.....",
        "lgsan....."
        // "lg",
        // "llggs",
        // "llggsa",
        // "lgsa"
    };

    private static List<string> allSpawnTable = new List<string>{
        // ".",
        // "lgsan.....",
        // "lgsan.....",
        // "lgsan.....",
        // "lgsan.....",
        // "lgsan....."
        "lgsan",
        "lgsan",
        "lgsan",
        "lgsan",
        "lgsan",
        "lgsan"

    };

    [HarmonyPatch(typeof(Balloon), "Start")]
    [HarmonyPostfix]
    public static void Start(Balloon __instance) {
        // randomize balloon type if dreaming
        if (ReceiverCoreScript.Instance().game_mode.GetGameMode() != GameMode.RankingCampaign || __instance.IsPopped)  return;
        int rank = (ReceiverCoreScript.Instance().game_mode as RankingProgressionGameMode).progression_data.receiver_rank;
        List<string> tbl = allBalloons.Value ? allSpawnTable : rankSpawnTable;
        string spawnString = tbl[rank];
        char type = spawnString[UnityEngine.Random.Range(0, spawnString.Length)];
        switch(type) {
            case 'l':
                __instance.gameObject.AddComponent<LeadBalloon>();
                break;
            case 'a':
                __instance.gameObject.AddComponent<BlinkBalloon>();
                break;
            case 'g':
                __instance.gameObject.AddComponent<GhostBalloon>();
                break;
            case 's':
                __instance.gameObject.AddComponent<SpawnBalloon>();
                break;
            case 'n':
                __instance.gameObject.AddComponent<SnipeBalloon>();
                break;
            default:
                break;
        }
    }
    
    [HarmonyPatch(typeof(Balloon), "WasShot")]
    [HarmonyPrefix]
    public static bool WasShot(Balloon __instance, ref ShootableQuery shootable_query) {
        BaseBalloon baseBalloon = __instance.GetComponent<BaseBalloon>();

        if (baseBalloon == null) {
            return true;
        }
        if (!__instance.IsPopped ) {
            return baseBalloon.OnShoot(ref __instance, ref shootable_query);
        }
        return false;
    }

    [HarmonyPatch(typeof(Balloon), "SetPersistentData")]
    [HarmonyPostfix]
    public static void SetPersistentData(Balloon __instance, JSONObject data){
        try {
            if (__instance == null || __instance.IsPopped) {return;}
            BaseBalloon baseBalloon = __instance.gameObject.GetComponent<BaseBalloon>();
            baseBalloon.SetPersistentData(ref __instance, data);
        } catch (Exception ex) {
            Debug.Log("Caught exception in SetPersistentData "+ ex.ToString());
        }
    }
    
    [HarmonyPatch(typeof(Balloon), "GetPersistentData")]
    [HarmonyPostfix]
    public static void GetPersistentData(Balloon __instance,  JSONObject __result){
        try {
            if (__instance == null || __instance.IsPopped) {return;}
            BaseBalloon baseBalloon = __instance.gameObject.GetComponent<BaseBalloon>();
            baseBalloon.GetPersistentData(ref __instance, ref __result);
        } catch (Exception ex) {
            Debug.Log("Caught exception in GetPersistentData "+ ex.ToString());
        }
    }

    
    [HarmonyPatch(typeof(Balloon), "OnTriggerEnter")]
    [HarmonyPrefix]
    public static bool OnTriggerEnter(Balloon __instance, Collider other) {
        if (!__instance.IsPopped) {
            BaseBalloon baseBalloon = __instance.gameObject.GetComponent<BaseBalloon>();
            if (baseBalloon != null) return baseBalloon.OnTouch(ref __instance, other);
        }
        return true;
    }

    
}
