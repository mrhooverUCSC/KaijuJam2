using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
using System;

public class GameManager : MonoBehaviour
{
    [SerializeField] Button[] playerActionButtons;
    [SerializeField] Crab crab;
    [SerializeField] int playerHealth = 50;
    [SerializeField] TextMeshProUGUI playerHealthText;
    [SerializeField] GameObject failureInterface;
    [SerializeField] GameObject successInterface;
    //unused game state
    public enum GameState
    {
        GO,
        END
    }
    GameState currentGameState = GameState.GO;

    //cannon variables
    List<int> CannonFire = new List<int> { 0,1,2,3,4 };
    int currentCannon;
    int currentCannonFails;
    [SerializeField] GameObject cannonUI;
    int maxCannonDamage = 5;

    //pitfall variables
    //a big 2d grid of dirt/stone, have to blow a continuous vertical path through
    //int[,]: 0 is dirt, 1 is stone, negative is empty
    int[,] terrain;
    [SerializeField] GameObject pitFallUI;
    [SerializeField] GameObject[] buttonGrid;
    int bombs;

    [Header("Medic Variables")]     // medic variables
    [SerializeField] List<int> sce_Lineup;
    [SerializeField] List<int> sol_Lineup;
    [SerializeField] List<MedicSquadTypes> ScenarioOrder;
    [SerializeField] List<MedicSquadTypes> SolutionOrder;
    [SerializeField] GameObject medicUI;
    // [SerializeField] List<bool> pairings;
    [SerializeField] public Dictionary<MedicSquadTypes, GameObject> button_Pairing;
    public int MedicSquadCorrectPairings;   // stores # of pairings player got correct
    public int MedicSquadAmountPaired;  // stores # of pairings player has done
    public int MedicSquadNumberOfPairings;  // stores overall # of pairings total
    public enum MedicSquadTypes {
        FRACTURES = -1,
        BURNS = -2,
        DEAD = -3,
        OUTOFAMMO = -4,
        // ^ SCENARIOS ARE ABOVE (OUTPUT), v SOLUTIONS ARE BELOW (INPUT)
        PLASTER = 1,
        SPLINTS = 2,
        REINFORCEMENTS = 3,
        RESUPPLY = 4
    }
    public int ms_ButtonInput;   // stores Medic Squad button input of game object

    // Start is called before the first frame update
    void Start() {
        crab.AttachToCrab(this);
        button_Pairing = new Dictionary<MedicSquadTypes, GameObject>();
        ScenarioOrder.Clear();
        SolutionOrder.Clear();
        ms_ButtonInput = 0;
    }

    // Update is called once per frame
    void Update() {
        
    }
    
    public void PlayerAction(string action) //connected to the buttons on the main play screen, on the player's turn
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

