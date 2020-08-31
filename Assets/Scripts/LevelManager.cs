using redd096;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class LevelManager : MonoBehaviour
{
    [SerializeField] Enemy[] enemyPrefabs = default;

    public bool gameEnded { get; private set; }

    private void Start()
    {
        //spawn first enemy
        SpawnEnemy(3);

        //add events
        AddEvents();

        StartGame();
    }

    private void OnDestroy()
    {
        //remove events
        RemoveEvents();
    }

    #region private API

    #region start and end game

    void StartGame()
    {
        //hide end menu and set game ended false
        GameManager.instance.uiManager.EndMenu(false);
        gameEnded = false;
    }

    void EndGame()
    {
        //be sure to remove pause menu and resume game
        SceneLoader.instance.ResumeGame();

        //show end menu and set game ended
        GameManager.instance.uiManager.EndMenu(true);
        gameEnded = true;

        //show cursor and stop time
        Utility.LockMouse(CursorLockMode.None);
        Time.timeScale = 0;
    }

    #endregion

    #region events

    void AddEvents()
    {
        GameManager.instance.player.OnDead += OnPlayerDead;
    }

    void RemoveEvents()
    {
        GameManager.instance.player.OnDead -= OnPlayerDead;
    }

    void OnPlayerDead()
    {
        //end game after few seconds
        Invoke("EndGame", 2);
    }

    #endregion

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

    public static Vector3 RandomPositionOnNavMesh()
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
}
