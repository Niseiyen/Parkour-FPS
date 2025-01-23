using Photon.Pun;
using System.Collections;
using UnityEngine;
using TMPro;

public class Weapon : MonoBehaviour
{
    [SerializeField] Camera camera;

    [SerializeField] private int damage;
    [SerializeField] private float fireRate;

    private float nextFire;

    [Header("VFX")]
    [SerializeField] private GameObject hitVFX;

    [Header("Ammo")]
    [SerializeField] private int mag = 5;
    [SerializeField] private int ammo = 30;
    [SerializeField] private int magAmmo = 30;

    [Header("UI")]
    [SerializeField] private TextMeshProUGUI magText;
    [SerializeField] private TextMeshProUGUI ammoText;

    private Animator animator;
    private bool isReloading = false;

    private void Start()
    {
        UpdateWeaponUI();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (nextFire > 0)
        {
            nextFire -= Time.deltaTime;
        }

        if (Input.GetButton("Fire1") && nextFire <= 0 && ammo > 0 && !isReloading)
        {
            nextFire = 1 / fireRate;

            ammo--;

            UpdateWeaponUI();

            Shoot();
        }

        if (Input.GetKeyDown(KeyCode.R) && !isReloading && mag > 0)
        {
            StartCoroutine(Reload());
        }
    }

    private IEnumerator Reload()
    {
        isReloading = true;
        animator.SetTrigger("Reload");

        yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);

        if (mag > 0)
        {
            mag--;
            ammo = magAmmo;
        }

        UpdateWeaponUI();
        isReloading = false;
    }

    private void UpdateWeaponUI()
    {
        magText.text = mag.ToString();
        ammoText.text = ammo + " / " + magAmmo;
    }

    private void Shoot()
    {
        Ray ray = new Ray(camera.transform.position, camera.transform.forward);

        RaycastHit hit;
        if (Physics.Raycast(ray.origin, ray.direction, out hit, 100f))
        {
            PhotonNetwork.Instantiate(hitVFX.name, hit.point, Quaternion.identity);

            if (hit.transform.gameObject.GetComponent<Health>())
            {
                hit.transform.gameObject.GetComponent<PhotonView>().RPC("TakeDamage", RpcTarget.All, damage);
            }
        }
    }
}
