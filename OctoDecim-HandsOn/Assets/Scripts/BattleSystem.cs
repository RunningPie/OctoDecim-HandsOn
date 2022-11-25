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
    [Header("States")]
    public BattleState b_state;
    public enum SpawnState { SPAWNING, WAITING, COUNTING };
    public SpawnState s_state = SpawnState.COUNTING;

    [Header("Prefabs")]
    public GameObject playerPrefab;
    public GameObject enemyPrefab;

    [Header("Spawning")]
    public Transform playerPosition;
    public Transform enemyPosition;
    public Transform[] spawnPoints;

    public GameObject playerGO;

    PScript playerUnit;
    EScript enemyUnit;

    // TextMeshProUGUI enemyName;
    // TextMeshProUGUI playerName;
    [Header("Dialogue Box")]
    public TextMeshProUGUI actionText;
    public Button attackBtn;
    public Button moveBtn;

    [Header("Move Arrows")]
    public Button upMBtn;
    public Button dwnMBtn;
    public Button leftMBtn;
    public Button rightMBtn;

    [Header("Attack Arrows")]
    public Button upABtn;
    public Button dwnABtn;
    public Button leftABtn;
    public Button rightABtn;

    [Header("Player Movement Variables")]
    public float moveSpeed = 5f;
    public Transform movePoint;
    public bool PHasMoved = false;
    public bool PHasAttacked = false;
    public int moveCounter = 0;

    [Header("Layer Masks")]
    public LayerMask obstacle;
    public LayerMask enemy;

    [Header("Attack Variables")]
    public bool isDead=false;
    // public float minHealth=0f;

    
    // public List<EScript> enemyList;

    [Header("Animators")]
    public Animator playerAnim;

    
    [System.Serializable]
    public class Wave
    {
        public string name;
        public Transform enemy;
        public int enemyCount;
        public float rate;


    }

    [Header("Waves Variables")]
    public Wave[] waves;
    private int nextWave = 0;

    public float timeBetweenWaves = 5f;
    public float waveCountDown;

    private float searchCountDown = 1f;

    public bool doneSpawning;

    // Start is called before the first frame update
    void Start()
    {
        waveCountDown = 0;

        if (spawnPoints.Length == 0)
        {
            Debug.Log("NO Spawn Point Referenced");
        }

        b_state = BattleState.START;      // Set BattleSystem state to START
        actionText.gameObject.SetActive(false);
        moveBtn.gameObject.SetActive(false);
        attackBtn.gameObject.SetActive(false);
        SetMoveButtons(false);
        SetAttButtons(false);

        playerGO = Instantiate(playerPrefab, playerPosition);
        //enemyGO = Instantiate(enemyPrefab, enemyPosition);

        movePoint = GameObject.FindGameObjectWithTag("PlayerMovePoint").GetComponent<Transform>();
        playerAnim = GameObject.FindGameObjectWithTag("Player").GetComponent<Animator>();
        

        movePoint.parent = null;

        StartCoroutine(SetupBattle());
        
    }

    void Update()
    {
        if (s_state == SpawnState.WAITING)
        {
            // CHECK IF ENEMIES ARE STILL ALIVE
            if (!EnemyIsAlive())
            {
                // BEGIN NEW WAVE
                WaveCompleted();
                return;

            }
            else
            {
                return;
            }
        }

        if (waveCountDown <= 0)
        {
            if (s_state != SpawnState.SPAWNING)
            {
                // Start Spawning Wave
                StartCoroutine(SpawnWave(waves[nextWave]));
            }
        }
        else
        {
            waveCountDown -= Time.deltaTime;
        }
    }

    void SetMoveButtons(bool _state)
    {
        upMBtn.gameObject.SetActive(_state);
        dwnMBtn.gameObject.SetActive(_state);
        leftMBtn.gameObject.SetActive(_state);
        rightMBtn.gameObject.SetActive(_state);
        
    }

    void SetAttButtons(bool _state)
    {
        upABtn.gameObject.SetActive(_state);
        dwnABtn.gameObject.SetActive(_state);
        leftABtn.gameObject.SetActive(_state);
        rightABtn.gameObject.SetActive(_state);
        
    }

    IEnumerator SetupBattle()
    {
        actionText.gameObject.SetActive(false);
        moveBtn.gameObject.SetActive(false);
        attackBtn.gameObject.SetActive(false);
        SetMoveButtons(false);
        SetAttButtons(false);
        
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

        b_state = BattleState.PLAYERTURN;
        PlayerTurn();
    }

    void WaveCompleted()
    {
        Debug.Log("Wave Completed");
        actionText.text = "Wave Completed";

        s_state = SpawnState.COUNTING;
        waveCountDown = timeBetweenWaves;

        if (nextWave + 1 > waves.Length - 1)
        {
            nextWave = 0;
            Debug.Log("Completed all waves. Looping...");
        }
        else
        {
            nextWave++;
        }

    }

    bool EnemyIsAlive()
    {
        searchCountDown -= Time.deltaTime;
        if (searchCountDown <= 0){
            searchCountDown = 1f;
            if (GameObject.FindGameObjectWithTag("Enemy") ==  null)
            {
                return false;
            }
        }
        return true;
    }

    IEnumerator SpawnWave(Wave _wave)
    {
        s_state = SpawnState.SPAWNING;

        // SPAWN WAVE
        Debug.Log("Spawning Wave: " + _wave.name);
        actionText.text = "Spawning Wave: " + _wave.name;
        for (int i = 0; i < _wave.enemyCount; i++)
        {
            SpawnEnemy(_wave.enemy);
            yield return new WaitForSeconds(0f);
        }

        
        s_state = SpawnState.WAITING;

        actionText.text = "Choose an option...";
        yield break;
    }

    void SpawnEnemy(Transform _enemy)
    {
        // SPAWN ENEMY
        Debug.Log("Spawning Enemy: "+_enemy.name);

        Transform _sp = spawnPoints[Random.Range(0, spawnPoints.Length)];
        Instantiate (_enemy, _sp.position, _sp.rotation);
        
    }



    IEnumerator PlayerAttack()
    {

        actionText.text = "Where do you want to attack to?";
        SetAttButtons(true);
        //actionText.text = "Looking for weakest nearby enemy...";
        moveBtn.gameObject.SetActive(false);
        attackBtn.gameObject.SetActive(false);
        
        
        yield return new WaitUntil(() => PHasAttacked);

        b_state = BattleState.ENEMYTURN;
        Debug.Log("Switching to Enemy");

        yield return new WaitForSeconds(0.15f);
        playerAnim.SetBool("attack", false);

        StartCoroutine(EnemyTurn());

        // DAMAGE ENEMY
        // Debug.Log("Hit Enemy");

        // CHECK IF ENEMY IS DEAD
        // if (isDead)
        // {
        //     // END BATTLE
        //     state = BattleState.WON;
        //     EndBattle();
        // }
        // else
        // {
        //     // ENEMY TURN
        //     state = BattleState.ENEMYTURN;
        //     StartCoroutine(EnemyTurn());
        // }

    }

    IEnumerator PlayerMove()
    {

        actionText.text = "Where do you want to move to?";
        moveBtn.gameObject.SetActive(false);
        attackBtn.gameObject.SetActive(false);
        SetMoveButtons(true);

        yield return new WaitUntil(() => PHasMoved);

        b_state = BattleState.ENEMYTURN;
        Debug.Log("Switching to Enemy");

        yield return new WaitForSeconds(0.25f);
        playerAnim.SetBool("move", false);
        //yield return new WaitForSeconds(1f);

        StartCoroutine(EnemyTurn());
    }

    IEnumerator AttackMove(string dir)
    {
        switch(dir) 
        {
        case "UP":
            playerGO.transform.position = Vector3.MoveTowards(playerGO.transform.position, movePoint.position, moveSpeed*Time.deltaTime);

            if (Vector3.Distance(playerGO.transform.position, movePoint.position) <= .05f)
            {
                if (Physics2D.OverlapCircle(movePoint.position + new Vector3(0f, 1f, 0f), .5f, enemy))
                {
                    if (playerGO.GetComponent<PScript>().Attack())
                    {
                        movePoint.position += new Vector3(0f, 1f, 0f);
                        
                        actionText.text = "Enemy Found!";
                        playerAnim.SetBool("attack", true);
                        PHasAttacked = true;

                        yield return new WaitForSeconds(0.25f);
                        movePoint.position += new Vector3(0f, -1f, 0f);
                    }
                    else
                    {
                        
                        actionText.text = "Not Enough Energy!";
                        yield return new WaitForSeconds(2.5f);
                        actionText.text = "Skipping turn...";
                        yield return new WaitForSeconds(2.5f);
                        PHasAttacked = true;
                    }
                }
                else
                {
                    actionText.text = "No Enemy There!";
                    PHasAttacked = false;
                }
                
            }
            break;

        case "DOWN":
            playerGO.transform.position = Vector3.MoveTowards(playerGO.transform.position, movePoint.position, moveSpeed*Time.deltaTime);

            if (Vector3.Distance(playerGO.transform.position, movePoint.position) <= .05f)
            {
                if (Physics2D.OverlapCircle(movePoint.position + new Vector3(0f, -1f, 0f), .5f, enemy))
                {
                    if (playerGO.GetComponent<PScript>().Attack())
                    {
                        movePoint.position += new Vector3(0f, -1f, 0f);
                        
                        actionText.text = "Enemy Found!";
                        playerAnim.SetBool("attack", true);
                        PHasAttacked = true;

                        yield return new WaitForSeconds(0.25f);
                        movePoint.position += new Vector3(0f, 1f, 0f);
                    }
                    else
                    {
                        
                        actionText.text = "Not Enough Energy!";
                        yield return new WaitForSeconds(2.5f);
                        actionText.text = "Skipping turn...";
                        yield return new WaitForSeconds(2.5f);
                        PHasAttacked = true;
                    }
                }
                else
                {
                    actionText.text = "No Enemy There!";
                    PHasAttacked = false;
                }
                
            }
            break;

        case "RIGHT":
            playerGO.transform.position = Vector3.MoveTowards(playerGO.transform.position, movePoint.position, moveSpeed*Time.deltaTime);

            if (Vector3.Distance(playerGO.transform.position, movePoint.position) <= .05f)
            {
                if (Physics2D.OverlapCircle(movePoint.position + new Vector3(1f, 0f, 0f), .5f, enemy))
                {
                    if (playerGO.GetComponent<PScript>().Attack())
                    {
                        movePoint.position += new Vector3(1f, 0f, 0f);
                        
                        actionText.text = "Enemy Found!";
                        playerAnim.SetBool("attack", true);
                        PHasAttacked = true;

                        yield return new WaitForSeconds(0.25f);
                        movePoint.position += new Vector3(-1f, 0f, 0f);
                    }
                    else
                    {
                        
                        actionText.text = "Not Enough Energy!";
                        yield return new WaitForSeconds(2.5f);
                        actionText.text = "Skipping turn...";
                        yield return new WaitForSeconds(2.5f);
                        PHasAttacked = true;
                    }
                }
                else
                {
                    actionText.text = "No Enemy There!";
                    PHasAttacked = false;
                }
                
            }
            break;
        
        case "LEFT":
            playerGO.transform.position = Vector3.MoveTowards(playerGO.transform.position, movePoint.position, moveSpeed*Time.deltaTime);

            if (Vector3.Distance(playerGO.transform.position, movePoint.position) <= .05f)
            {
                if (Physics2D.OverlapCircle(movePoint.position + new Vector3(-1f, 0f, 0f), .5f, enemy))
                {
                    if (playerGO.GetComponent<PScript>().Attack())
                    {
                        movePoint.position += new Vector3(-1f, 0f, 0f);
                        actionText.text = "Enemy Found!";
                        playerAnim.SetBool("attack", true);
                        PHasAttacked = true;

                        yield return new WaitForSeconds(0.25f);
                        movePoint.position += new Vector3(1f, 0f, 0f);
                    }
                    else
                    {
                        actionText.text = "Not Enough Energy!";
                        yield return new WaitForSeconds(2.5f);
                        actionText.text = "Skipping turn...";
                        yield return new WaitForSeconds(2.5f);
                        PHasAttacked = true;
                    }
                }
                else
                {
                    actionText.text = "No Enemy There!";
                    PHasAttacked = false;
                }
                
            }
            break;

        default:
            // code block
            break;
        }
    }

    public void AttUp()
    {
        StartCoroutine(AttackMove("UP"));
    }

    public void AttDown()
    {
        StartCoroutine(AttackMove("DOWN"));
    }

    public void AttLeft()
    {
        StartCoroutine(AttackMove("LEFT"));
    }

    public void AttRight()
    {
        StartCoroutine(AttackMove("RIGHT"));
    }

    public void MoveUp()
    {
        playerGO.transform.position = Vector3.MoveTowards(playerGO.transform.position, movePoint.position, moveSpeed*Time.deltaTime);

        if (Vector3.Distance(playerGO.transform.position, movePoint.position) <= .05f)
        {
            if ((!Physics2D.OverlapCircle(movePoint.position + new Vector3(0f, 1f, 0f), .2f, obstacle)) & (!Physics2D.OverlapCircle(movePoint.position + new Vector3(0f, 1f, 0f), .2f, enemy)))
            {
                movePoint.position += new Vector3(0f, 1f, 0f);
                playerAnim.SetBool("move", true);
                moveCounter ++;
                if (moveCounter >= 2)
                {
                    playerGO.GetComponent<PScript>().RecoverEnergy();
                    moveCounter = 0;
                }
                PHasMoved = true;
            }
            else
            {
                actionText.text = "Cannot Move There!";
                PHasMoved = false;
            }
            
        }

    }

    public void MoveDown()
    {
        playerGO.transform.position = Vector3.MoveTowards(playerGO.transform.position, movePoint.position, moveSpeed*Time.deltaTime);

        if (Vector3.Distance(playerGO.transform.position, movePoint.position) <= .05f)
        {
            if ((!Physics2D.OverlapCircle(movePoint.position + new Vector3(0f, -1f, 0f), .2f, obstacle)) & (!Physics2D.OverlapCircle(movePoint.position + new Vector3(0f, -1f, 0f), .2f, enemy)))
            {
                movePoint.position += new Vector3(0f, -1f, 0f);
                playerAnim.SetBool("move", true);
                moveCounter ++;
                if (moveCounter >= 2)
                {
                    playerGO.GetComponent<PScript>().RecoverEnergy();
                    moveCounter = 0;
                }
                PHasMoved = true;
            }
            else
            {
                actionText.text = "Cannot Move There!";
                PHasMoved = false;
            }
            
        }

    }

    public void MoveLeft()
    {
        playerGO.transform.position = Vector3.MoveTowards(playerGO.transform.position, movePoint.position, moveSpeed*Time.deltaTime);
        playerGO.transform.localScale = new Vector3(-1, 1, 1);

        if (Vector3.Distance(playerGO.transform.position, movePoint.position) <= .05f)
        {
            if ((!Physics2D.OverlapCircle(movePoint.position + new Vector3(-1f, 0f, 0f), .2f, obstacle)) & (!Physics2D.OverlapCircle(movePoint.position + new Vector3(-1f, 0f, 0f), .2f, enemy)))
            {
                movePoint.position += new Vector3(-1f, 0f, 0f);
                playerAnim.SetBool("move", true);
                moveCounter ++;
                if (moveCounter >= 2)
                {
                    playerGO.GetComponent<PScript>().RecoverEnergy();
                    moveCounter = 0;
                }
                PHasMoved = true;
            }
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
        playerGO.transform.localScale = new Vector3(1, 1, 1);
        
        if (Vector3.Distance(playerGO.transform.position, movePoint.position) <= .05f)
        {
            if ((!Physics2D.OverlapCircle(movePoint.position + new Vector3(1f, 0f, 0f), .2f, obstacle)) & (!Physics2D.OverlapCircle(movePoint.position + new Vector3(1f, 0f, 0f), .2f, enemy)))
            {
                movePoint.position += new Vector3(1f, 0f, 0f);
                playerAnim.SetBool("move", true);
                moveCounter ++;
                if (moveCounter >= 2)
                {
                    playerGO.GetComponent<PScript>().RecoverEnergy();
                    moveCounter = 0;
                }
                PHasMoved = true;
            }
            else
            {
                actionText.text = "Cannot Move There!";
                PHasMoved = false;
            }
        }
    }

    void PlayerTurn()
    {
        Debug.Log("Players Turn");
        actionText.gameObject.SetActive(true);
        actionText.text = "Choose an option...";
        moveBtn.gameObject.SetActive(true);
        attackBtn.gameObject.SetActive(true);

    }

    public void OnAttackButton()
    {
        if (b_state != BattleState.PLAYERTURN)
        {
            return;
        }

        StartCoroutine(PlayerAttack());
    }

    public void OnMoveButton()
    {
        if (b_state != BattleState.PLAYERTURN)
        {
            return;
        }

        StartCoroutine(PlayerMove());
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
        SetMoveButtons(false);
        SetAttButtons(false);

        yield return new WaitForSeconds(1f);

        // bool isDead = playerUnit.TakeDamage(enemyUnit.damage);
        // actionText.text = "Enemy attacked dealt " + enemyUnit.damage + " damage!";
        
        yield return new WaitForSeconds(1f);

        if (isDead)
        {
            b_state = BattleState.LOST;
            EndBattle();
        }
        else
        {
            b_state = BattleState.PLAYERTURN;
            PlayerTurn();
        }
    }

    void EndBattle()
    {
        if (b_state == BattleState.WON)
        {
            Debug.Log("You Won");
        }
        else if (b_state == BattleState.LOST)
        {
            Debug.Log("You Lost");
        }
    }
}
