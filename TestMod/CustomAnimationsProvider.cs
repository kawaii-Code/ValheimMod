using System.Collections.Generic;
using UnityEngine;

namespace TestMod
{
    public class CustomAnimationsProvider : MonoBehaviour
    {
        private const string AnimName = "Guitar Playing";
        private Animator _animator;
        private CharacterAnimEvent _animEvent;
        private AnimationEffect _animationEffect;

        private RuntimeAnimatorController _controller;
        private RuntimeAnimatorController _oldController;
        private PlayerController _playerController;
        private GameObject _guitar;
        private Transform _playerRoot;
        private GameObject _guitarInstance;

        private bool _isPlaying;

        private List<AudioClip> _clips;

        private AudioSource _audioSource;
    
#if UNITY_EDITOR
    [SerializeField] private RuntimeAnimatorController _testController;
    [SerializeField] private GameObject _testGuitar;
    [SerializeField] private Transform _testRoot;
    [SerializeField] private List<AudioClip> _clipsTest;

    private void Awake()
    {
        Init(_testController, _testGuitar, _testRoot, _clipsTest);
    }
#endif

        public void Init(RuntimeAnimatorController controller, GameObject guitar, Transform root, List<AudioClip> clips)
        {
            _clips = clips;
            _controller = controller;
            _guitar = guitar;
            _playerRoot = root;
        
            var obj = transform.Find("Visual");
            _animator = obj.GetComponent<Animator>();
            _oldController = _animator.runtimeAnimatorController;
            _animEvent = obj.GetComponent<CharacterAnimEvent>();
            _animationEffect = obj.GetComponent<AnimationEffect>();
        }
    
        [ContextMenu("CustomAnimation")]
        public void PlayGuitarAnim()
        {
            _animator.runtimeAnimatorController = _controller;
            _animator.Play(AnimName);
            var offset = new Vector3(-0.002f, 0.009f, 0.0011f) * 130;
            _isPlaying = true;
            _guitarInstance = GameObject.Instantiate(_guitar);
            _guitarInstance.AddComponent<ZNetView>();
            _guitarInstance.AddComponent<ZSyncTransform>();
            _audioSource = _guitarInstance.AddComponent<AudioSource>();
            _guitarInstance.transform.parent = _playerRoot.transform;
            _guitarInstance.transform.localPosition = new Vector3(-0.437f, 1.294f, 0.092f); 
            _guitarInstance.transform.localRotation = Quaternion.Euler(21.425f, 82.124f, 91.123f);
        }


        [ContextMenu("StopCustomAnimation")]
        public void StopGuitarAnim()
        {
            _animator.runtimeAnimatorController = _oldController;
            _audioSource = null;
            _isPlaying = false;
            Destroy(_guitarInstance.gameObject);
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.H))
            {
                PlayGuitarAnim();
                return;
            }

            if (Input.GetKeyDown(KeyCode.J))
            {
                StopGuitarAnim();
                return;          
            }
            

            if(_isPlaying == false)
                return;
        
            if(_audioSource == null)
                return;
        
            if(Input.GetKeyDown(KeyCode.A))
                _audioSource.PlayOneShot(_clips[0]);
        
            if(Input.GetKeyDown(KeyCode.D))
                _audioSource.PlayOneShot(_clips[1]);
        
            if(Input.GetKeyDown(KeyCode.E))
                _audioSource.PlayOneShot(_clips[2]);
        
            if(Input.GetKeyDown(KeyCode.C))
                _audioSource.PlayOneShot(_clips[3]);
        }
    }
}