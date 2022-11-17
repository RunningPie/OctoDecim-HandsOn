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

    TextMeshProUGUI enemyName;
    TextMeshProUGUI playerName;

    // Start is called before the first frame update
    void Start()
    {
        state = BattleState.START;      // Set BattleSystem state to START
        SetupBattle();
    }

    void SetupBattle()
    {
        // Grab player game object reference
        GameObject playerGO = Instantiate(playerPrefab, playerPosition);
        playerUnit = playerGO.GetComponent<Unit>();

        // Grab enemy game object reference
        GameObject enemyGO = Instantiate(enemyPrefab, enemyPosition);
        enemyUnit = enemyGO.GetComponent<Unit>();

        // Display enemy name on TMP GUI
        enemyName = enemyGO.transform.GetChild(1).GetChild(0).gameObject.GetComponent<TextMeshProUGUI>();
        enemyName.text = enemyUnit.unitName;

        // Display player name on TMP GUI
        playerName = playerGO.transform.GetChild(1).GetChild(0).gameObject.GetComponent<TextMeshProUGUI>();
        playerName.text = playerUnit.unitName;
    }

}
