using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartScript : MonoBehaviour
{
    public GameObject dataSentry;

    //a simple script for loading our main screen
    public void Open()
    {

        DontDestroyOnLoad(dataSentry);
        SceneManager.LoadScene("ExperScreen");
    }


    //Just in case they want to close the program



    private void Update()
    {
        if (Input.GetKey("escape"))
        {
            Application.Quit();
        }
    }
   
}
