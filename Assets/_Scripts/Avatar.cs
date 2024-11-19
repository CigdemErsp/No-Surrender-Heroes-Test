using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Avatar : Unit
{  
    private void Start()
    {
        maxHealth = 100;
        currentHealth = maxHealth;
        speed = 2f;
        teamTag = transform.tag;
        point = 20;
        _animator.SetBool("Idle", true);
    }

    // Update is called once per frame
    void Update()
    {
        if (!gameEnded)
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
            else if (nearestEnemy.GetComponent<Unit>() != _currentTarget)
            {
                _currentTarget = nearestEnemy.GetComponent<Unit>();
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
                else if (!isAttacking)
                {
                    isAttacking = true;
                    _animator.SetBool("Moving", false);
                    _animator.SetBool("Attacking", true);
                    StartCoroutine(WaitForAnimationToFinish());
                }
            }
        }
    }

    void OnEnable()
    {
        Game.OnGameEnd += GameEnd; // Subscribe to the event
    }

    void OnDisable()
    {
        Game.OnGameEnd -= GameEnd; // Unsubscribe to avoid memory leaks
    }

    public void GameEnd()
    {
        StopAllCoroutines();
        gameEnded = true;
    }

}
