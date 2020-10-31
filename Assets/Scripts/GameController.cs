using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

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
    public MusicController music;
    public GameObject map;
    public GameObject emptyArena;
    public MenuScript menu;
    public Image UI_health;
    public Image UI_ammo;
    public float arenaHeight;


    [SerializeField] private GameState state;

    public float deadEnemyCounter;

    private bool canIncrease;
    private bool playEnd;
    private bool playExit;

    public bool playerInBoss;

    // Start is called before the first frame update
    void Start()
    {
        playExit = false;
        playerInBoss = false;
        playEnd = false;
        canIncrease = true;
        cam.GetComponent<PostProcessVolume>().enabled = false;
        deadEnemyCounter = 0;
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log("emptyarena position" + emptyArena.transform.position);

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
                boss.maxAttackDelay = 4.0f;
                boss.teleportChance = 1.5f;
                state = GameState.pacifist;

            }
            else if (deadEnemyCounter <= 5)
            {
                boss.health = 10;
                boss.minAttackDelay = 2.0f;
                boss.maxAttackDelay = 3.0f;
                boss.teleportChance = 3.5f;
                state = GameState.neutral;
            }
            else if (deadEnemyCounter <= 9)
            {
                music.changePitchTo(0.9f);
                boss.health = 15;
                boss.minAttackDelay = 1.0f;
                boss.maxAttackDelay = 2.0f;
                boss.teleportChance = 5.0f;
                state = GameState.neutral;
            }
            else if (deadEnemyCounter == 10)
            {
                music.changePitchTo(0.8f);
                boss.health = 20;
                boss.minAttackDelay = 0.0f;
                boss.maxAttackDelay = 1.0f;
                boss.teleportChance = 8.0f;
                cam.GetComponent<PostProcessVolume>().enabled = true;
                state = GameState.genocide;
            }
        }

        if(playEnd)
        {
            StartCoroutine(EndCo());
        }
        if (playExit && playerInBoss)
        {
            StartCoroutine(ExitCo());
        }

    }

    public void activatePlayEnd()
    {
        playEnd = true;
    }

    private IEnumerator EndCo()
    {
        playEnd = false;
        music.stopAll();
        UI_health.enabled = false;
        UI_ammo.enabled = false;
        map.GetComponent<SpriteRenderer>().enabled = false;
        player.isLoading = true;
        emptyArena.SetActive(true);
        yield return new WaitForSeconds(1.3f);
        map.SetActive(false);
        yield return new WaitForSeconds(0.3f);
        player.transform.position = new Vector2(player.transform.position.x, boss.transform.position.y - 1.0f);
        emptyArena.transform.position = new Vector2(emptyArena.transform.position.x, boss.transform.position.y + 10);
        player.isLoading = false;
        cam.target = boss.transform;
        yield return new WaitForSeconds(1.0f);
        playExit = true;
    }
    private IEnumerator ExitCo()
    {
        playExit = false;
        Debug.Log("End");
        player.kill();
        yield return new WaitForSeconds(4.0f);
        boss.GetComponent<SpriteRenderer>().enabled = false;
        player.GetComponent<SpriteRenderer>().enabled = false;
        yield return new WaitForSeconds(1.0f);
        SceneManager.LoadScene("End");
    }
    private IEnumerator ResetGame()
    {
        yield return new WaitForSeconds(6.0f);
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
