using UnityEngine;

public class PickUpWeapons : MonoBehaviour
{
    [SerializeField] private GameObject GunToSpawn;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GunToSpawn.SetActive(true);
            Destroy(gameObject);
        }
    }
}
