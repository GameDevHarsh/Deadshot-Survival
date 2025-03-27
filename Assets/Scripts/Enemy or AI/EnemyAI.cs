using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{

    private ThirdPersonShooterController Target;
    [SerializeField] private float AtRange;
    [SerializeField] private float Speed;
    [SerializeField] private float RotationSpeed;
    [SerializeField] private float RunSpeed;
    public float Health;
    [SerializeField] private AudioClip attackingClip;
     private AudioSource audioSource;
    private NavMeshAgent meshAgent;
    private Animator anim;
    private bool IsGetHit;
    private bool IsDead = false;
    private bool IsPlInRange;
    // Update is called once per frame

    private void OnEnable()
    {
        anim = GetComponent<Animator>();
        meshAgent = GetComponent<NavMeshAgent>();
        IsPlInRange = false;
        Target = FindAnyObjectByType<ThirdPersonShooterController>();
        audioSource = Target.audioSource;
    }

    void Update()
    {
        if(!IsDead)
        {
            var _distance = Vector3.Distance(transform.position, Target.transform.position);
            Vector3 direction = (Target.transform.position - transform.position).normalized;
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            targetRotation.x = 0;
            targetRotation.z = 0;
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, RotationSpeed * Time.deltaTime);
            if (!IsGetHit)
            {
                if (_distance < AtRange)
                {
                    anim.SetBool("IsWalking", false);
                    anim.SetTrigger("Attack");
                    anim.SetBool("IsAttacking", true);
                    IsPlInRange = true;
                }
                else
                {
                    IsPlInRange = false;
                    meshAgent.destination = Target.transform.position;
                    anim.SetBool("IsWalking", true);
                    anim.SetBool("IsAttacking", false);
                }
            }
            if (Health <= 0)
            {
                StartCoroutine("Dead");
            }
        }
        
    }
    public void Damage()
    {
        if (Health > 0)
        {
            IsGetHit = true;
            anim.SetBool("IsWalking", false);
            anim.SetBool("IsGettingHit", true);
            var _damage = Random.Range(1, 4);
            meshAgent.updatePosition = false;
            meshAgent.updateRotation = false;
            Health -= _damage;
           StartCoroutine("ContinueOtherTask");
        }
    }
    private IEnumerator ContinueOtherTask()
    {
        yield return new WaitForSeconds(2);
        anim.SetBool("IsGettingHit", false);
        meshAgent.updatePosition = true;
        meshAgent.updateRotation = true;
        IsGetHit = false;
    }
    private IEnumerator Dead()
    {
        IsDead = true;
        anim.SetTrigger("Death");
        meshAgent.enabled = false;
        yield return new WaitForSeconds(2f);
        Destroy(this.gameObject);
    }
    private void DamagePlayer()
    {
        if(IsPlInRange)
        {
            audioSource.PlayOneShot(attackingClip);
            Target.GetComponent<ThirdPersonHealth>().DamagePl();
        }
        
    }
}

