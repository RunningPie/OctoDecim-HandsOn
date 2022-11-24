using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EScript : MonoBehaviour
{
    public string unitName;

    public int  damage;

    public int maxHP;
    public int currentHP;

    public static List<EScript> enemyList = new List<EScript>();

    public static List<EScript> GetEnemyList()
    {
        return enemyList;
    }

    private void Awake()
    {
        enemyList.Add(this);
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
}
