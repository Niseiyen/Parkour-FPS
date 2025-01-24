using Photon.Pun;
using System.Collections;
using UnityEngine;
using TMPro;
using Photon.Pun.UtilityScripts;

public class Weapon : MonoBehaviour
{
    [SerializeField] Camera camera;

    [SerializeField] private int damage;
    [SerializeField] private float fireRate;

    private float nextFire;

    [Header("VFX")]
    [SerializeField] private GameObject hitVFX;
    [SerializeField] private GameObject muzzleFlash;
    [SerializeField] private Transform muzzlePoint;

    [Header("Ammo")]
    [SerializeField] private int ammo = 30;
    [SerializeField] private int maxAmmo = 30;

    [Header("UI")]
    [SerializeField] private TextMeshProUGUI ammoText;

    private Animator animator;
    private bool isReloading = false;
    public bool isBusy { get; private set; } = false;

    [Header("Recoil Setting")]
    [Range(0, 2)]
    [SerializeField] private float recoverPercent = 0.7f;
    [SerializeField] private float recoilUp = 1f;
    [SerializeField] private float recoilBack = 0f;

    [HideInInspector] public Vector3 originalPosition;
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

        if (Input.GetKeyDown(KeyCode.R) && !isReloading && ammo < 30)
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
        isBusy = true;
        animator.SetTrigger("Reload");

        yield return new WaitForSeconds(0.5f);

        ammo = 30;

        UpdateWeaponUI();
        isReloading = false;
        isBusy = false; 
    }

    private void UpdateWeaponUI()
    {
        ammoText.text = ammo.ToString() + " / " + maxAmmo.ToString();
    }

    private void Shoot()
    {
        recoiling = true;
        recovering = false;

        GameObject muzzleFlash = PhotonNetwork.Instantiate(this.muzzleFlash.name, muzzlePoint.position, Quaternion.identity);

        Ray ray = new Ray(camera.transform.position, camera.transform.forward);

        RaycastHit hit;
        if (Physics.Raycast(ray.origin, ray.direction, out hit, 100f))
        {
            PhotonNetwork.Instantiate(hitVFX.name, hit.point, Quaternion.identity);

            if (hit.transform.gameObject.GetComponent<Health>())
            {
                PhotonNetwork.LocalPlayer.AddScore(damage);

                if (damage >= hit.transform.gameObject.GetComponent<Health>().health)
                {
                    RoomManager.instance.kills++;
                    RoomManager.instance.SetHashes();
                }

                hit.transform.gameObject.GetComponent<PhotonView>().RPC("TakeDamage", RpcTarget.All, damage);
            }
        }
    }

    private void Recoil()
    {
        Vector3 finalPosition = new Vector3(originalPosition.x, originalPosition.y + recoilUp, originalPosition.z - recoilBack);

        transform.localPosition = Vector3.SmoothDamp(transform.localPosition, finalPosition, ref recoilVelocity, recoilLength);

        if (transform.localPosition == finalPosition)
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

    public void InitializeWeapon()
    {
        originalPosition = transform.localPosition;
        transform.localPosition = originalPosition;
        recoilLength = 0;
        recoverLength = 1 / fireRate * recoverPercent;

        recoiling = false;
        recovering = false;
    }
}
