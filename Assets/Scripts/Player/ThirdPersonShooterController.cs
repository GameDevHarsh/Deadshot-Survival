using Cinemachine;
using Core.Inputs;
using Core.Player;
using Core.Projectiles;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEngine.UI;

public class ThirdPersonShooterController : MonoBehaviour
{
    [Header("Mouse Sensitivity Settings")]
    [Space(2)]
    [SerializeField] private float normalSensivity;
    [SerializeField] private float aimSensivity;
    [SerializeField] GameObject aimTargetPrefab;

    [Header("Components Inputs")]
    [SerializeField] private CinemachineVirtualCamera aimCam;
    [SerializeField] private CinemachineVirtualCamera followCam;

    [SerializeField] private Rig aimRig;
    [SerializeField] private Image crosshair;

    [Tooltip("Actual Bullet Gameobject.")]
    [SerializeField] private Transform weapon;
    [SerializeField] private Transform spawnBulletPosition;
    [SerializeField] ParticleSystem muzzleFlash;
    [SerializeField] private TextMeshProUGUI currentAmmoText;
    [SerializeField] private TextMeshProUGUI totalAmmoText;
    [SerializeField] private int totalAmmoCount;
    [SerializeField] private int magAmmoCount;
    [SerializeField] private GameObject emptyMag;


    [Header("Audio clips")]
    [SerializeField] private AudioClip dryFireClip;
    [SerializeField] private AudioClip fireClip;
    [SerializeField] private GameObject bulletPrefab;
    public AudioSource audioSource;
    //private BulletPool bulletPool;
    GameObject bulletPoolTransform;


    [Header("Bullet Layer Mask")]
    [Tooltip("Set a layer on which the bullets will show their effect.")]
    [SerializeField] private LayerMask aimColliderLayerMask = new LayerMask();
    private Queue<GameObject> bulletPool = new Queue<GameObject>();
    [SerializeField] private int bulletPoolSize = 20;

    [SerializeField] private float recoilForce = 0.1f;
    [SerializeField] private float recoilDuration = 0.1f;
    private CinemachineBasicMultiChannelPerlin aimCinemachineNoise;
    private CinemachineBasicMultiChannelPerlin followCinemachineNoise;

    private Coroutine recoilCoroutine;

    [SerializeField] UiManager uiManager;
    private Animator anim;
    private ThirdPersonController thirdPersonController;
    private InputManager inputManager;
    private PlayerInputs playerInputs;
    private Vector3 mouseWorldPosition;
    private RaycastHit hit;
    private float aimRigWeight;
    private bool aim;
    private bool shoot;
    private float shootcooldownTime;
    private float AimcooldownTime;
    private int currentammocount;
    private bool isReloading = false;
    private GameObject bullet;
    #region Initialization
    private void Awake()
    {
        //Get the references
        anim = GetComponent<Animator>();
        thirdPersonController = GetComponent<ThirdPersonController>();
        inputManager = FindFirstObjectByType<InputManager>();
        playerInputs = GetComponent<PlayerInputs>();
        //  bulletPool = weapon.GetComponent<BulletPool>();
        InitializeBulletPool();
        // Get the noise component for recoil effect
        if (aimCam != null)
        {
            aimCinemachineNoise = aimCam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        }
        if (followCam != null)
        {
            followCinemachineNoise = followCam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        }
    }
    private void InitializeBulletPool()
    {
        bulletPoolTransform = new GameObject("Bullet Pool");
        for (int i = 0; i < bulletPoolSize; i++)
        {
            GameObject bullet = Instantiate(bulletPrefab);
            bullet.transform.parent = bulletPoolTransform.transform;
            bullet.SetActive(false);
            bulletPool.Enqueue(bullet);
        }
    }

    private GameObject GetBulletFromPool()
    {
        GameObject bullet;
        if (bulletPool.Count > 0)
        {
            bullet = bulletPool.Dequeue();
        }
        else
        {
            bullet = Instantiate(bulletPrefab);
        }
        bullet.transform.position = spawnBulletPosition.position;
        bullet.transform.rotation = spawnBulletPosition.rotation;
        bullet.SetActive(true);
        return bullet;
    }

