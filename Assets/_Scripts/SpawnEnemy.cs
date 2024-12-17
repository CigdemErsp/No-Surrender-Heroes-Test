using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SpawnEnemy : MonoBehaviour
{
    private int _level = 1;
    private int maxMana = 10;
    private int _currentMana;
    private int manaRegenAmount = 1;  // Amount of mana regenerated
    private float manaRegenRate = 4f;

    private Vector2 originalPosition;

    [SerializeField] private List<GameObject> _gameObject;
    [SerializeField] private GameObject gameArena;

    private float spawnTime = 4f;

    public List<GameObject> playerUnits; // List of player-controlled units

    void Start()
    {
        playerUnits = FindObjectsOfType<Unit>().Select(unit => unit.gameObject).ToList();
        _level = SceneManager.GetActiveScene().buildIndex; 
        _currentMana = maxMana;
        StartCoroutine(SummonEnemy());
        StartCoroutine(RegenerateMana());
    }

    // Regenerate mana over time
    IEnumerator RegenerateMana()
    {
        float elapsedTime = 0f; // Timer for the current regen interval

        while (true)
        {
            if (_currentMana < maxMana)
            {
                elapsedTime += Time.deltaTime;

                if (elapsedTime >= manaRegenRate)
                {
                    _currentMana += manaRegenAmount; // Regenerate mana after the interval
                    elapsedTime -= manaRegenRate;   // Reset timer
                }
            }
            yield return null; // Wait for the next frame
        }
    }

    IEnumerator SummonEnemy()
    {
        while (true) // Check spawn count in the loop condition
        {
            yield return new WaitForSeconds(spawnTime);

            float randomEnemyIndex = Random.Range(0, _gameObject.Count);
            GameObject _enemy = _gameObject[(int)randomEnemyIndex];

            if (_currentMana < _enemy.GetComponent<Avatar>().manaCost)
                yield return null;
            else
            {
                _currentMana -= _enemy.GetComponent<Avatar>().manaCost;

                GameObject target = GetBestTarget();

                if (target != null)
                {
                    string tmp = "Enemy " + _enemy.GetComponent<Unit>().currentHealth;
                    Debug.Log(tmp);
                    SummonEnemyNearTarget(target, _enemy);
                }
                else
                {
                    Renderer cubeRenderer = gameArena.GetComponent<Renderer>();
                    Bounds cubeBounds = cubeRenderer.bounds;

                    // Get min and max world positions of the side
                    Vector3 min = cubeBounds.min; // Bottom-left corner
                    Vector3 max = cubeBounds.max; // Top-right corner

                    float randomX = Random.Range(min.x, max.x);
                    float randomY = Random.Range(min.y, max.y);
                    float randomZ = Random.Range((max.z + min.z) / 2, max.z);

                    // Convert the canvas position into world space
                    Vector3 spawnPosition = new Vector3(randomX, randomY, randomZ);

                    GameObject _newEnemy = Instantiate(_enemy, spawnPosition, Quaternion.identity);
                    _newEnemy.transform.Rotate(0, 180, 0);
                    _newEnemy.GetComponent<Avatar>().maxHealth = _newEnemy.GetComponent<Avatar>().maxHealth + (_level * 2);
                    _newEnemy.GetComponent<Avatar>().damage += (_level * 2);
                }
            }
        }
    }

    private void SummonEnemyNearTarget(GameObject target, GameObject enemy)
    {
        string tmp = "Enemy " + target.GetComponent<Unit>().transform.position;
        Debug.Log(tmp);
        Renderer cubeRenderer = gameArena.GetComponent<Renderer>();
        Bounds cubeBounds = cubeRenderer.bounds;

        // Get min and max world positions of the side
        Vector3 min = cubeBounds.min; // Bottom-left corner
        Vector3 max = cubeBounds.max; // Top-right corner

        Vector3 pos = FindNearestSummonablePosition(target.transform.position, cubeBounds, enemy.GetComponent<Unit>().range);

        GameObject _newEnemy = Instantiate(enemy, pos, Quaternion.identity);
        _newEnemy.transform.Rotate(0, 180, 0);
        _newEnemy.GetComponent<Avatar>().maxHealth = _newEnemy.GetComponent<Avatar>().maxHealth + (_level * 2);
        _newEnemy.GetComponent<Avatar>().damage += (_level * 2);
    }

    private Vector3 FindNearestSummonablePosition(Vector3 targetPosition, Bounds cubeBounds, float range)
    {
        // Clamp the target position within the upper half of the cube bounds
        float clampedX = Mathf.Clamp(targetPosition.x, cubeBounds.min.x, cubeBounds.max.x); // Full range on X-axis
        float clampedY = Mathf.Clamp(targetPosition.y, cubeBounds.min.y, cubeBounds.max.y); // Full range on Y-axis
        float clampedZ = Mathf.Clamp(targetPosition.z, (cubeBounds.max.z + cubeBounds.min.z) / 2, cubeBounds.max.z); // Upper half on Z-axis

        // Create the clamped position
        Vector3 clampedPosition = new Vector3(clampedX, clampedY, clampedZ);

        // Calculate the direction to the target and distance
        Vector3 directionToTarget = (targetPosition - clampedPosition).normalized;
        float distanceToTarget = Vector3.Distance(clampedPosition, targetPosition);

        // If the distance exceeds the enemy's range, adjust the position to stay within range
        if (distanceToTarget > range)
        {
            // Move the position closer to the target but keep it within the upper half
            clampedPosition += directionToTarget * (distanceToTarget - range);

            // Ensure the adjusted position remains within the upper half of the cube bounds
            clampedPosition.z = Mathf.Clamp(clampedPosition.z, (cubeBounds.max.z + cubeBounds.min.z) / 2, cubeBounds.max.z);
        }

        return clampedPosition;
    }



    public void RegisterUnit(GameObject unit)
    {
        if (unit.CompareTag("Team 2") && !playerUnits.Contains(unit))
            playerUnits.Add(unit);
    }

    public void DeregisterUnit(GameObject unit)
    {
        if (playerUnits.Contains(unit))
            playerUnits.Remove(unit);
    }

    private GameObject GetBestTarget()
    {
        GameObject bestTarget = null;
        float lowestHealth = int.MaxValue;

        foreach (GameObject unit in playerUnits)
        {
            if (unit != null)
            {
                float health = unit.GetComponent<Unit>().currentHealth;
                if (health < lowestHealth)
                {
                    lowestHealth = health;
                    bestTarget = unit;
                }
            }
        }
        return bestTarget;
    }

    void OnEnable()
    {
        Game.OnGameEnd += GameEnd; // Subscribe to the event
        DragDrop.OnNewUnitSummoned += RegisterUnit;
        Unit.OnUnitDeath += DeregisterUnit;
    }

    void OnDisable()
    {
        Game.OnGameEnd -= GameEnd; // Unsubscribe to avoid memory leaks
        DragDrop.OnNewUnitSummoned -= RegisterUnit;
        Unit.OnUnitDeath -= DeregisterUnit;
    }

    public void GameEnd()
    {
        StopAllCoroutines();
    }

}