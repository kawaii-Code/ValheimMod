using System.Collections;
using UnityEngine;

namespace TestMod
{
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
}