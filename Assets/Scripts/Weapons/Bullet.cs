using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

namespace Core.Projectiles
{
    public class Bullet : MonoBehaviour
    {
        private Rigidbody rigidbody;
        public static event Action<Bullet> OnBulletExpired;
        [HideInInspector]  public Vector3 Dir;
        public float bulletSpeed;

        // private IObjectPool<Bullet> bulletPool;

        //public void SetPool(IObjectPool<Bullet> pool)
        //{
        //    bulletPool = pool;
        //}


        private void Start()
        {
            rigidbody = GetComponent<Rigidbody>();
        }

        #region Triggers
        private void OnTriggerEnter(Collider other)
        {
            GameObject effect = null;

            if (other.CompareTag("Enemy") || other.CompareTag("Head"))
            {
                if (other.CompareTag("Head"))
                {
                    EnemyAI enemy = other.GetComponentInParent<EnemyAI>();
                    if (enemy != null)
                    {
                        enemy.Health = 0;
                    }
                }
                else
                {
                    other.GetComponent<EnemyAI>().Damage();
                }
            }
            OnBulletExpired?.Invoke(this);
        }


        #endregion

        private void Update() 
        {
            rigidbody.linearVelocity = Dir * bulletSpeed;
            if (this.transform.position.z >= 100f || this.transform.position.z <= -100f)
            {
                OnBulletExpired?.Invoke(this);
                //bulletPool.Release(this);
                //Destroy(this.gameObject);
            }    
        }

    }
}
