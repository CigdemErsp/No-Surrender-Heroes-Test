using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnEnemy : MonoBehaviour
{
    private Vector2 originalPosition;

    [SerializeField] private Canvas _canvas;
    [SerializeField] private List<GameObject> _gameObject;
    [SerializeField] private RectTransform canvasRectTransform; // The canvas where you want to spawn enemies

    private float spawnTime = 2f;
    private int spawnMaxCount = 5;
    public int spawnCount;

    void Start()
    {
        spawnCount = 2;
        StartCoroutine(SpawnEnemyRandomly());
    }

    IEnumerator SpawnEnemyRandomly()
    {
        while (true) // Check spawn count in the loop condition
        {
            yield return new WaitForSeconds(spawnTime);

            if (spawnCount < spawnMaxCount) // Check if we can spawn more
            {
                float randomEnemyIndex = Random.Range(0, _gameObject.Count);
                GameObject _enemy = _gameObject[(int)randomEnemyIndex];

                Vector2 canvasSize = canvasRectTransform.sizeDelta;

                float randomX = Random.Range(0, canvasSize.x);
                float randomY = Random.Range(0, canvasSize.y);

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
                    Instantiate(_enemy, spawnPosition3D, Quaternion.identity);
                    spawnCount++;
                    Debug.Log("Soldier Spawned! Current Count: " + spawnCount);
                }
            }
            else
            {
                // If we hit the max count, wait before checking again
                yield return null; // Wait for the next frame
            }
        }
    }

    void OnEnable()
    {
        Avatar.OnSoldierDied += DecreaseSpawnCount; // Subscribe to the event
    }

    void OnDisable()
    {
        Avatar.OnSoldierDied -= DecreaseSpawnCount; // Unsubscribe to avoid memory leaks
    }

    public void DecreaseSpawnCount()
    {
        if (spawnCount > 0)
        {
            spawnCount--;
        }
    }
}