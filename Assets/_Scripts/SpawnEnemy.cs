using System.Collections;
using System.Collections.Generic;
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

    void Start()
    {
        _level = SceneManager.GetActiveScene().buildIndex; 
        _currentMana = maxMana;
        StartCoroutine(SpawnEnemyRandomly());
        StartCoroutine(RegenerateMana());
    }

    // Regenerate mana over time
    IEnumerator RegenerateMana()
    {
        while (true)
        {
            if (_currentMana < maxMana)
            {
                _currentMana += manaRegenAmount;
                // Debug.Log($"Regenerate {_currentMana}");
            }
            yield return new WaitForSeconds(manaRegenRate);
        }
    }

    IEnumerator SpawnEnemyRandomly()
    {
        while (true) // Check spawn count in the loop condition
        {
            yield return new WaitForSeconds(spawnTime);
            float randomEnemyIndex = Random.Range(0, _gameObject.Count);
            GameObject _enemy = _gameObject[(int)randomEnemyIndex];

            string tmp = "Try to spawn " + _currentMana;
            // Debug.Log(tmp);

            if (_currentMana < _enemy.GetComponent<Avatar>().manaCost)
                yield return null;
            else
            {
                _currentMana -= _enemy.GetComponent<Avatar>().manaCost;
                // tmp = "Spawn " + _currentMana;
                // Debug.Log(tmp);

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

                // If the ray hits a 3D object, instantiate the game object at the hit point
                GameObject _newEnemy = Instantiate(_enemy, spawnPosition, Quaternion.identity);
                _newEnemy.transform.Rotate(0, 180, 0);
                _newEnemy.GetComponent<Avatar>().maxHealth = _newEnemy.GetComponent<Avatar>().maxHealth + (_level * 2);
                _newEnemy.GetComponent<Avatar>().damage += (_level * 2);
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
    }

}