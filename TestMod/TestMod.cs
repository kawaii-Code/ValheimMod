using System;
using System.Collections.Specialized;
using System.Reflection;
using BepInEx;
using HarmonyLib;
using UnityEngine;
using Jotunn;
using Jotunn.Configs;
using Jotunn.Entities;
using Jotunn.Managers;
using Jotunn.Utils;

namespace TestMod
{
    [BepInPlugin(TestModGuid, "Test Mod", "1.0.0")]
    [BepInProcess("valheim.exe")]
    [BepInDependency(Main.ModGuid)]
    [NetworkCompatibility(CompatibilityLevel.EveryoneMustHaveMod, VersionStrictness.Minor)]
    public class TestMod : BaseUnityPlugin
    {
        private const string TestModGuid = "test.mod";
        private readonly Harmony _harmony = new Harmony(TestModGuid);

        private void Awake()
        {
            ItemConfig config = new ItemConfig();
            config.Amount = 2;
            AddPizzaBundle();
            config.Description = "This is a test item";
            config.Name = "Test Item";
            CustomItem item = new CustomItem("SpawnPoint.prefab", true, config);
            item.FixReferences();
            _harmony.PatchAll();
        }

        private void AddPizzaBundle()
        {
            AssetBundle bundle = AssetUtils.LoadAssetBundleFromResources("cum");
            Debug.LogError(bundle == null);
            GameObject pizza = bundle.LoadAsset<GameObject>("Pizza");
            PrefabManager.Instance.AddPrefab(pizza);
        }
        
        private static bool CursorShown;

        private void Update()
        {
            if (UnityInput.Current.GetKeyDown(KeyCode.F3))
            {
                
                CursorShown = !CursorShown;
            }
        }
        

        private void OnDestroy()
        {
            _harmony.UnpatchSelf();
        }

        [HarmonyPatch(typeof(Character), nameof(Character.Awake))]
        public class ChubPatch
        {
            public static void Postfix()
            {
               
            }
        }
        
        [HarmonyPatch(typeof(Character), nameof(Character.CheckRun))]
        public class StaminaPatch
        {
            public static void Prefix(ref Character __instance)
            {
            }
        }
        
        [HarmonyPatch(typeof(Character), nameof(Character.Jump))]
        public class JumpPatch
        {
            public static void Prefix(ref Character __instance)
            {
                __instance.m_boss = true;
                Debug.Log(__instance.m_boss);
                //__instance.SetHealth(100);
            }
        }

        
        [HarmonyPatch(typeof(ItemDrop), nameof(ItemDrop.Pickup))]
        public class CumPatch
        {
            public static void Postfix(ref ItemDrop __instance)
            {
                if(__instance.gameObject.name != "Pizza")
                    return;
                
                Debug.LogError("булыжник дропается примерно тут");
            }
        }
        
        [HarmonyPatch(typeof(GameCamera), nameof(GameCamera.UpdateMouseCapture))]
        public class MainCameraPatch
        {
            public static void Postfix()
            {
                if (CursorShown)
                {
                    Cursor.lockState = CursorLockMode.None;
                    Cursor.visible = true;
                }
                else
                {
                    Cursor.lockState = CursorLockMode.Locked;
                    Cursor.visible = false;
                }
            }
        }
    }
}