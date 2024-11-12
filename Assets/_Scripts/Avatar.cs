using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Avatar : MonoBehaviour
{
    public delegate void SoldierDiedHandler();
    public static event SoldierDiedHandler OnSoldierDied;

    public delegate void Score(int points);
    public static event Score ScoreUp;

    public delegate void HealthBarHandler();
    public static event HealthBarHandler OnDamageTaken;

    [SerializeField] private float speed = 2f; // Speed of the soldier
    private string teamTag; // Team tag
    [SerializeField] private float range; // Range of weapon
    public int damage = 10;

    [SerializeField] private Animator _animator;

    private int maxHealth = 100; // Max health of the soldier
    private int currentHealth = 100;

    private Avatar _currentTarget;

    private bool isAttacking = false;
    private bool isDead = false;

    [SerializeField] private int point;

    public int manaCost = 2;

    public int getCurrentHealth()
    {
        return currentHealth;
    }

    public int getMaxHealth()
    {
        return maxHealth;
    }

    public void setMaxHealth(int health)
    {
        maxHealth = health;
    }

    private void Start()
    {
        currentHealth = maxHealth;
        teamTag = transform.tag;
        _animator.SetBool("Idle", true);
    }

    // Update is called once per frame
    void Update()
    {
        GameObject nearestEnemy = FindNearestEnemy(); // Find the closest enemy

        if (nearestEnemy == null)
        {
            _animator.SetBool("Attacking", false);
            _animator.SetBool("Idle", true);
            _animator.SetBool("Moving", false);
            isAttacking = false;
            return;
        }
        else if (nearestEnemy.GetComponent<Avatar>() != _currentTarget)
        {
            _currentTarget = nearestEnemy.GetComponent<Avatar>();
        }
        else
        {
            // Calculate the direction to the target
            Vector3 dir = (nearestEnemy.transform.position - transform.position).normalized;

            // Create the rotation we need to be in to look at the target
            Quaternion lookRotation = Quaternion.LookRotation(dir);

            // Smoothly rotate towards the target
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);

            _animator.SetBool("Idle", false);
            float dist = Vector3.Distance(transform.position, nearestEnemy.transform.position);

            if (dist > range)
            {
                isAttacking = false;
                // Move towards the enemy
                _animator.SetBool("Moving", true);
                _animator.SetBool("Attacking", false);
                Vector3 direction = (nearestEnemy.transform.position - transform.position).normalized;
                transform.position += direction * speed * Time.deltaTime;
            }
            else if(!isAttacking)
            {
                isAttacking = true;
                _animator.SetBool("Moving", false);
                _animator.SetBool("Attacking", true);
                StartCoroutine(WaitForAnimationToFinish());
            }
        }

    }

    GameObject FindNearestEnemy()
    {
        GameObject[] enemies = FindEnemies(); // Find all enemies
        GameObject nearest = null;
        float minDist = Mathf.Infinity;

        foreach (GameObject enemy in enemies)
        {
            float dist = Vector3.Distance(transform.position, enemy.transform.position);
            if (dist < minDist)
            {
                nearest = enemy;
                minDist = dist;
            }
        }
        return nearest;
    }

    GameObject[] FindEnemies()
    {
        Avatar[] allObjects = GameObject.FindObjectsOfType<Avatar>();
        List<GameObject> nonTeamObjects = new List<GameObject>();

        foreach (Avatar obj in allObjects)
        {
            if (obj.tag != teamTag) // Exclude objects with the specified tag
            {
                nonTeamObjects.Add(obj.gameObject);
            }
        }

        return nonTeamObjects.ToArray();
    }

    void Shoot()
    {
        if (!_currentTarget.isDead) {
            _currentTarget.currentHealth -= damage;
            OnDamageTaken?.Invoke();
            string tmp = _currentTarget.name + " " + _currentTarget.teamTag + " " + _currentTarget.currentHealth;
            // Debug.Log(tmp);

            if (_currentTarget.currentHealth <= 0)
            {
                _currentTarget._animator.SetTrigger("Death");
                _animator.SetBool("Attacking", false);
                _animator.SetBool("Idle", true);
                isAttacking = false;
                _currentTarget.isDead = true;

                if (_currentTarget.tag == "Team 1")
                {
                    OnSoldierDied?.Invoke();
                    ScoreUp?.Invoke(_currentTarget.point);
                }

            }
        }
    }

    IEnumerator WaitForAnimationToFinish()
    {
        AnimatorStateInfo animationState = _animator.GetCurrentAnimatorStateInfo(0);

        while(_animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0f)
        {
            yield return null;
            animationState = _animator.GetCurrentAnimatorStateInfo(0);
        }


    }

    void OnDeath()
    {
        Destroy(gameObject);
    }

}