    private void ReturnBulletToPool(Bullet bullet)
    {
        bullet.gameObject.SetActive(false);
        bullet.transform.position = spawnBulletPosition.position;
        bullet.GetComponent<Rigidbody>().linearVelocity = Vector3.zero;
        bulletPool.Enqueue(bullet.gameObject);
    }
    private void OnEnable()
    {
        //disables the Aiming camera on enable
        Bullet.OnBulletExpired += ReturnBulletToPool;
        shootcooldownTime = 0;
        AimcooldownTime = 0;
        aimCam.gameObject.SetActive(false);
        //assigning new gameobject to bullet pool.
        Cursor.lockState = CursorLockMode.Locked;
        //intialize current ammo count according to the mag ammo count of the weapon.
        currentammocount = magAmmoCount;
        totalAmmoCount = 450;
        aimRigWeight = 0;
        aim = false;
    }

    private void Start()
    {
        inputManager.PlayerInputBroadcaster.CallBacks.OnPlayerStartAim += (aimValue) =>
        {
            aim = aimValue;
            //seting bool true of the animator.
            //calling the aim function which will make the player aim.
            anim.SetBool("Aiming", aimValue);
            playerInputs.aim = aimValue;
            ////checking if we cancel the aim.
            //if (!aimValue)
            //{
            //    //stop the aiming position.
            //    StopAim();
            //}
        };

        inputManager.PlayerInputBroadcaster.CallBacks.OnPlayerShootFired += (shootValue) =>
        {
            shoot = shootValue;
        };
        inputManager.PlayerInputBroadcaster.CallBacks.OnPlayerReloadFired += (reloadValue) =>
        {
            //starts the reload coroutine.
            StartCoroutine("Reload");
        };
    }
    #endregion

    #region Updates & Rigidbody
    private void Update()
    {
        if(!uiManager.isUIActive)
        {
            HandleMouseWorldPosition();
            //set the weight of aim rig.
            aimRig.weight = Mathf.Lerp(aimRig.weight, aimRigWeight, Time.deltaTime * 10f);
            //assign mouse world position to the world aim target variable.
            Vector3 worldAimTarget = mouseWorldPosition;
            //we want to check only Left & Right, and not Up & Down.
            worldAimTarget.y = transform.position.y;
            Vector3 aimDirection = (worldAimTarget - transform.position).normalized;
            transform.forward = Vector3.Lerp(transform.forward, aimDirection, Time.deltaTime * 20f);
            //updating the text of current ammo text
            currentAmmoText.text = currentammocount.ToString("D2");
            //updating the text of current ammo text
            totalAmmoText.text = totalAmmoCount.ToString("D3"); 
            //checking if shoot boolean is true as well as the player has ammo and not reloading then shoots.
            if (shoot && currentammocount > 0 && !isReloading)
            {
                Shoot();
            }
            else if (shoot && currentammocount == 0 && !isReloading)
            {
                shootcooldownTime += Time.deltaTime;
                if (shootcooldownTime > 0.3f)
                {
                    audioSource.PlayOneShot(dryFireClip, 0.8f);
                    shootcooldownTime = 0;
                }
            }
            if (aim)
            {
                Aim(mouseWorldPosition);
            }
            else
            {
                //stop the aiming position.
                StopAim();
            }


            //check if the current bullet count is zero then reloads the gun.
            if (currentammocount == 0)
            {
                StartCoroutine("Reload");
            }
            if (aim && shoot)
            {
                aimRig.weight = 1f;
            }
            else if (aim && !shoot)
            {
                aimRig.weight = 1f;
            }
            else if (!aim && shoot && !isReloading)
            {
                aimRig.weight = 1f;
            }
            else
            {
                DisableAimRig();
            }
        }
    }
    #endregion
    private void DisableAimRig()
    {
        AimcooldownTime += Time.deltaTime;
        if (AimcooldownTime > 10f)
        {
            aimRig.weight = 0;
            AimcooldownTime = 0;
        }
    }
    private IEnumerator Reload()
    {
        if (totalAmmoCount == 0 || currentammocount == magAmmoCount)
            yield break;

        emptyMag.SetActive(true);
        if (!isReloading)
        {
            isReloading = true;
            anim.SetTrigger("Reload");
        }
        yield return new WaitForSeconds(2f);
        if (isReloading)
        {
            if (currentammocount + totalAmmoCount > magAmmoCount)
            {
                totalAmmoCount = totalAmmoCount - (magAmmoCount - currentammocount);
                currentammocount = 30;
            }
            else
            {
                currentammocount = currentammocount + totalAmmoCount;
                totalAmmoCount = 0;
            }
            isReloading = false;
        }
        yield return null;

    }
    private void Shoot()
    {
        shootcooldownTime += Time.deltaTime;
        if (shootcooldownTime > 0.1f)
        {
            audioSource.PlayOneShot(fireClip, 0.8f);
            shootcooldownTime = 0;
            currentammocount--;
            Vector3 aimDir = (mouseWorldPosition - spawnBulletPosition.position).normalized;
            muzzleFlash.Emit(10);
            //  bulletPool.SetBulletDirection(aimDir, hit, bulletPoolTransform);
            Vector3 spawnPos = spawnBulletPosition.position;
            bullet = GetBulletFromPool();
            bullet.transform.position = spawnPos;
            bullet.transform.rotation = Quaternion.LookRotation(aimDir, Vector3.up);
            bullet.GetComponent<Bullet>().Dir = aimDir;
           // StartCoroutine(ReturnBulletAfterTime(bullet, 3f));
            // Apply recoil effect
            if (recoilCoroutine != null)
            {
                StopCoroutine(recoilCoroutine);
            }
            recoilCoroutine = StartCoroutine(ApplyRecoil());
            // Instantiate(bulletPrefab, spawnBulletPosition.position, Quaternion.LookRotation(aimDir, Vector3.up), bulletPoolTransform.transform);
            // bulletPool.BulletObjectPool.Get();
        }
    }
    private IEnumerator ApplyRecoil()
    {
        if (aimCinemachineNoise != null)
        {
            aimCinemachineNoise.m_AmplitudeGain = 1f; // Set shake intensity
            aimCinemachineNoise.m_FrequencyGain = 2f; // Set shake speed
        }
        if (followCinemachineNoise != null)
        {
            followCinemachineNoise.m_AmplitudeGain = 1f; // Set shake intensity
            followCinemachineNoise.m_FrequencyGain = 2f; // Set shake speed
        }

        // Push player slightly backward
        Vector3 recoilDirection = -transform.forward * recoilForce;
       // thirdPersonController.CharacterController.Move(recoilDirection);

        yield return new WaitForSeconds(recoilDuration);

        if (aimCinemachineNoise != null)
        {
            aimCinemachineNoise.m_AmplitudeGain = 0f; // Reset shake
            aimCinemachineNoise.m_FrequencyGain = 0f;
        }
        if (followCinemachineNoise != null)
        {
            followCinemachineNoise.m_AmplitudeGain = 0f; // Reset shake
            followCinemachineNoise.m_FrequencyGain = 0f;
        }
    }