        //start the timer, which calls the exit function
        if (action == "FireCannons")
        {
            StartCoroutine(PlayerActionTimer( CheckCannons() ));
        }
        else if (action == "MedicSquad")
        {
            StartCoroutine(PlayerActionTimer( MedicClose() ));
        }
        else if (action == "BlowPitfall")
        {
            StartCoroutine(PlayerActionTimer( CheckHole() ));
        }

    }
    public IEnumerator PlayerActionTimer(IEnumerator end)
    {
        //Create the timer, click it down
        yield return new WaitForSecondsRealtime(8f);
        //if the minigame hasn't already ended, shut it down
        StartCoroutine(end);
    }
    public void ReadyPlayerTurn() //called when the enemy's turn ends to prepare for the player's turn
    {
        foreach (Button b in playerActionButtons)
        {
            b.interactable = true;
        }
    }
    public void DamagePlayer(int dmg) //reduces player's health and displays the change
    {
        playerHealth -= dmg;
        if(playerHealth >= 50)
        {
            playerHealth = 50;
        }
        playerHealthText.text = "Player Health: " + playerHealth.ToString();
        if(playerHealth <= 0)
        {
            Debug.Log("player died");
            Failure();
        }
    }

    #region Endings
    public void Failure()
    {
        //disable the player buttons
        foreach (Button b in playerActionButtons)
        {
            b.interactable = false;
        }
        failureInterface.SetActive(true);
    }
    public void Success()
    {
        //disable the player buttons
        foreach (Button b in playerActionButtons)
        {
            b.interactable = false;
        }
        successInterface.SetActive(true);
    }
    public void LoadTitle()
    {
        TitleManager.Instance.EnterTitle();
    }
    #endregion

    #region FireCannons
    //4 cannons fire, in a random order. Press them in the correct order
    private void FireCannons()
    {
        StartCoroutine(fire());
    }
    private IEnumerator fire()
    {
        CinematicCameraManager.instance.StartPlayerFocus(6f); //TALK TO MATT, THIS IS TEMP
        //shuffle the inputs
        Shuffle(CannonFire);
        currentCannon = 0;
        currentCannonFails = 0;
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
        crab.TakeDamage(maxCannonDamage - currentCannonFails);
        yield return new WaitForSeconds(.5f);
        crab.EnemyTurn();
        //CinematicCameraManager.instance.ChangeCameraMode(CinematicCameraManager.CameraMode.DYNAMIC);
        //CinematicCameraManager.instance.Reset();
    }
    #endregion

    #region MedicSquad
    // i am sorry for fucking up this code -Ivan
    public void MedicSquad() {
        button_Pairing.Clear();
        MedicSquadCorrectPairings = 0;
        MedicSquadAmountPaired = 0;
        MedicOrganize();
    }

    public void MedicOrganize() {
        // changes the order of the input and output buttons and assigning them the positions
        Shuffle(sce_Lineup);
        Shuffle(sol_Lineup);

        // 0: scenarios, 1: solutions, 2: scenario positions, 3: solution positions
        for(int i = 0; i < sce_Lineup.Count; i++) {
            int sce_index = sce_Lineup[i];  // stores scenario index for positioning sake
            int sol_index = sol_Lineup[i];  // stores solution index for positioning sake

            int sce_val = (sce_Lineup[i] + 1) * (-1);    // stores value assigned to MST
            int sol_val = sol_Lineup[i] + 1;

            medicUI.transform.GetChild(1).GetChild(sce_index).gameObject.SetActive(true);
            medicUI.transform.GetChild(1).GetChild(sce_index).GetComponent<Button>().interactable = true;
            medicUI.transform.GetChild(1).GetChild(sce_index).position = medicUI.transform.GetChild(3).GetChild(i).position;
            button_Pairing.Add((MedicSquadTypes)sce_val, medicUI.transform.GetChild(1).GetChild(sce_index).gameObject);

            medicUI.transform.GetChild(2).GetChild(sol_index).gameObject.SetActive(true);
            medicUI.transform.GetChild(2).GetChild(sol_index).GetComponent<Button>().interactable = true;
            medicUI.transform.GetChild(2).GetChild(sol_index).position = medicUI.transform.GetChild(4).GetChild(i).position;
            button_Pairing.Add((MedicSquadTypes)sol_val, medicUI.transform.GetChild(2).GetChild(sol_index).gameObject);
            
        }

        medicUI.transform.gameObject.SetActive(true);
        MedicSquadNumberOfPairings = sce_Lineup.Count;   // stores how many pairs there are

    }

    public IEnumerator MedicClose() {
        yield return new WaitForSecondsRealtime(.6f);
        //give feedback on how well the player did
        Debug.Log("Received a score of: " + MedicSquadCorrectPairings + "/" + MedicSquadNumberOfPairings);
        Debug.Log(MedicSquadCorrectPairings / MedicSquadNumberOfPairings * -5);
        DamagePlayer(MedicSquadCorrectPairings / MedicSquadNumberOfPairings * -5);
        medicUI.transform.gameObject.SetActive(false);
        crab.EnemyTurn();
    }

    // Medic Squad Input Button Function (value will be )
    public void GrabInput(int inp) {
        Debug.Log("Clicked on " + inp);
        ms_ButtonInput = inp;
    }

    // Medic Squad Output Button Function
    public void ComparePairings(int ms_ButtonOutput) {
        Debug.Log("Clicked on " + ms_ButtonOutput);
        // if the minigame has not received an input from a player
        if(ms_ButtonInput <= 0 || ms_ButtonInput > 4) { 
            Debug.Log("nothing has been stored yet");
            return;
        }

        if(ms_ButtonInput == (ms_ButtonOutput * (-1))) {
            Debug.Log("These are the same");
            MedicSquadCorrectPairings++;
        }
        else {
            Debug.Log("WRONG: " + ms_ButtonOutput + " and " + ms_ButtonInput + " arent the samee");
        }

        button_Pairing[(MedicSquadTypes)ms_ButtonInput].transform.GetComponent<Button>().interactable = false;
        button_Pairing[(MedicSquadTypes)ms_ButtonOutput].transform.GetComponent<Button>().interactable = false;

        ms_ButtonInput = 0;
        MedicSquadAmountPaired++;

        // Moves onto enemy turn once it reaches the last button pairing attempt
        if(MedicSquadAmountPaired >= MedicSquadNumberOfPairings) {
            StartCoroutine(MedicClose());
        }

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
            Vector2Int temp = new Vector2Int(UnityEngine.Random.Range(1, 7), UnityEngine.Random.Range(1, 7));
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
                    catch{}
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
        //Debug.Log(temp);

        List<Vector2Int> visited = new List<Vector2Int>();
        FloodExplode(temp, 3, visited);
        if(bombs == 0)
        {
            StartCoroutine(CheckHole());
        }
    }

    private void FloodExplode(Vector2Int pos, int strength, List<Vector2Int> visisted)
    {
        try
        {
            Debug.Log(pos.ToString() + " " + strength + " " + terrain[pos.x, pos.y].ToString());
        }catch{}
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

    private IEnumerator CheckHole()
    {
        yield return new WaitForSecondsRealtime(.5f);
        //highlight the best route? to show it was successful?
        if (HelperCheckHole())
        {
            //Debug.Log("path found");
            crab.AddPitfall();
        }
        pitFallUI.SetActive(false);
        crab.EnemyTurn();
    }

    private bool HelperCheckHole()
    {
        for (int i = 0; i < 8; ++i)
        {
            if (terrain[0, i] == -1)
            {
                //Debug.Log("empty on top row found: " + i);
                List<Vector2Int> visited = new List<Vector2Int>();
                if (FloodCheckHole(visited, new Vector2Int(0, i)))
                {
                    return true;
                }
            }
        }
        return false;
    }
    private bool FloodCheckHole(List<Vector2Int> visited, Vector2Int pos)
    {
        //if on the bottom row, return true: made it top to bottom
        if(pos.x == 7 && terrain[pos.x, pos.y] < 0)
        {
            return true;
        }
        //if out of bounds, return false
        if(pos.x < 0 || pos.x > 7 || pos.y < 0 || pos.y > 7)
        {
            return false;
        }
        //if not empty, return false
        if(terrain[pos.x, pos.y] >= 0)
        {
            return false;
        }
        //if already been here, return false
        if(checkForDuplicate(pos, visited))
        {
            return false;
        }

        visited.Add(pos);
        //if a true is found anywhere below, return true
        if(FloodCheckHole(visited, new Vector2Int(pos.x - 1, pos.y)) || FloodCheckHole(visited, new Vector2Int(pos.x + 1, pos.y)) || FloodCheckHole(visited, new Vector2Int(pos.x, pos.y - 1)) || FloodCheckHole(visited, new Vector2Int(pos.x, pos.y + 1))){
            return true;
        }

        //if not any of those cases, return false
        return false;
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
