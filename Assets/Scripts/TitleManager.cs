using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleManager : MonoBehaviour
{
    public static TitleManager Instance { get; private set; }
    
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    #region menu functions
    public void EnterLevel()
    {
        Debug.Log("entering combat");
        SceneManager.LoadScene("Combat Scene");
    }

    public void EnterTitle()
    {
        Debug.Log("entering combat");
        SceneManager.LoadScene("Title Scene");
    }
    #endregion
}
