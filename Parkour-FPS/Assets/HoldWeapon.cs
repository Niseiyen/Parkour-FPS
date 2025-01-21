using UnityEngine;

public class HoldWeapon : MonoBehaviour
{
    private Transform playerTransform;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Get the player's transform component
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
    }

    // Update is called once per frame
    void Update()
    {
        // Rotate the weapon to face the same direction as the player
        transform.rotation = Quaternion.LookRotation(playerTransform.forward);
    }
}
