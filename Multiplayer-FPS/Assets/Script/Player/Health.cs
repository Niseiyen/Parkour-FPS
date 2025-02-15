using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;

public class Health : MonoBehaviour
{
    [SerializeField] DamageIndicator damageIndicator;

    public int health = 100;
    public bool isLocalPlayer;

    private bool hasDied = false;

    [Header("UI")]
    [SerializeField] private TextMeshProUGUI healthText;


    [PunRPC]
    public void TakeDamage(int _damage, Vector3 damagePosition)
    {
        if(hasDied) return;

        health -= _damage;

        damageIndicator.damageLocation = damagePosition;
        GameObject indicator = Instantiate(damageIndicator.gameObject, damageIndicator.transform.position, damageIndicator.transform.rotation, 
            damageIndicator.transform.parent);

        indicator.SetActive(true);

        healthText.text = health.ToString();
        if (health <= 0)
        {
            hasDied = true;

            if(isLocalPlayer)
            {
                RoomManager.instance.SpawnPlayer();

                RoomManager.instance.deaths++;
                RoomManager.instance.SetHashes();
            }

            Destroy(gameObject);
        }
    }
}