    //private IEnumerator ReturnBulletAfterTime(GameObject bullet, float delay)
    //{
    //    yield return new WaitForSeconds(delay);
    //    ReturnBulletToPool(bullet.GetComponent<Bullet>());
    //}
    private void HandleMouseWorldPosition()
    {
        mouseWorldPosition = Vector3.zero;  //Actual World Position of Mouse - what it hit.

        //screenCenterPoint - Center point of the screen.
        Vector2 screenCenterPoint = new Vector2(Screen.width / 2f, Screen.height / 2f);
        Ray ray = Camera.main.ScreenPointToRay(screenCenterPoint);

        if (Physics.Raycast(ray, out hit, 999f, aimColliderLayerMask))
        {
            mouseWorldPosition = hit.point;
            aimTargetPrefab.transform.position = hit.point;
            if(bullet!=null)
            {
                bullet.GetComponent<Bullet>().Dir = hit.point - spawnBulletPosition.position;
            }
            if (hit.transform.CompareTag("Enemy")|| hit.transform.CompareTag("Head"))
            {
                crosshair.GetComponent<Image>().color = Color.red;
            }
            else
            {
                crosshair.GetComponent<Image>().color = Color.white;
            }
            Debug.DrawRay(ray.origin, transform.forward * 10, Color.red);
        }
    }
    private void Aim(Vector3 mouseWorldPosition)
    {
        aimCam.gameObject.SetActive(true);
        thirdPersonController.SetSensitivity(aimSensivity);
        playerInputs.ignoreInputs = true;
    }

    private void StopAim()
    {
        aimCam.gameObject.SetActive(false);
        thirdPersonController.SetSensitivity(normalSensivity);
        playerInputs.ignoreInputs = false;
    }
    private IEnumerator ThrowEmptyMag()
    {
        GameObject temp = Instantiate(emptyMag, emptyMag.transform.position, Quaternion.identity);
        temp.transform.localScale = new Vector3(0.6f, 0.6f, 0.6f);
        temp.GetComponent<Rigidbody>().isKinematic = false;
        emptyMag.SetActive(false);
        yield return new WaitForSeconds(2f);
        Destroy(temp);
        yield return null;
    }
    private void OnDisable()
    {
        Bullet.OnBulletExpired -= ReturnBulletToPool; // Unsubscribe when disabled
        Destroy(bulletPoolTransform);
    }
}
