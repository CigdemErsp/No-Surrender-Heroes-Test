using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Avatar;

public class Turret : Unit
{
    [SerializeField] private GameObject _turretHead;

    private void Start()
    {
        maxHealth = 1000;
        currentHealth = maxHealth;
        speed = 0;
        range = 10f;
        damage = 20;
        teamTag = transform.tag;
        point = 40;
        _animator.SetBool("Idle", true);
    }

    // Update is called once per frame
    void Update()
    {
        if (!gameEnded)
        {
            GameObject nearestEnemy = FindNearestEnemy(); // Find the closest enemy

            // Calculate the direction to the target
            Vector3 dir = (nearestEnemy.transform.position - transform.position).normalized;

            // Create the rotation we need to be in to look at the target
            Quaternion lookRotation = Quaternion.LookRotation(dir);

            // Smoothly rotate towards the target
            _turretHead.transform.rotation = Quaternion.Slerp(_turretHead.transform.rotation, lookRotation, Time.deltaTime * 5f);
            float dist = Vector3.Distance(transform.position, nearestEnemy.transform.position);

            if (nearestEnemy == null || dist > range)
            {
                isAttacking = false;
                _animator.SetBool("Attacking", false);
                _animator.SetBool("Idle", true);
                isAttacking = false;
                return;
            }
            else if (nearestEnemy.GetComponent<Unit>() != _currentTarget)
            {
                _currentTarget = nearestEnemy.GetComponent<Avatar>();
            }
            else if (!isAttacking)
            {
                isAttacking = true;
                _animator.SetBool("Idle", false);
                _animator.SetBool("Attacking", true);
                StartCoroutine(WaitForAnimationToFinish());
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
