using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDetectorEnemy : MonoBehaviour
{
    public Enemy enemy;

    private void Start()
    {
        enemy = GetComponent<Enemy>();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            enemy.isPatrolling = false;
        }

    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            enemy.isPatrolling = true;

        }
    }
}