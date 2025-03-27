using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.VFX;

namespace Core.Projectiles
{
    public class BulletPool : MonoBehaviour
    {
        //[SerializeField] private float bulletSpeed = 100f;
        //[SerializeField] Bullet bulletPrefab;
        //[SerializeField] Transform spawnPoint;
        //GameObject bulletPoolParent;

        //[SerializeField] ParticleSystem muzzleFlash;
        [SerializeField] VisualEffect bloodHitEffect;

        [SerializeField] List<HitEffectType> hitEffectType;
        Dictionary<string, ParticleSystem> hitDict = new Dictionary<string, ParticleSystem>();

        //private IObjectPool<Bullet> bulletPool;
        //private Vector3 bulletDir;
        private RaycastHit bulletHitPoint;

        #region Initialization
        private void Awake()
        {
            //bulletPool = new ObjectPool<Bullet>(
            //    CreateBullet,
            //    OnGet,
            //    OnRelease,
            //    OnDestroyed,
            //    maxSize: 4
            //);

            foreach(HitEffectType hitType in hitEffectType)
            {
                hitDict.Add(hitType.effectName, hitType.effect);
            }
        }
        #endregion

        //public void SetBulletDirection(Vector3 aimDir, RaycastHit hitPoint, GameObject poolTransform)
        //{
        //    bulletDir = aimDir;
        //    bulletHitPoint = hitPoint;
        //    bulletPoolParent = poolTransform;
        //}

        #region ObjectPool Default Functions
        //private Bullet CreateBullet()
        //{
        //    Bullet bullet = Instantiate(bulletPrefab, spawnPoint.position, Quaternion.LookRotation(bulletDir, Vector3.up), bulletPoolParent.transform);
        //    bullet.SetPool(bulletPool);
        //    return bullet;
        //}

        //private void OnGet(Bullet bullet)
        //{
        //    if (bulletHitPoint.collider == null)
        //        return;
        //    bullet.gameObject.SetActive(true);
        //    StartCoroutine(MoveBullet(bullet));
        //    bullet.transform.position = spawnPoint.position;
        //    muzzleFlash.Emit(1);
        //    PlayHitEffect(bulletHitPoint.transform.tag);
            
        //}
        //private IEnumerator MoveBullet(Bullet bullet)
        //{
        //    bullet.GetComponent<Rigidbody>().velocity = bulletDir * bulletSpeed;
        //    yield return new WaitForSeconds(1f);
        //}
        //private void OnRelease(Bullet bullet)
        //{
        //    bullet.gameObject.SetActive(false);
        //    bullet.transform.position = spawnPoint.position;
        //}

        //private void OnDestroyed(Bullet bullet)
        //{
        //    Destroy(bullet.gameObject);
        //}
        #endregion

        //private void PlayHitEffect(string name)
        //{
        //    if(!hitDict.ContainsKey(name)) return;
        //    hitDict[name].transform.position = bulletHitPoint.point;
        //    hitDict[name].transform.forward = bulletHitPoint.normal;
        //    if(!PrefabUtility.IsPartOfAnyPrefab(hitDict[name]))
        //    {
        //        hitDict[name].transform.parent = bulletHitPoint.transform;
        //    }
        //    hitDict[name].Emit(1);
        //}

        #region Getter
        //public IObjectPool<Bullet> BulletObjectPool
        //{
        //    get
        //    {
        //        return bulletPool;
        //    }
        //}
        #endregion
    }

    [Serializable]
    public class HitEffectType
    {
        public string effectName;
        public ParticleSystem effect;
    }
}
