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
        private Vector3 startPosition;
        // private IObjectPool<Bullet> bulletPool;

        //public void SetPool(IObjectPool<Bullet> pool)
        //{
        //    bulletPool = pool;
        //}

        private void OnEnable()
        {
            if(rigidbody==null)
            {

            rigidbody = GetComponent<Rigidbody>();
            }
            rigidbody.linearVelocity = Vector3.zero;
            rigidbody.angularVelocity = Vector3.zero;

        }
        public void SetStartPosition(Vector3 position)
        {
            startPosition = position;
        }
        #region Triggers
        private void OnTriggerEnter(Collider other)
        {
           // GameObject effect = null;

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

        private void FixedUpdate() 
        {
            rigidbody.linearVelocity = Dir * bulletSpeed;
            float maxDistance = 50f;

            if ((transform.position - startPosition).sqrMagnitude > maxDistance * maxDistance)
            {
                OnBulletExpired?.Invoke(this);
            }
        }

    }
}
