using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public enum GameState
{
    pacifist,
    neutral,
    genocide
}
public class GameController : MonoBehaviour
{
    
    public Player player;
    public Boss boss;
    public CameraMovement cam;
    public AudioSource music;
    public MenuScript menu;


    [SerializeField] private GameState state;

    public float deadEnemyCounter;

    private bool canIncrease;

    // Start is called before the first frame update
    void Start()
    {
        canIncrease = true;
        cam.GetComponent<PostProcessVolume>().enabled = false;
        deadEnemyCounter = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (player.getIsDead())
        {
            StartCoroutine(ResetGame());
        }
        if (boss.isActive)
        {
            canIncrease = false;
        }

        if (!boss.isActive)
        {
            if (deadEnemyCounter == 0)
            {
                boss.health = 5;
                boss.minAttackDelay = 3.0f;
                boss.maxAttackDelay = 6.0f;
                boss.teleportChance = 0;
                state = GameState.pacifist;

            }
            else if (deadEnemyCounter <= 5)
            {
                boss.health = 10;
                boss.minAttackDelay = 2.0f;
                boss.maxAttackDelay = 5.0f;
                boss.teleportChance = 3.5f;
                state = GameState.neutral;
            }
            else if (deadEnemyCounter <= 9)
            {
                music.pitch = 0.9f;
                boss.health = 15;
                boss.minAttackDelay = 1.0f;
                boss.maxAttackDelay = 3.0f;
                boss.teleportChance = 5.0f;
                state = GameState.neutral;
            }
            else if (deadEnemyCounter == 10)
            {
                music.pitch = 0.8f;
                boss.health = 20;
                boss.minAttackDelay = 0.0f;
                boss.maxAttackDelay = 1.0f;
                boss.teleportChance = 8.0f;
                cam.GetComponent<PostProcessVolume>().enabled = true;
                state = GameState.genocide;
            }
        }


    }

    private IEnumerator ResetGame()
    {
        yield return new WaitForSeconds(5.0f);
        menu.paused = true;
    }

    public void deadEnemyUp()
    {
        if (canIncrease)
        {
            deadEnemyCounter++; 
        }
    }

    public GameState getGameState()
    {
        return state;
    }
}
