using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Crab : MonoBehaviour
{
    [SerializeField] int health;
    [SerializeField] TextMeshProUGUI healthText;
    [SerializeField] int distance;
    [SerializeField] TextMeshProUGUI distanceText;
    // Start is called before the first frame update
    void Start()
    {
        distanceText.text = distance + " meters";
        healthText.text = health.ToString();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void TakeDamage(int dmg)
    {
        health -= dmg;
        healthText.text = health.ToString();
        if (health <= 0)
        {
            Debug.Log("Crab Defeated");
        }
    }

    public void Move(int dis)
    {
        distance -= dis;
        distanceText.text = distance + " meters";
        if(distance <= 0)
        {
            Debug.Log("Game Lost");
        }
    }

    public void EnemyTurn(GameManager gm)
    {
        int temp = (int)Random.Range(0, 2);
        Debug.Log(temp);
        if(temp == 0)
        {
            Move(100);
        }
        else
        {
            gm.DamagePlayer(5);
        }
        gm.ReadyPlayerTurn();
    }


}
