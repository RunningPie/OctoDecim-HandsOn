using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;   // UI Library
using TMPro;        // Text Mesh Pro Library

public class EScript : MonoBehaviour
{
    public string unitName;

    public int  damage;

    public int maxHP;
    public int currentHP;

    public static List<EScript> enemyList = new List<EScript>();

    public Transform EMovePoint;
    public Animator anim;
    public GameObject actionTextGO;
    public TextMeshProUGUI actionText;
    public LayerMask player;
    public float moveSpeed = 5f;

    public static List<EScript> GetEnemyList()
    {
        return enemyList;
    }

    private void Start()
    {
        enemyList.Add(this);
        EMovePoint.parent = null;
        actionTextGO = GameObject.Find("ActionText");
        actionText = actionTextGO.GetComponent<TextMeshProUGUI>();
    }

    public void OnDeath()
    {
        gameObject.SetActive(false);
        enemyList.Remove(this);
    }

    public bool TakeDamage(int dmg)
    {
        currentHP -= dmg;

        if (currentHP <= 0)
        {
            OnDeath();
            return true;
        }
        else
        {
            return false;
        }
    }

    public void Heal(int amount)
    {
        currentHP += amount;
        if (currentHP > maxHP)
        {
            currentHP = maxHP;
        }
    }

    IEnumerator findPlayer()
    {
        transform.position = Vector3.MoveTowards(transform.position, EMovePoint.position, moveSpeed*Time.deltaTime);

            if (Vector3.Distance(transform.position, EMovePoint.position) <= .05f)
            {
                if (Physics2D.OverlapCircle(EMovePoint.position + new Vector3(0f, 1f, 0f), .5f, player))
                {
                    EMovePoint.position += new Vector3(0f, 1f, 0f);
                        
                    actionText.text = "Enemy is Attacking!";
                    // anim.SetBool("attack", true);
                    // PHasAttacked = true;

                    yield return new WaitForSeconds(0.25f);
                    EMovePoint.position += new Vector3(0f, -1f, 0f);
                    // }
                    // else
                    // {
                        
                    //     actionText.text = "Not Enough Energy!";
                    //     yield return new WaitForSeconds(2.5f);
                    //     actionText.text = "Skipping turn...";
                    //     yield return new WaitForSeconds(2.5f);
                    //     // PHasAttacked = true;
                    // }
                }
                // else
                // {
                //     actionText.text = "No Enemy There!";
                //     PHasAttacked = false;
                // }
                
            }
    }
}
