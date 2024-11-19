using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    public delegate void SoldierDiedHandler(int points);
    public static event SoldierDiedHandler OnSoldierDied;

    public delegate void Score(int points);
    public static event Score ScoreUp;

    public delegate void HealthBarHandler();
    public static event HealthBarHandler OnDamageTaken;

    public int maxHealth; // Max health of the soldier
    public int currentHealth;
    public float speed; // Speed of the soldier
    public string teamTag; // Team tag
    public float range; // Range of weapon
    public int damage = 10;
    public int point;
    public int manaCost = 2;

    public Animator _animator;

    public bool isAttacking = false;
    public bool isDead = false;
    public bool gameEnded = false;

    public Unit _currentTarget;

    public void SoldierDied(int points)
    {
        OnSoldierDied?.Invoke(points);
    }

    public void UpdateScore(int points)
    {
        ScoreUp?.Invoke(points);
    }

    public GameObject FindNearestEnemy()
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
        Unit[] allObjects = GameObject.FindObjectsOfType<Unit>();
        List<GameObject> nonTeamObjects = new List<GameObject>();

        foreach (Unit obj in allObjects)
        {
            if (obj.tag != teamTag && !obj.isDead) // Exclude objects with the specified tag
            {
                nonTeamObjects.Add(obj.gameObject);
            }
        }

        return nonTeamObjects.ToArray();
    }

    public void Shoot()
    {
        // Debug.Log(_currentTarget);
        if (!_currentTarget.isDead)
        {
            _currentTarget.currentHealth -= damage;
            OnDamageTaken?.Invoke();
            // string tmp = _currentTarget.name + " " + _currentTarget.teamTag + " " + _currentTarget.currentHealth;
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
                    UpdateScore(_currentTarget.point);
                }
                else
                {
                    SoldierDied(_currentTarget.point);
                }
                _currentTarget = null;
            }
        }
    }

    public IEnumerator WaitForAnimationToFinish()
    {
        AnimatorStateInfo animationState = _animator.GetCurrentAnimatorStateInfo(0);

        while (_animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0f)
        {
            yield return null;
            animationState = _animator.GetCurrentAnimatorStateInfo(0);
        }
    }

    public void OnDeath()
    {
        Destroy(gameObject);
    }
}
