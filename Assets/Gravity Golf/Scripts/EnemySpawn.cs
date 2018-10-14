using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawn : MonoBehaviour {
    public GameObject enemy;
    GameObject player;
    MapGenerator generator;
	// Use this for initialization
	void Start () {
        player = GameManager.instance.player;
        generator = MapGenerator.instance;
	}

    public void SpawnEnemy() {
        GameObject e = Instantiate(enemy, player.transform.position, Quaternion.Euler(Vector3.zero));
        generator.spawnedEnemies.Add(e.GetComponentInChildren<EnemyController>());
    }
}
