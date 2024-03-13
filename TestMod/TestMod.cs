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
    public class MyMonobehavior : MonoBehaviour
    {
    }
    
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
            PrefabManager.OnVanillaPrefabsAvailable += AddCustomItems;
            _harmony.PatchAll();
        }

        private void AddCustomItems()
        {
            AssetBundle bundle = AssetUtils.LoadAssetBundleFromResources("myweapons");
            AssetBundle swordBundle = AssetUtils.LoadAssetBundleFromResources("myswords");

            GameObject capsuleAxe = bundle.LoadAsset<GameObject>("CapsuleAxe");
            GameObject cubeSword = swordBundle.LoadAsset<GameObject>("CubeSword");

            ItemConfig cubeSwordConfig = new ItemConfig()
            {
                Name = "Кубо-меч",
                Description = "Его сложно положить в ножны...",
            };

            ItemConfig capsuleAxeConfig = new ItemConfig
            {
                Name = "Тестовый топорик",
                Description = "Рубит деревья как пиццу",
                CraftingStation = CraftingStations.Workbench,
            };
            capsuleAxeConfig.AddRequirement(new RequirementConfig("Wood", 20));

            // Mono Behavior
            //capsuleAxe.AddComponent<MyMonobehavior>();

            ItemConfig evilSwordConfig = new ItemConfig
            {
                Name = "$item_evilsword",
                Description = "$item_evilsword_desc",
                CraftingStation = CraftingStations.Workbench,
            };
            evilSwordConfig.AddRequirement(new RequirementConfig("Stone", 1));
            evilSwordConfig.AddRequirement(new RequirementConfig("Wood", 1));
            
            CustomItem capsuleAxeCustomItem = new CustomItem(capsuleAxe, true, capsuleAxeConfig);
            CustomItem evilSword = new CustomItem("EvilSword", "SwordBlackmetal", evilSwordConfig);
            CustomItem cubeSwordCustomItem = new CustomItem(cubeSword, true, cubeSwordConfig);
            
            ItemManager.Instance.AddItem(evilSword);
            ItemManager.Instance.AddItem(capsuleAxeCustomItem);
            ItemManager.Instance.AddItem(cubeSwordCustomItem);

            // You want that to run only once, Jotunn has the item cached for the game session
            PrefabManager.OnVanillaPrefabsAvailable -= AddCustomItems;
        }

        private void OnDestroy()
        {
            _harmony.UnpatchSelf();
        }

        private void Update()
        {
            if (UnityInput.Current.GetKeyDown(KeyCode.F3))
            {
                AddCustomItems();
            }
        }

        [HarmonyPatch(typeof(Tutorial), nameof(Tutorial.Update))]
        [HarmonyPatch(typeof(Tutorial), nameof(Tutorial.ShowText))]
        public class RemoveTutorialRaven
        {
            public static bool Prefix()
            {
                Jotunn.Logger.LogDebug("Deleting Raven with Jotunn!");
                Debug.Log("Deleting Raven");
                return false;
            }
        }
    }
}