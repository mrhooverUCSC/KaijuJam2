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
    [SerializeField] GameObject startPosition;
    [SerializeField] GameObject endPosition;
    Vector3 targetPosition;
    bool crabTurn = false;
    public int pitfalls = 0;
    // Start is called before the first frame update
    void Start()
    {
        distanceText.text = distance + " meters until CRABOCALYPSE";
        healthText.text = "Crab Health: " + health.ToString();
    }

    // Update is called once per frame
    void Update()
    {
        if (crabTurn && targetPosition != null && Vector3.Distance(targetPosition, transform.position) > .5)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, .1f);
        }
        else if (crabTurn && targetPosition != null && Vector3.Distance(targetPosition, transform.position) <= .5)
        {
            crabTurn = false;
            gm.ReadyPlayerTurn();
        }
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
        distanceText.text = distance + " meters until CRABOCALYPSE";
        //Debug.Log(distance);
        float percentThere = (1.0f - distance / 1000.0f);
        //print(percentThere);
        targetPosition = startPosition.transform.position + new Vector3((endPosition.transform.position.x - startPosition.transform.position.x) * percentThere, (endPosition.transform.position.y - startPosition.transform.position.y) * percentThere, (endPosition.transform.position.z - startPosition.transform.position.z) * percentThere); //startPosition Vector3.Distance(startPosition.transform.position, endPosition.transform.position) / (1 - dis/1000);
        Debug.Log(targetPosition);
        if (distance <= 0)
        {
            Debug.Log("Game Lost");
            gm.Failure();
        }
        crabTurn = true;
    }

    public void AddPitfall()
    {
        pitfalls++;
        pitfallText.text = pitfalls + "Pitfalls";
    }

    public void EnemyTurn()
    {
        int temp = (int)Random.Range(0, 2);
        if (temp == 0)
        {
            if (pitfalls > 0)
            {
                TakeDamage(2);
                Move(15);
                pitfalls--;
                if (pitfalls > 0)
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
            gm.ReadyPlayerTurn();
        }
    }


}
