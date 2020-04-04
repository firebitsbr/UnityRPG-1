using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    float x;
    float z;
    public float moveSpeed = 3;
    public Collider weaponCollider;
    public PlayerUIManager playerUIManager;
    public GameObject gameOverText;
    public Transform target;
    public int maxHp = 100;
    int hp = 100;
    public int maxStamina = 100;
    int stamina;
    bool isDie;

    Rigidbody rb;
    Animator animator;
    void Start()
    {
        hp = maxHp;
        stamina = maxStamina;
        playerUIManager.Init(this);
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        HideColliderWeapon();
    }

    void Update()
    {
        if (isDie)
        {
            return;
        }
        x = Input.GetAxisRaw("Horizontal");
        z = Input.GetAxisRaw("Vertical");

        if (Input.GetKeyDown(KeyCode.Space))
        {
            Attack();
        }

        IncreaseStamina();
    }

    void IncreaseStamina()
    {
        stamina++;
        if (stamina >= maxStamina)
        {
            stamina = maxStamina;
        }
        playerUIManager.UpdataStamina(stamina);
    }

    void Attack()
    {
        if (stamina >= 20)
        {
            stamina -= 40;
            playerUIManager.UpdataStamina(stamina);
            LookAtTarget();
            animator.SetTrigger("Attack");
        }
        
    }

    void LookAtTarget()
    {
        float distance = Vector3.Distance(transform.position, target.position);
        if (distance <= 2f)
        { 
            transform.LookAt(target);
        }
    }

    public void HideColliderWeapon()
    {
        weaponCollider.enabled = false;
    }

    public void ShowColliderWeapon()
    {
        weaponCollider.enabled = true;
    }

    void Damage(int damage)
    {
        hp -= damage;
        if (hp <= 0)
        {
            hp = 0;
            isDie = true;
            animator.SetTrigger("Die");
            gameOverText.SetActive(true);
            rb.velocity = Vector3.zero;
        }
        playerUIManager.UpdataHP(hp);
        Debug.Log("Player残りHP :" + hp);
    }

    private void FixedUpdate()
    {
        if (isDie)
        {
            return;
        }
        Vector3 direction = transform.position + new Vector3(x, 0, z) * moveSpeed;
        transform.LookAt(direction);
        rb.velocity = new Vector3(x, 0, z) * moveSpeed;
        animator.SetFloat("Speed", rb.velocity.magnitude);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isDie)
        {
            return;
        }

        Damager damager = other.GetComponent<Damager>();
        if (damager != null)
        {
            animator.SetTrigger("Hurt");
            Damage(damager.damage);
        }
    }

}
