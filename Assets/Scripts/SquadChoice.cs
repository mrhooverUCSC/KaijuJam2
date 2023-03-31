using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SquadChoice : MonoBehaviour {
    [SerializeField] public string typeOfChoice;
    [SerializeField] public int indexOfChoice;

    public bool ComparePairings() {
        if(gameObject == EventSystem.current.currentSelectedGameObject) {   // if this is the same object, stop here
            Debug.Log("this is the same");
        }

        return false;
        // public void ComparePairings(int sc_index, int so_index) {
        // if(ScenarioLineup[sc_index] == SolutionLineup[so_index]) { pairings.Add(true); }
        // else { pairings.Add(false); }
        // ScenarioLineup.RemoveAt(sc_index);
        // SolutionLineup.RemoveAt(so_index);
        // holdingButtonChoice = false;        // empties out the object list
    }

}
