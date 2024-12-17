using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Unit : MonoBehaviour
{
    public delegate void SoldierDiedHandler(int points);
    public static event SoldierDiedHandler OnSoldierDied;

    public delegate void Score(int points);
    public static event Score ScoreUp;

    public delegate void HealthBarHandler();
    public static event HealthBarHandler OnDamageTaken;

    public delegate void TurretDestroyed(string teamTag);
    public static event TurretDestroyed OnTurretDestroy;

    public delegate void UnitDeathHandler(GameObject unit);
    public static event UnitDeathHandler OnUnitDeath;

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

    [SerializeField] private Transform bulletSpawnPoint;
    [SerializeField] private GameObject bullet;
    [SerializeField] private float bulletSpeed = 10;

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

                if(_currentTarget.GetComponent<Turret>() != null)
                {
                    Unit.OnTurretDestroy(_currentTarget.teamTag);
                }
                else if (_currentTarget.tag == "Team 1")
                {
                    UpdateScore(_currentTarget.point);
                }
                else
                {
                    OnUnitDeath?.Invoke(gameObject);
                    SoldierDied(_currentTarget.point);
                }
                _currentTarget = null;
            }
        }
    }

    void ShootBullet()
    {
        if (_currentTarget != null)
        {
            var newBullet = Instantiate(bullet, bulletSpawnPoint.position, bulletSpawnPoint.rotation);

            float animationDuration = _animator.GetCurrentAnimatorStateInfo(0).length;
            StartCoroutine(TrackAndHitTarget(newBullet, animationDuration));
        }
    }

    IEnumerator TrackAndHitTarget(GameObject bullet, float animDuration)
    {
        while (bullet != null)
        {
            // Calculate direction and move the bullet
            Vector3 direction = (_currentTarget.transform.position - bullet.transform.position).normalized;
            bullet.transform.position += direction * bulletSpeed * Time.deltaTime;

            // Rotate the bullet to face the target
            bullet.transform.rotation = Quaternion.LookRotation(direction);

            // Check if the bullet is close enough to "hit" the target
            if (Vector3.Distance(bullet.transform.position, _currentTarget.transform.position) < 1f)
            {
                // Bullet hits the target
                Destroy(bullet);
                yield break;
            }

            yield return null; // Wait for the next frame
        }

        // Cleanup if target is dead or null
        if (bullet != null) Destroy(bullet);
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
