using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class PScript : MonoBehaviour
{
    public string unitName;

    public int  damage;

    public int maxHP;
    public int currentHP;

    public float maxEnergy;
    public float currentEnergy;

    Image currentEnergyBar;
    Image totalEnergyBar;

    void Start()
    {
        currentEnergyBar = GameObject.FindGameObjectWithTag("currentEnergyBar").GetComponent<Image>();
        totalEnergyBar = GameObject.FindGameObjectWithTag("totalEnergyBar").GetComponent<Image>();
    }

    public bool TakeDamage(int dmg)
    {
        currentHP -= dmg;

        if (currentHP <= 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    void OnCollisionEnter2D(Collision2D _collision)
    {
        Debug.Log("Collision!");
        GameObject enemy = _collision.gameObject;
        if (enemy.CompareTag("Enemy"))
        {
            Debug.Log("Attack Enemy");
            enemy.GetComponent<EScript>().TakeDamage(10);
        }
    }

    public bool Attack()
    {
        if (currentEnergy <= 0)
        {
            currentEnergy = 0;
        }
        else
        {
            currentEnergy -= 1;
        }
        
        Debug.Log(currentEnergy / maxEnergy);
        currentEnergyBar.fillAmount = currentEnergy / maxEnergy;

        Debug.Log(currentEnergy);
        if (currentEnergy < 0)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    public bool RecoverEnergy()
    {
        if (currentEnergy >= maxEnergy)
        {
            currentEnergy = maxEnergy;
        }
        else
        {
            currentEnergy += 1;
        }
        Debug.Log(currentEnergy / maxEnergy);
        currentEnergyBar.fillAmount = currentEnergy / maxEnergy;

        Debug.Log(currentEnergy);
        if (currentEnergy <= 0)
        {
            return false;
        }
        else
        {
            return true;
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
