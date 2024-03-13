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
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class CobbleDropZone : MonoBehaviour
    {
        private float _dropDelay = 0.5f;
        private Cobble _cobble;

        private void Start()
        {
            _cobble = FindObjectOfType<Cobble>();
            Debug.LogError($"cobble - {_cobble == null}");
        }
    
        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.TryGetComponent(out Player player) == false || other.gameObject.TryGetComponent(out Humanoid player2) == false)
                return;

            _cobble.transform.position = new Vector3(transform.position.x, transform.position.y + 10, transform.position.z);
        
            StartCoroutine(DelayedDrop());
        }

        private IEnumerator DelayedDrop()
        {
            yield return new WaitForSecondsRealtime(_dropDelay);

            _cobble.Drop();
        }
    }
    
    public class Cobble : MonoBehaviour
    {
        private bool _dropOnAwake;
        private Rigidbody _rigidbody;
 
        public void Init(bool dropOnAwake)
        {
            _dropOnAwake = dropOnAwake;
        }
    
        private void OnCollisionEnter(Collision other)
        {
            Debug.LogError(other.gameObject.name);
        
            if(other.gameObject.TryGetComponent(out Humanoid player) == false)
                return;

            Debug.LogError("Player entered");
        
            HitData hitData = new HitData();
            hitData.m_damage.m_damage = 99999f;
            hitData.m_hitType = HitData.HitType.EdgeOfWorld;
            player.Damage(hitData);
            
            GetComponent<AudioSource>().Play();
        }
    
        private void Awake()
        {
            if(_dropOnAwake)
                _rigidbody.isKinematic = false;
        
            _rigidbody = GetComponent<Rigidbody>();
        }

        [ContextMenu("Drop")]
        public void Drop()
        {
            _rigidbody.isKinematic = false;
        }
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
            ItemConfig config = new ItemConfig();
            config.Amount = 2;
            AddPizzaBundle();
            AddGuitarBundle();
            config.Description = "This is a test item";
            config.Name = "Test Item";
            CustomItem item = new CustomItem("SpawnPoint.prefab", true, config);
            item.FixReferences();
            _harmony.PatchAll();
        }

        private void AddGuitarBundle()
        {   
            
            AssetBundle bundle = AssetUtils.LoadAssetBundleFromResources("guitar");
            Debug.LogError(bundle == null);
            GameObject guitar = bundle.LoadAsset<GameObject>("guitar");

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