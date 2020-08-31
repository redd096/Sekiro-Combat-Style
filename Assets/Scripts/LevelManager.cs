using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class LevelManager : MonoBehaviour
{
    [SerializeField] Enemy[] enemyPrefabs = default;

    private void Start()
    {
        SpawnEnemy(5);
    }

    #region private API

    Vector3 RandomPositionOnNavMesh()
    {
        //find random position
        Vector3 randomDirection = Random.insideUnitSphere * Random.Range(1, 100);

        //find random position on nav mesh
        NavMeshHit hit;
        NavMesh.SamplePosition(randomDirection, out hit, 100, 1);
        Vector3 finalPosition = hit.position;

        //set y
        //finalPosition.y = 0.5f;

        return finalPosition;
    }

    #endregion

    #region public API

    public void SpawnEnemy()
    {
        //instantiate new random enemy in random position
        int random = Random.Range(0, enemyPrefabs.Length);
        Enemy enemy = Instantiate(enemyPrefabs[random]);

        enemy.transform.position = RandomPositionOnNavMesh();
    }

    public void SpawnEnemy(float delayTime)
    {
        //spawn after delay
        Invoke("SpawnEnemy", delayTime);
    }

    #endregion
}
