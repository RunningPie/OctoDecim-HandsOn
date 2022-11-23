using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;   // UI Library
using TMPro;        // Text Mesh Pro Library

public enum BattleState
{
    START, PLAYERTURN, ENEMYTURN, WON, LOST
}

public class BattleSystem : MonoBehaviour
{

    public BattleState state;

    public GameObject playerPrefab;
    public GameObject enemyPrefab;

    public Transform playerPosition;
    public Transform enemyPosition;

    Unit playerUnit;
    Unit enemyUnit;

    // TextMeshProUGUI enemyName;
    // TextMeshProUGUI playerName;
    public TextMeshProUGUI actionText;
    public Button attackBtn;
    public Button moveBtn;

    // Start is called before the first frame update
    void Start()
    {
        state = BattleState.START;      // Set BattleSystem state to START
        StartCoroutine(SetupBattle());
        
    }

    IEnumerator SetupBattle()
    {
        actionText.gameObject.SetActive(false);
        moveBtn.gameObject.SetActive(false);
        attackBtn.gameObject.SetActive(false);

        // Grab player game object reference
        GameObject playerGO = Instantiate(playerPrefab, playerPosition);
        playerUnit = playerGO.GetComponent<Unit>();

        // Grab enemy game object reference
        GameObject enemyGO = Instantiate(enemyPrefab, enemyPosition);
        enemyUnit = enemyGO.GetComponent<Unit>();

        // // Display enemy name on TMP GUI
        // enemyName = enemyGO.transform.GetChild(1).GetChild(0).gameObject.GetComponent<TextMeshProUGUI>();
        // enemyName.text = enemyUnit.unitName;

        // // Display player name on TMP GUI
        // playerName = playerGO.transform.GetChild(1).GetChild(0).gameObject.GetComponent<TextMeshProUGUI>();
        // playerName.text = playerUnit.unitName;

        yield return new WaitForSeconds(2f);

        state = BattleState.PLAYERTURN;
        PlayerTurn();
    }

    IEnumerator PlayerAttack()
    {
        // DAMAGE ENEMY
        Debug.Log("Hit Enemy");
        bool isDead = enemyUnit.TakeDamage(playerUnit.damage);
        yield return new WaitForSeconds(2f);

        // CHECK IF ENEMY IS DEAD
        if (isDead)
        {
            // END BATTLE
            state = BattleState.WON;
            EndBattle();
        }
        else
        {
            // ENEMY TURN
            state = BattleState.ENEMYTURN;
            StartCoroutine(EnemyTurn());
        }

    }

    IEnumerator EnemyTurn()
    {
        Debug.Log("Enemy is attacking");

        yield return new WaitForSeconds(1f);

        bool isDead = enemyUnit.TakeDamage(playerUnit.damage);
        
        yield return new WaitForSeconds(1f);

        if (isDead)
        {
            state = BattleState.LOST;
            EndBattle();
        }
        else
        {
            state = BattleState.PLAYERTURN;
            PlayerTurn();
        }

    }

    IEnumerator PlayerHeal()
    {
        playerUnit.Heal(5);

        yield return new WaitForSeconds(2f);

        state = BattleState.ENEMYTURN;
        StartCoroutine(EnemyTurn());
    }

    void EndBattle()
    {
        if (state == BattleState.WON)
        {
            Debug.Log("You Won");
        }
        else if (state == BattleState.LOST)
        {
            Debug.Log("You Lost");
        }
    }

    void PlayerTurn()
    {
        Debug.Log("Players Turn");
        actionText.gameObject.SetActive(true);
        moveBtn.gameObject.SetActive(true);
        attackBtn.gameObject.SetActive(true);

    }

    public void OnAttackButton()
    {
        if (state != BattleState.PLAYERTURN)
        {
            return;
        }

        StartCoroutine(PlayerAttack());
    }

    public void OnHealButton()
    {
        if (state != BattleState.PLAYERTURN)
        {
            return;
        }

        StartCoroutine(PlayerHeal());
    }
}
