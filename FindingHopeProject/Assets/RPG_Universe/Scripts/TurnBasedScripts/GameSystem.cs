using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;
using UnityEngine.SceneManagement;

public enum BattleState { START, PLAYERTURN, ENEMYTURN, WON, LOST }

public class GameSystem : MonoBehaviour
{
    public BattleState state;

    public GameObject playerPrefab;
    public GameObject enemyPrefab;

    Fighter player;
    Fighter enemy;

    public Text dialogueText;

    public StatusHUD playerHUD;
    public StatusHUD enemyHUD;

    void Start()
    {
        state = BattleState.START;
        StartCoroutine(SetupBattle());
    }

    IEnumerator SetupBattle()
    {
        player = playerPrefab.GetComponent<Fighter>();
        enemy = enemyPrefab.GetComponent<Fighter>();

        dialogueText.text = "You were challenged by " + enemy.fighterName;

        playerHUD.SetHUD(player);
        enemyHUD.SetHUD(enemy);

        yield return new WaitForSeconds(2f);

        if (player.speed > enemy.speed)
        {
            state = BattleState.PLAYERTURN;
            PlayerTurn();
        } else
        {
            state = BattleState.ENEMYTURN;
            StartCoroutine(EnemyTurn());
        }
    }

    IEnumerator PlayerAttack()
    {
        int damage = 0;
        string message = "";
        bool lockDamage = false;

        // Miss chance 
        float dice = UnityEngine.Random.Range(0f, 1f);
        if (dice < player.missRate)
        {
            damage = 0;
            message = player.fighterName + " missed!";
            lockDamage = true;
        }

        // crit chance
        dice = UnityEngine.Random.Range(0f, 1f);
        if (dice < player.critRate && !lockDamage)
        {
            damage = (int)Math.Round(player.currentDamage * 1.2);
            message = "It's a CRITICAL Hit!";
        } else if (!lockDamage)
        {
            message = player.fighterName + " attacked!";
            damage = player.currentDamage;
        }

        dialogueText.text = message;
        yield return new WaitForSeconds(2f);

        bool isDead = enemy.TakeDamage(damage);
        enemyHUD.SetHP(enemy);

        if (isDead)
        {
            dialogueText.text = enemy.fighterName + " fainted!";
            yield return new WaitForSeconds(2f);
            state = BattleState.WON;
            StartCoroutine(EndBattle());
        }
        else 
        {
            dialogueText.text = enemy.fighterName + "'s turn!";
            yield return new WaitForSeconds(2f);
            state = BattleState.ENEMYTURN;
            StartCoroutine(EnemyTurn());
        }

    }

    IEnumerator EnemyTurn()
    {
        dialogueText.text = "What will " + enemy.fighterName + " do?";
        yield return new WaitForSeconds(2f);

        bool isDead = false;

        float dice = UnityEngine.Random.Range(0f, 1f);
        if (dice <= 0.2)
        {
            enemy.AttackBooster();
            dialogueText.text = enemy.fighterName + " used an Attack Booster!";
            yield return new WaitForSeconds(2f);
        } else if (dice <= 0.4)
        {
            enemy.DefenseBooster();
            dialogueText.text = enemy.fighterName + " used a Defense Booster!";
            yield return new WaitForSeconds(2f);
        } else
        {
            int damage = 0;
            bool lockDamage = false;

            // Miss chance 
            dice = UnityEngine.Random.Range(0f, 1f);
            if (dice < enemy.missRate)
            {
                damage = 0;
                dialogueText.text = enemy.fighterName + " missed!";
                yield return new WaitForSeconds(2f);
                lockDamage = true;
            }

            // crit chance
            dice = UnityEngine.Random.Range(0f, 1f);
            if (dice < enemy.critRate && !lockDamage)
            {
                damage = (int)Math.Round(enemy.currentDamage * 1.2);
                dialogueText.text = "It's a CRITICAL Hit!";
                yield return new WaitForSeconds(2f);
            }
            else if (!lockDamage)
            {
                dialogueText.text = enemy.fighterName + " attacked!";
                yield return new WaitForSeconds(2f);
                damage = enemy.currentDamage;
            }

            isDead = player.TakeDamage(damage);
            playerHUD.SetHP(player);
        }

        yield return new WaitForSeconds(2f);

        if (isDead)
        {
            state = BattleState.LOST;
            StartCoroutine(EndBattle());
        } else
        {
            state = BattleState.PLAYERTURN;
            PlayerTurn();
        }
    }

    IEnumerator EndBattle()
    {
        if (state == BattleState.WON)
        {
            dialogueText.text = player.fighterName + " has defeated " + enemy.fighterName + " !";
        } else if (state == BattleState.LOST)
        {
            dialogueText.text = enemy.fighterName + " has defeated " + player.fighterName + " !";
        }

        yield return new WaitForSeconds(1.5f);
        SceneManager.LoadScene("LabyrinthScene");

    }

    void PlayerTurn()
    {
        dialogueText.text = "What will " + player.fighterName + " do?";
    }

    public void OnAttackButton()
    {
        if (state != BattleState.PLAYERTURN) return;

        StartCoroutine(PlayerAttack());
    }

    public void OnBagButton()
    {
        if (state != BattleState.PLAYERTURN) return;
        dialogueText.text = player.fighterName + " opened the bag!";
    }

    public void OnRunButton()
    {
        if (state != BattleState.PLAYERTURN) return;
        StartCoroutine(Waiter());
    }

    IEnumerator Waiter()
    {
        float dice = UnityEngine.Random.Range(0f, 1f);
        if (dice < 0.15f)
        {
            dialogueText.text = player.fighterName + " ran from " + enemy.fighterName + "!";
            yield return new WaitForSeconds(2f);
            SceneManager.LoadScene("LabyrinthScene");
        } else
        {
            dialogueText.text = player.fighterName + " could not run from " + enemy.fighterName + "!";
            yield return new WaitForSeconds(1f);
        }
    }
}
