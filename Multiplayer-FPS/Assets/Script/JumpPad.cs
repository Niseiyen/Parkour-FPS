using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpPad : MonoBehaviour
{
    [Header("Bounce Settings")]
    [SerializeField] private float bounceForce; 

    [Header("FX")]
    [SerializeField] private GameObject bounceFX;
    [SerializeField] private Transform bounceFXLocation;
    [SerializeField] private GameObject continueFX;

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.CompareTag("Player"))
        {
            continueFX.SetActive(false);
            Instantiate(bounceFX, bounceFXLocation.position, Quaternion.identity);
            collision.gameObject.GetComponent<Rigidbody>().AddForce(Vector3.up * bounceForce, ForceMode.Impulse);
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        continueFX.SetActive(true);
    }
}
