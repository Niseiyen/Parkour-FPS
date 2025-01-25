using Photon.Pun;
using System.Collections;
using UnityEngine;
using TMPro;
using Photon.Pun.UtilityScripts;

public class Weapon : MonoBehaviour
{
    [SerializeField] Camera camera;

    [SerializeField] private int damage;
    [SerializeField] private int pelletsCount = 1;
    [SerializeField] private float spreadMultiplier = 0f;
    [SerializeField] private float fireRate;

    private float nextFire;

    [Header("VFX")]
    [SerializeField] private GameObject hitVFX;
    [SerializeField] private GameObject muzzleFlash;
    [SerializeField] private Transform muzzlePoint;

    [Header("Trail")]
    [SerializeField] private GameObject trailPrefab;
    [SerializeField] private float trailSpeed = 50f;


    [Header("Ammo")]
    [SerializeField] private int ammo;
    [SerializeField] private int maxAmmo;

    [Header("UI")]
    [SerializeField] private TextMeshProUGUI ammoText;

    [Header("Recoil Setting")]
    [Range(0, 2)]
    [SerializeField] private float recoverPercent = 0.7f;
    [SerializeField] private float recoilUp = 1f;
    [SerializeField] private float recoilBack = 0f;

    [HideInInspector] public Vector3 originalPosition;
    private Vector3 recoilVelocity = Vector3.zero;
    private float recoilLength;
    private float recoverLength;

    public bool recoiling;
    public bool recovering;

    private Animator animator;
    private bool isReloading = false;
    public bool isBusy { get; private set; } = false;

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

        if (Input.GetKeyDown(KeyCode.R) && !isReloading && ammo < maxAmmo)
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

        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        float reloadDuration = stateInfo.length - 0.2f;

        yield return new WaitForSeconds(reloadDuration);
        ammo = maxAmmo;

        UpdateWeaponUI();
        isReloading = false;
        isBusy = false;
    }

    public void UpdateWeaponUI()
    {
        ammoText.text = ammo.ToString() + " / " + maxAmmo.ToString();
    }

    private void Shoot()
    {
        recoiling = true;
        recovering = false;
        Debug.Log($"{gameObject.name} is recoiling.");

        GameObject muzzleFlashInstance = PhotonNetwork.Instantiate(this.muzzleFlash.name, muzzlePoint.position, Quaternion.identity);
        StartCoroutine(MoveMuzzleFlash(muzzleFlashInstance));

        for (int i = 0; i < pelletsCount; i++)
        {
            Vector3 splayOffset = Random.insideUnitCircle * spreadMultiplier;
            splayOffset.z = 0;

            Vector3 direction = camera.transform.forward + new Vector3(splayOffset.x, splayOffset.y, 0);
            Ray ray = new Ray(camera.transform.position, direction);

            if (Physics.Raycast(ray.origin, ray.direction, out RaycastHit hit, 100f))
            {
                PhotonNetwork.Instantiate(hitVFX.name, hit.point, Quaternion.identity);

                GameObject trail = PhotonNetwork.Instantiate(trailPrefab.name, muzzlePoint.position, Quaternion.identity);
                StartCoroutine(AnimateTrail(trail, hit.point));

                PhotonView targetPhotonView = hit.transform.GetComponent<PhotonView>();
                if (targetPhotonView != null && targetPhotonView.IsMine)
                {
                    Debug.Log("Hit myself. Ignoring...");
                    continue;
                }

                Health targetHealth = hit.transform.GetComponent<Health>();
                if (targetHealth != null)
                {
                    PhotonNetwork.LocalPlayer.AddScore(damage);

                    if (damage >= targetHealth.health)
                    {
                        RoomManager.instance.kills++;
                        RoomManager.instance.SetHashes();
                    }

                    targetPhotonView.RPC("TakeDamage", RpcTarget.All, damage, transform.position);
                }
            }
        }
    }

    private IEnumerator AnimateTrail(GameObject trail, Vector3 targetPosition)
    {
        float time = 0f;
        Vector3 startPosition = trail.transform.position;

        while (time < 1f)
        {
            time += Time.deltaTime * trailSpeed / Vector3.Distance(startPosition, targetPosition);
            trail.transform.position = Vector3.Lerp(startPosition, targetPosition, time);
            yield return null;
        }

        trail.transform.position = targetPosition;

        PhotonNetwork.Destroy(trail);
    }



    private IEnumerator MoveMuzzleFlash(GameObject muzzleFlashInstance)
    {
        float time = 0f;
        Vector3 initialPosition = muzzleFlashInstance.transform.position;
        Vector3 randomOffset = new Vector3(Random.Range(-0.1f, 0.1f), Random.Range(-0.1f, 0.1f), 0f);

        while (time < 0.2f)
        {
            muzzleFlashInstance.transform.position = Vector3.Lerp(initialPosition, initialPosition + randomOffset, time / 0.2f);
            time += Time.deltaTime;
            yield return null;
        }

        muzzleFlashInstance.transform.position = initialPosition;

        PhotonNetwork.Destroy(muzzleFlashInstance);
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
