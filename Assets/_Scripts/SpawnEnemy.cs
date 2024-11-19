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

    [SerializeField] private Canvas _canvas;
    [SerializeField] private List<GameObject> _gameObject;
    [SerializeField] private RectTransform canvasRectTransform; // The canvas where you want to spawn enemies

    private float spawnTime = 2f;

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

                Vector2 canvasSize = canvasRectTransform.sizeDelta;

                float randomX = Random.Range(0, canvasSize.x);
                float randomY = Random.Range((canvasSize.y / 2) + 100, canvasSize.y);

                // Convert the canvas position into world space
                Vector2 spawnPosition = new Vector2(randomX, randomY);

                Ray ray = Camera.main.ScreenPointToRay(new Vector3(spawnPosition.x, spawnPosition.y, 0));

                // Variable to store hit information from the raycast
                RaycastHit hit;

                // Perform the raycast to detect any 3D object in the world
                if (Physics.Raycast(ray, out hit))
                {
                    // If the ray hits a 3D object, instantiate the game object at the hit point
                    Vector3 spawnPosition3D = hit.point;
                    GameObject _newEnemy = Instantiate(_enemy, spawnPosition3D, Quaternion.identity);
                    _newEnemy.transform.Rotate(0, 180, 0);
                    _newEnemy.GetComponent<Avatar>().maxHealth = _newEnemy.GetComponent<Avatar>().maxHealth + (_level * 10);
                    _newEnemy.GetComponent<Avatar>().damage += (_level * 2);
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
    }

}