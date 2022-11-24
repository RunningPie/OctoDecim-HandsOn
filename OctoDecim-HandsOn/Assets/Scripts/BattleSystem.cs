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

    PScript playerUnit;
    EScript enemyUnit;

    // TextMeshProUGUI enemyName;
    // TextMeshProUGUI playerName;
    public TextMeshProUGUI actionText;
    public Button attackBtn;
    public Button moveBtn;
    public Button upBtn;
    public Button dwnBtn;
    public Button leftBtn;
    public Button rightBtn;

    public float moveSpeed = 5f;
    public Transform movePoint;
    public bool PHasMoved = false;
    public bool PHasAttacked = false;

    public LayerMask obstacle;
    public LayerMask enemy;

    public bool isDead=false;
    // public float minHealth=0f;

    public GameObject playerGO;
    // public List<EScript> enemyList;

    // public Animator anim;

    // Start is called before the first frame update
    void Start()
    {
        state = BattleState.START;      // Set BattleSystem state to START
        actionText.gameObject.SetActive(false);
        moveBtn.gameObject.SetActive(false);
        attackBtn.gameObject.SetActive(false);
        upBtn.gameObject.SetActive(false);
        dwnBtn.gameObject.SetActive(false);
        leftBtn.gameObject.SetActive(false);
        rightBtn.gameObject.SetActive(false);

        playerGO = Instantiate(playerPrefab, playerPosition);
        //enemyGO = Instantiate(enemyPrefab, enemyPosition);

        movePoint = GameObject.FindGameObjectWithTag("PlayerMovePoint").GetComponent<Transform>();

        movePoint.parent = null;

        StartCoroutine(SetupBattle());
        
    }

    IEnumerator SetupBattle()
    {
        actionText.gameObject.SetActive(false);
        moveBtn.gameObject.SetActive(false);
        attackBtn.gameObject.SetActive(false);
        upBtn.gameObject.SetActive(false);
        dwnBtn.gameObject.SetActive(false);
        leftBtn.gameObject.SetActive(false);
        rightBtn.gameObject.SetActive(false);
        
        // Grab player game object reference
        playerUnit = playerGO.GetComponent<PScript>();

        // Grab enemy game object reference
        //enemyUnit = enemyGO.GetComponent<EScript>();

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

        // actionText.text = "Where do you want to attack to?";
        actionText.text = "Looking for weakest nearby enemy...";
        moveBtn.gameObject.SetActive(false);
        attackBtn.gameObject.SetActive(false);
        upBtn.gameObject.SetActive(false);
        dwnBtn.gameObject.SetActive(false);
        leftBtn.gameObject.SetActive(false);
        rightBtn.gameObject.SetActive(false);

        Collider2D[] colliderArray = Physics2D.OverlapCircleAll(playerGO.transform.position, 2.5f);
        float minHealth = 100;
        foreach (Collider2D collider2D in colliderArray)
        {
            EScript subEnemyUnit = collider2D.GetComponent<EScript>();
            if (subEnemyUnit.currentHP < minHealth)
            {
                minHealth = subEnemyUnit.currentHP;
            }
        }
        foreach (Collider2D collider2D in colliderArray)
        {
            EScript subEnemyUnit = collider2D.GetComponent<EScript>();
            if (subEnemyUnit.currentHP == minHealth)
            {
                subEnemyUnit.TakeDamage(playerUnit.damage);
                Debug.Log("Enemy damaged!");
            }
        }

        PHasAttacked = true;
        
        yield return new WaitUntil(() => PHasAttacked);

        // DAMAGE ENEMY
        // Debug.Log("Hit Enemy");

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

        actionText.gameObject.SetActive(true);
        actionText.text = "Waiting for enemy...";
        PHasMoved = false;
        PHasAttacked = false;
        moveBtn.gameObject.SetActive(false);
        attackBtn.gameObject.SetActive(false);
        upBtn.gameObject.SetActive(false);
        dwnBtn.gameObject.SetActive(false);
        leftBtn.gameObject.SetActive(false);
        rightBtn.gameObject.SetActive(false);

        yield return new WaitForSeconds(1f);

        // bool isDead = playerUnit.TakeDamage(enemyUnit.damage);
        // actionText.text = "Enemy attacked dealt " + enemyUnit.damage + " damage!";
        
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

    IEnumerator PlayerMove()
    {

        actionText.text = "Where do you want to move to?";
        moveBtn.gameObject.SetActive(false);
        attackBtn.gameObject.SetActive(false);
        upBtn.gameObject.SetActive(true);
        dwnBtn.gameObject.SetActive(true);
        leftBtn.gameObject.SetActive(true);
        rightBtn.gameObject.SetActive(true);

        yield return new WaitUntil(() => PHasMoved);

        state = BattleState.ENEMYTURN;
        Debug.Log("Switching to Enemy");
        StartCoroutine(EnemyTurn());
    }

    public void MoveUp()
    {
        playerGO.transform.position = Vector3.MoveTowards(playerGO.transform.position, movePoint.position, moveSpeed*Time.deltaTime);

        if (Vector3.Distance(playerGO.transform.position, movePoint.position) <= .05f)
        {
            // if (Mathf.Abs(Input.GetAxisRaw("Horizontal")) == 1f)
            // {
            //     if (!Physics2D.OverlapCircle(movePoint.position + new Vector3(Input.GetAxisRaw("Horizontal"), 0f, 0f), .2f, obstacle))
            //     {
            //         movePoint.position += new Vector3(Input.GetAxisRaw("Horizontal"), 0f, 0f);
            //     }
            // }
            // if (Mathf.Abs(Input.GetAxisRaw("Vertical")) == 1f)
            // {
            if (!Physics2D.OverlapCircle(movePoint.position + new Vector3(0f, 1f, 0f), .2f, obstacle))
            {
                movePoint.position += new Vector3(0f, 1f, 0f);
                PHasMoved = true;
            }
            else
            {
                actionText.text = "Cannot Move There!";
                PHasMoved = false;
            }
            // if (Physics2D.OverlapCircle(movePoint.position + new Vector3(0f, 1f, 0f), .5f, enemy))
            // {
            //     Collider2D collider2D = Physics2D.OverlapCircle(movePoint.position + new Vector3(0f, 1f, 0f), .5f, enemy);
            //     EScript subEnemyUnit = collider2D.GetComponent<EScript>();
            //     isDead = subEnemyUnit.TakeDamage(playerUnit.damage);
            //     Debug.Log("Enemy Attacked!");
            //     if (isDead)
            //     {
            //         Debug.Log("Enemy Died");
            //         subEnemyUnit.OnDeath();
            //     }
            //     PHasAttacked = true;
            // }
            
        }

    }

    public void MoveDown()
    {
        playerGO.transform.position = Vector3.MoveTowards(playerGO.transform.position, movePoint.position, moveSpeed*Time.deltaTime);

        if (Vector3.Distance(playerGO.transform.position, movePoint.position) <= .05f)
        {
            // if (Mathf.Abs(Input.GetAxisRaw("Horizontal")) == 1f)
            // {
            //     if (!Physics2D.OverlapCircle(movePoint.position + new Vector3(Input.GetAxisRaw("Horizontal"), 0f, 0f), .2f, obstacle))
            //     {
            //         movePoint.position += new Vector3(Input.GetAxisRaw("Horizontal"), 0f, 0f);
            //     }
            // }
            // if (Mathf.Abs(Input.GetAxisRaw("Vertical")) == 1f)
            // {
            if (!Physics2D.OverlapCircle(movePoint.position + new Vector3(0f, -1f, 0f), .2f, obstacle))
            {
                movePoint.position += new Vector3(0f, -1f, 0f);
                PHasMoved = true;
            }
            else
            {
                actionText.text = "Cannot Move There!";
                PHasMoved = false;
            }
            // if (Physics2D.OverlapCircle(movePoint.position + new Vector3(0f, -1f, 0f), .5f, enemy))
            // {
            //     Collider2D collider2D = Physics2D.OverlapCircle(movePoint.position + new Vector3(0f, -1f, 0f), .5f, enemy);
            //     EScript subEnemyUnit = collider2D.GetComponent<EScript>();
            //     isDead = subEnemyUnit.TakeDamage(playerUnit.damage);
            //     Debug.Log("Enemy Attacked!");
            //     if (isDead)
            //     {
            //         Debug.Log("Enemy Died");
            //         subEnemyUnit.OnDeath();
            //     }
            //     PHasAttacked = true;
            // }
            
        }

    }

    public void MoveLeft()
    {
        playerGO.transform.position = Vector3.MoveTowards(playerGO.transform.position, movePoint.position, moveSpeed*Time.deltaTime);

        if (Vector3.Distance(playerGO.transform.position, movePoint.position) <= .05f)
        {
            // if (Mathf.Abs(Input.GetAxisRaw("Horizontal")) == 1f)
            // {
            //     if (!Physics2D.OverlapCircle(movePoint.position + new Vector3(Input.GetAxisRaw("Horizontal"), 0f, 0f), .2f, obstacle))
            //     {
            //         movePoint.position += new Vector3(Input.GetAxisRaw("Horizontal"), 0f, 0f);
            //     }
            // }
            // if (Mathf.Abs(Input.GetAxisRaw("Vertical")) == 1f)
            // {
            if (!Physics2D.OverlapCircle(movePoint.position + new Vector3(-1f, 0f, 0f), .2f, obstacle))
            {
                movePoint.position += new Vector3(-1f, 0f, 0f);
                PHasMoved = true;
            }
            // else if (Physics2D.OverlapCircle(movePoint.position + new Vector3(-1f, 0f, 0f), .5f, enemy))
            // {
            //     Collider2D collider2D = Physics2D.OverlapCircle(movePoint.position + new Vector3(-1f, 0f, 0f), .5f, enemy);
            //     EScript subEnemyUnit = collider2D.GetComponent<EScript>();
            //     isDead = subEnemyUnit.TakeDamage(playerUnit.damage);
            //     Debug.Log("Enemy Attacked!");
            //     if (isDead)
            //     {
            //         Debug.Log("Enemy Died");
            //         subEnemyUnit.OnDeath();
            //     }
            //     PHasAttacked = true;
            // }
            else
            {
                actionText.text = "Cannot Move There!";
                PHasMoved = false;
            }
        }
    }

    public void MoveRight()
    {
        playerGO.transform.position = Vector3.MoveTowards(playerGO.transform.position, movePoint.position, moveSpeed*Time.deltaTime);

        if (Vector3.Distance(playerGO.transform.position, movePoint.position) <= .05f)
        {
            // if (Mathf.Abs(Input.GetAxisRaw("Horizontal")) == 1f)
            // {
            //     if (!Physics2D.OverlapCircle(movePoint.position + new Vector3(Input.GetAxisRaw("Horizontal"), 0f, 0f), .2f, obstacle))
            //     {
            //         movePoint.position += new Vector3(Input.GetAxisRaw("Horizontal"), 0f, 0f);
            //     }
            // }
            // if (Mathf.Abs(Input.GetAxisRaw("Vertical")) == 1f)
            // {
            if (!Physics2D.OverlapCircle(movePoint.position + new Vector3(1f, 0f, 0f), .2f, obstacle))
            {
                movePoint.position += new Vector3(1f, 0f, 0f);
                PHasMoved = true;
            }
            // else if (Physics2D.OverlapCircle(movePoint.position + new Vector3(1f, 0f, 0f), .5f, enemy))
            // {
            //     Collider2D collider2D = Physics2D.OverlapCircle(movePoint.position + new Vector3(1f, 0f, 0f), .5f, enemy);
            //     EScript subEnemyUnit = collider2D.GetComponent<EScript>();
            //     isDead = subEnemyUnit.TakeDamage(playerUnit.damage);
            //     Debug.Log("Enemy Attacked!");
            //     if (isDead)
            //     {
            //         Debug.Log("Enemy Died");
            //         subEnemyUnit.OnDeath();
            //     }
            //     PHasAttacked = true;
            // }
            else
            {
                actionText.text = "Cannot Move There!";
                PHasMoved = false;
            }
        }
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
        actionText.text = "Choose an option...";
        moveBtn.gameObject.SetActive(true);
        attackBtn.gameObject.SetActive(true);
        upBtn.gameObject.SetActive(false);
        dwnBtn.gameObject.SetActive(false);
        leftBtn.gameObject.SetActive(false);
        rightBtn.gameObject.SetActive(false);

    }

    public void OnAttackButton()
    {
        if (state != BattleState.PLAYERTURN)
        {
            return;
        }

        StartCoroutine(PlayerAttack());
    }

    public void OnMoveButton()
    {
        if (state != BattleState.PLAYERTURN)
        {
            return;
        }

        StartCoroutine(PlayerMove());
    }
}
