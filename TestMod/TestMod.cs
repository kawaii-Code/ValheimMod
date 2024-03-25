using System.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using BepInEx;
using HarmonyLib;
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

        public static Piece Toilet;

        private static RuntimeAnimatorController _controller;
        private static GameObject _guitarPrefab;

        private static List<AudioClip> _clips;
        
        private static Player _localPlayer;
        
        private void Awake()
        {
            PrefabManager.OnVanillaPrefabsAvailable += LoadMod;

            AddPizzaBundle();
            AddGuitarBundle();

            ItemConfig config = new ItemConfig();
            config.Amount = 2;
            config.Description = "This is a test item";
            config.Name = "Test Item";

            CustomItem item = new CustomItem("SpawnPoint.prefab", true, config);
            item.FixReferences();
            _harmony.PatchAll();
        }

        private void AddCustomCreatures(GameObject spine)
        {
            AssetBundle sanekBundle = AssetUtils.LoadAssetBundleFromResources("mycreatures");
            GameObject sanekPrefab = sanekBundle.LoadAsset<GameObject>("Sanek");
            CreatureConfig sanekConfig = new CreatureConfig();

            sanekConfig.Name = "Санёк";
            sanekConfig.Faction = Character.Faction.PlainsMonsters;
            sanekConfig.AddSpawnConfig(new SpawnConfig()
            {
                Name = "SanekSpawn",
                SpawnChance = 100f,
                SpawnInterval = 1f,
                SpawnDistance = 1f,
                Biome = Heightmap.Biome.Meadows,
                MaxSpawned = 5,
            });
            sanekConfig.AddDropConfig(new DropConfig()
            {
                Chance = 100f,
                Item = "Spine",
                LevelMultiplier = false,
                MinAmount = 1,
                MaxAmount = 2,
                OnePerPlayer = false,
            });
            CustomCreature sanek = new CustomCreature(sanekPrefab, false, sanekConfig);
            
            CreatureManager.Instance.AddCreature(sanek);
        }

        private void AddGuitarBundle()
        {
            AssetBundle bundle = AssetUtils.LoadAssetBundleFromResources("guitar");
            Debug.LogError(bundle == null);
            GameObject guitar = bundle.LoadAsset<GameObject>("guitar");
            _controller = bundle.LoadAsset<RuntimeAnimatorController>("CustomAnimationController");
            _guitarPrefab = bundle.LoadAsset<GameObject>("guitar2");
            Debug.LogError($"guitar - {_guitarPrefab == null}");
            Debug.LogError("Controller - " + (_controller == null));

            var clips = new List<AudioClip>();
            
            for (int i = 1; i < 5; i++) 
                clips.Add(bundle.LoadAsset<AudioClip>($"clip{i}"));

            _clips = clips;
            
            ItemConfig guitarConfig = new ItemConfig
            {
                Name = "Гитара",
                Description = "Врагов бей, но струны не ломай",
                CraftingStation = CraftingStations.Workbench,
            };

            CustomItem guitarCustomItem = new CustomItem(guitar, true, guitarConfig);
            ItemManager.Instance.AddItem(guitarCustomItem);
            PrefabManager.Instance.AddPrefab(guitar);
        }

        private void LoadMod()
        {
            PrefabManager.OnVanillaPrefabsAvailable -= LoadMod;
            
            AssetBundle furnitureBundle = AssetUtils.LoadAssetBundleFromResources("myfurniture");
            GameObject toilet = furnitureBundle.LoadAsset<GameObject>("Toilet");
            Toilet = toilet.GetComponent<Piece>();
            PrefabManager.Instance.AddPrefab(toilet);

            AssetBundle itemsBundle = AssetUtils.LoadAssetBundleFromResources("myitems");
            GameObject spine = itemsBundle.LoadAsset<GameObject>("Spine");
            PrefabManager.Instance.AddPrefab(spine);
            
            AddCustomCreatures(spine);
        }

        private void AddPizzaBundle()
        {
            AssetBundle bundle = AssetUtils.LoadAssetBundleFromResources("cum");
            Debug.LogError(bundle == null);
            GameObject pizza = bundle.LoadAsset<GameObject>("Pizza");
            PrefabManager.Instance.AddPrefab(pizza);
        }

        private static bool DevcommandsEnabled = false;

        private void OnDestroy()
        {
            _harmony.UnpatchSelf();
        }

        [HarmonyPatch(typeof(Character), nameof(Character.CheckRun))]
        public class StaminaPatch
        {
            public static void Prefix(ref Character __instance)
            {
            }
        }

        [HarmonyPatch(typeof(Player), nameof(Game.Start))]
        public class AnimatorSetPatch
        {
            public static void Postfix(ref Player __instance)
            {

                Cum();

                /*var provider = _localPlayer.gameObject.AddComponent<CustomAnimationsProvider>();
                Debug.LogError(_localPlayer.gameObject.name);
                provider.Init(_controller, _guitarPrefab, _localPlayer.gameObject.transform, _clips);*/
            }
        }

        private static async void Cum()
        {
            while (_localPlayer == null)
            {
                _localPlayer = Player.m_localPlayer;
                await Task.Delay(100);
            }
            
            Debug.LogError("Hell yeah");
            
            var provider = _localPlayer.gameObject.AddComponent<CustomAnimationsProvider>();
            Debug.LogError(_localPlayer.gameObject.name);
            provider.Init(_controller, _guitarPrefab, _localPlayer.gameObject.transform, _clips);
        } 
        
        [HarmonyPatch(typeof(PieceTable), nameof(PieceTable.UpdateAvailable))]
        public class AddToilet
        {
            public static void Postfix(ref List<List<Piece>> ___m_availablePieces)
            {
                ___m_availablePieces[(int)Toilet.m_category].Add(Toilet);
                Debug.Log("Adding toilet...");
            }
        }

        [HarmonyPatch(typeof(Player), nameof(Player.Update))]
        public class PlayerPatch
        {
            public static void Prefix()
            {
                if (!DevcommandsEnabled)
                {
                    Debug.Log("Enabling devcommands...");
                    Console.instance.TryRunCommand("devcommands");
                    DevcommandsEnabled = true;
                }
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
    }
}