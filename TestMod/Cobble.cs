using UnityEngine;

namespace TestMod
{
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
}