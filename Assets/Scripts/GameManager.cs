using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class GameManager : MonoBehaviour
{
    [SerializeField] Button[] playerActionButtons;

    //cannon variables
    [SerializeField] List<int> CannonFire;
    int currentCannon;
    int currentCannonFails;
    [SerializeField] GameObject cannonUI;

    //pitfall variables
    //a big 2d grid of dirt/stone, have to blow a continuous vertical path through
    //int[,]: 0 is dirt, 1 is stone, negative is empty
    int[,] terrain;
    [SerializeField] GameObject pitFallUI;
    [SerializeField] GameObject[] buttonGrid;
    int bombs;

    // medic variables
    [SerializeField] List<int> ScenarioLineup;
    [SerializeField] List<int> SolutionLineup;
    [SerializeField] List<string> ScenarioOrder;
    [SerializeField] List<string> SolutionOrder;
    [SerializeField] GameObject medicUI;
    [SerializeField] List<bool> pairings;
    [SerializeField] bool holdingButtonChoice;
    [SerializeField] string choiceOpt;

    // Start is called before the first frame update
    void Start() {
        pairings.Clear();
        holdingButtonChoice = false;
        ScenarioOrder.Clear();
        SolutionOrder.Clear();
        choiceOpt = "";
    }

    // Update is called once per frame
    void Update() {
        
    }
    public void PlayerAction(string action)
    {
        //call the approrpriate action
        if(action == "FireCannons") {
            FireCannons();
        } else if(action == "MedicSquad") {
            MedicSquad();
        }
        else if(action == "BlowPitfall")
        {
            BlowPitfall();
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
    private void FireCannons()
    {
        StartCoroutine(fire());
    }
    private IEnumerator fire()
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
    private IEnumerator CheckCannons()
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

    #region MedicSquad
    // i am sorry for fucking up this code -Ivan
    public void MedicSquad() {
        StartCoroutine(organize());
    }

    public IEnumerator organize() {
        Shuffle(ScenarioLineup);
        Shuffle(SolutionLineup);

        // 0: scenarios, 1: solutions, 2: scenario positions, 3: solution positions
        for(int i = 0; i < ScenarioLineup.Count; i++) {
            medicUI.transform.GetChild(0).GetChild(ScenarioLineup[i]).position = medicUI.transform.GetChild(2).GetChild(i).position;
            ScenarioOrder.Add(medicUI.transform.GetChild(0).GetChild(ScenarioLineup[i]).name);
            // medicUI.transform.GetChild(0).GetChild(ScenarioLineup[i]).GetComponent<SquadChoice>().indexOfChoice = i;

            medicUI.transform.GetChild(1).GetChild(SolutionLineup[i]).position = medicUI.transform.GetChild(3).GetChild(i).position;
            SolutionOrder.Add(medicUI.transform.GetChild(1).GetChild(SolutionLineup[i]).name);
            // medicUI.transform.GetChild(1).GetChild(SolutionLineup[i]).GetComponent<SquadChoice>().indexOfChoice = i;
            
        }

        medicUI.transform.gameObject.SetActive(true);
        
        yield return new WaitForSeconds(20.0f);
        medicUI.transform.gameObject.SetActive(false);
        EnemyTurn();
    }

    // type = 0 -> scenario, type = 1 -> solution
    public void GrabButtonInput(string choice) {
        if(holdingButtonChoice) {
            if(ScenarioOrder.Contains(choice)) {    // it is a scenario obj
                if(ScenarioOrder.Contains(choiceOpt)) {     // if we are choosing a button that is in the same row
                    choiceOpt = choice;
                } else {    // both the one we are holding and the new one receiving are different types, continue on
                    ComparePairings(ScenarioOrder.IndexOf(choice), SolutionOrder.IndexOf(choiceOpt));
                }
            } else if(SolutionOrder.Contains(choice)) {     // it is a solution obj
                if(SolutionOrder.Contains(choiceOpt)) {     // if we are choosing button that's in same row, swap
                    choiceOpt = choice;
                } else {        // both choices are different, we compare
                    ComparePairings(ScenarioOrder.IndexOf(choiceOpt), SolutionOrder.IndexOf(choice));
                }

            } else {    // something messed up?
                Debug.Log("BAD ERROR??");
            }


        } else if(!holdingButtonChoice) {
            holdingButtonChoice = true;    // if they are not holding a choice yet, they are now :)
            choiceOpt = choice;
        }

    }

    public void ComparePairings(int sc_index, int so_index) {
        if(ScenarioLineup[sc_index] == SolutionLineup[so_index]) { pairings.Add(true); }
        else { pairings.Add(false); }
        ScenarioLineup.RemoveAt(sc_index);
        SolutionLineup.RemoveAt(so_index);
        holdingButtonChoice = false;        // empties out the object list
    }

    #endregion

    #region BlowPitfall
    private void BlowPitfall()
    {
        pitFallUI.SetActive(true);
        bombs = 3;
        terrain = new int[8,8];
        List<Vector2Int> stored = new List<Vector2Int>();
        //find 4 different positions
        while(stored.Count < 4)
        {
            Vector2Int temp = new Vector2Int(Random.Range(1, 7), Random.Range(1, 7));
            if(checkForDuplicate(temp, stored) == true)
            {
                continue;
            }
            stored.Add(temp);
        }
        //create a 3x3 rock in each position
        for(int i = 0; i < stored.Count; ++i)
        {
            for(int j = -1; j < 2; ++j)
            {
                for(int k = -1; k < 2; ++k)
                {
                    try
                    {
                        terrain[stored[i].x + j, stored[i].y + k] = 1;
                    }
                    catch
                    {

                    }
                }
            }
            //Debug.Log(stored[i].ToString());
        }
        //color button grid
        for(int i = 0; i < 8; ++i)
        {
            //string o = "";
            for(int j = 0; j < 8; ++j)
            {
                if(terrain[i, j] == 0)
                {
                    buttonGrid[i].transform.GetChild(j).GetComponent<Image>().color = new Color(193/255f, 106/255f, 19/255f);
                }
                else
                {
                    buttonGrid[i].transform.GetChild(j).GetComponent<Image>().color = Color.gray;
                }
                buttonGrid[i].transform.GetChild(j).name = i + "" + j;
                //o += terrain[i, j] + " ";
            }
            //Debug.Log(o);
        }
    }
    public void Explode()
    {
        Debug.Log(EventSystem.current.currentSelectedGameObject.name);
        bombs--;
        Vector2Int temp = new Vector2Int(EventSystem.current.currentSelectedGameObject.name[0] - '0', EventSystem.current.currentSelectedGameObject.name[1] - '0');
        Debug.Log(temp);

        List<Vector2Int> visited = new List<Vector2Int>();
        FloodExplode(temp, 3, visited);
    }

    private void FloodExplode(Vector2Int pos, int strength, List<Vector2Int> visisted)
    {
        try
        {
            Debug.Log(pos.ToString() + " " + strength + " " + terrain[pos.x, pos.y].ToString());
        }
        catch
        {

        }
        int str = strength;
        //if strength is 0 or negative, or pos outisde the map, return
        if (str < 1 || pos.x < 0 || pos.x > 7 || pos.y < 0 || pos.y > 7 || checkForDuplicate(pos, visisted))
        {
            return;
        }
        //if stone and can blow through, break
        else if (terrain[pos.x,pos.y] == 1 && str > 1)
        {
            Debug.Log("Stone to None");
            terrain[pos.x, pos.y] = -1;
            str--;
            buttonGrid[pos.x].transform.GetChild(pos.y).GetComponent<Image>().color = Color.white;
        }
        //if stone but strength is 1, make into dirt
        else if(terrain[pos.x,pos.y] == 1 && str == 1)
        {
            Debug.Log("Stone to Dirt");
            terrain[pos.x, pos.y] = 0;
            str--;
            buttonGrid[pos.x].transform.GetChild(pos.y).GetComponent<Image>().color = new Color(193 / 255f, 106 / 255f, 19 / 255f);
        }
        //if dirt and strength 1 or more, break
        else if (terrain[pos.x, pos.y] == 0 && str > 0)
        {
            Debug.Log("Dirt to None");
            terrain[pos.x, pos.y] = -1;
            buttonGrid[pos.x].transform.GetChild(pos.y).GetComponent<Image>().color = Color.white;
        }
        visisted.Add(pos);
        str--;
        //call down
        if(str > 0)
        {
            FloodExplode(new Vector2Int(pos.x - 1, pos.y), str, visisted);
            FloodExplode(new Vector2Int(pos.x + 1, pos.y), str, visisted);
            FloodExplode(new Vector2Int(pos.x, pos.y - 1), str, visisted);
            FloodExplode(new Vector2Int(pos.x, pos.y + 1), str, visisted);
        }
    }

    //make sure the rocks don't spawn on each other
    private bool checkForDuplicate(Vector2Int temp, List<Vector2Int> list)
    {
        foreach (Vector2Int v in list)
        {
            if(temp.x == v.x && temp.y == v.y)
            {
                return true;
            }
        }
        return false;
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
