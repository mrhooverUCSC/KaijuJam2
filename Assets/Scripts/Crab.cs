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
    [SerializeField] TextMeshProUGUI pitfallText;
    [SerializeField] GameManager gm;
    public int pitfalls = 0;
    // Start is called before the first frame update
    void Start()
    {
        distanceText.text = distance + " meters until CRABOCALYPSE!!";
        healthText.text = "Crab Health: " + health.ToString();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AttachToCrab(GameManager g)
    {
        gm = g;
    }
    public void TakeDamage(int dmg)
    {
        health -= dmg;
        healthText.text = "Crab Health: " + health.ToString();
        if (health <= 0)
        {
            Debug.Log("Crab Defeated");
            gm.Success();
        }
    }

    public void Move(int dis)
    {
        distance -= dis;
        distanceText.text = distance + " meters until CRABOCALYPSE!!";
        if(distance <= 0)
        {
            Debug.Log("Game Lost");
            gm.Failure();
        }
    }

    public void AddPitfall()
    {
        pitfalls++;
        pitfallText.text = pitfalls + "Pitfalls";
    }

    public void EnemyTurn()
    {
        int temp = (int)Random.Range(0, 2);
        Debug.Log(temp);
        if(temp == 0)
        {
            if(pitfalls > 0)
            {
                TakeDamage(2);
                Move(15);
                pitfalls--;
                if(pitfalls > 0)
                {
                    pitfallText.text = pitfalls + "Pitfalls";
                }
                else
                {
                    pitfallText.text = "";
                }
            }
            else
            {
                Move(100);
            }
        }
        else
        {
            gm.DamagePlayer(5);
        }
        gm.ReadyPlayerTurn();
    }


}
