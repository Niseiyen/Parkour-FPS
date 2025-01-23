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

    [Header("Recoil Setting")]
    [Range(0, 2)]
    [SerializeField] private float recoverPercent = 0.7f;
    [SerializeField] private float recoilUp = 1f;
    [SerializeField] private float recoilBack = 0f;

    private Vector3 originalPosition;
    private Vector3 recoilVelocity = Vector3.zero;
    private float recoilLength;
    private float recoverLength;

    private bool recoiling;
    private bool recovering;

    private void Start()
    {
        UpdateWeaponUI();
        animator = GetComponent<Animator>();

        originalPosition = transform.localPosition;

        recoilLength = 0;
        recoverLength = 1 / fireRate * recoverPercent;
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

        if (recoiling)
        {
            Recoil();
        }

        if (recovering)
        {
            Recovering();
        }
    }

    private IEnumerator Reload()
    {
        isReloading = true;
        animator.SetTrigger("Reload");

        yield return new WaitForSeconds(0.5f);

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
        recoiling = true;
        recovering = false;

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

    private void Recoil()
    {
        Vector3 finalPosition = new Vector3(originalPosition.x, originalPosition.y + recoilUp, originalPosition.z - recoilBack);

        transform.localPosition = Vector3.SmoothDamp(transform.localPosition, finalPosition, ref recoilVelocity, recoilLength);

        if(transform.localPosition == finalPosition)
        {
            recoiling = false;
            recovering = true;
        }
    }

    private void Recovering()
    {
        Vector3 finalPosition = originalPosition;

        transform.localPosition = Vector3.SmoothDamp(transform.localPosition, finalPosition, ref recoilVelocity, recoverLength);

        if (transform.localPosition == finalPosition)
        {
            recoiling = false;
            recovering = false;
        }
    }
}
