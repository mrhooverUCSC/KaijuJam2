using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [SerializeField] Button[] playerActionButtons;
    #region cannon variables
    [SerializeField] List<int> CannonFire;
    int currentCannon;
    int currentCannonFails;
    [SerializeField] GameObject cannonUI;
    #endregion
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void PlayerAction(string action)
    {
        //call the approrpriate action
        if(action == "FireCannons")
        {
            FireCannons();
        }
        //disable the player buttons until their next turn
        foreach(Button b in playerActionButtons)
        {
            b.interactable = false;
        }
    }
    void EnemyTurn()
    {
        ReadyPlayerTurn();
    }
    void ReadyPlayerTurn()
    {
        foreach (Button b in playerActionButtons)
        {
            b.interactable = true;
        }
    }
    #region FireCannons
    //4 cannons fire, in a random order. Press them in the correct order
    public void FireCannons()
    {
        StartCoroutine(fire());
    }
    public IEnumerator fire()
    {
        //shuffle the inputs
        Shuffle(CannonFire);
        currentCannon = 0;
        //turn them on in order
        yield return new WaitForSeconds(.5f);
        cannonUI.transform.GetChild(CannonFire[0]).gameObject.SetActive(true);
        yield return new WaitForSeconds(.4f);
        cannonUI.transform.GetChild(CannonFire[1]).gameObject.SetActive(true);
        yield return new WaitForSeconds(.4f);
        cannonUI.transform.GetChild(CannonFire[2]).gameObject.SetActive(true);
        yield return new WaitForSeconds(.4f);
        cannonUI.transform.GetChild(CannonFire[3]).gameObject.SetActive(true);
        yield return new WaitForSeconds(.4f);
        cannonUI.transform.GetChild(CannonFire[4]).gameObject.SetActive(true);
        //signal start
        yield return new WaitForSeconds(1f);
        cannonUI.transform.GetChild(5).gameObject.SetActive(true);
        //enable buttons
        for(int i = 0; i < 5; ++i)
        {
            cannonUI.transform.GetChild(i).GetComponent<Button>().interactable = true;
        }
    }
    public void FireCannonIndividual(int cannonNumber)
    {
        if(CannonFire[currentCannon] == cannonNumber)
        {
            cannonUI.transform.GetChild(6 + currentCannon).GetComponent<Image>().color = Color.green;
        }
        else
        {
            cannonUI.transform.GetChild(6 + currentCannon).GetComponent<Image>().color = Color.red;
            currentCannonFails++;
        }
        cannonUI.transform.GetChild(6 + currentCannon).gameObject.SetActive(true);
        currentCannon++;
        if(currentCannon == 5)
        {
            StartCoroutine(CheckCannons());
        }
    }
    IEnumerator CheckCannons()
    {
        cannonUI.transform.GetChild(5).GetComponent<Image>().color = Color.green;
        yield return new WaitForSeconds(.5f);
        //hide the info
        for(int i = 0; i < 11; ++i)
        {
            cannonUI.transform.GetChild(i).gameObject.SetActive(false);
        }
        //deal damage
        yield return new WaitForSeconds(.5f);
        EnemyTurn();
    }
    #endregion
    //list shuffler
    static void Shuffle<T>(List<T> ts)
    {
        var count = ts.Count;
        var last = count - 1;
        for (var i = 0; i < last; ++i)
        {
            var r = UnityEngine.Random.Range(i, count);
            var tmp = ts[i];
            ts[i] = ts[r];
            ts[r] = tmp;
        }
    }

}
