using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PartID : MonoBehaviour
{
    //Just a global Variable that will hold the participant ID. Separated out because I don't want the generation
    //methods present in the start screen
    public GameObject input;
    public string ParticipantID;

    private void Update()
    {
        if (input != null)
            ParticipantID = input.GetComponent<Text>().text;


    }
}
