using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarManipulation : MonoBehaviour
{
    //these bools will be used by the method to  
    public bool temp = false;
    public bool water = false;
    public bool oxy = false;

    public GameObject tempBar;
    public GameObject waterBar;
    public GameObject oxyBar;


    float mod = .0005f;

    // Update is called once per frame
    void Update()
    {
        TempManipulate();
        WaterManipulate();
        OxygenManipulate();

    }


    public void TempManipulate()
    {
        //check the local position of the object(since the bar is attached to a canvas we can't use worldspace)
        if (!temp && tempBar.transform.localPosition.y > -64)
            tempBar.transform.position = new Vector2(tempBar.transform.position.x, tempBar.transform.position.y - mod);

        if (temp && tempBar.transform.localPosition.y < -7)
            tempBar.transform.position = new Vector2(tempBar.transform.position.x, tempBar.transform.position.y + mod);


    }

    public void WaterManipulate()
    {
        //check the local position of the object(since the bar is attached to a canvas we can't use worldspace)
        if (water && waterBar.transform.localPosition.y > -104)
            waterBar.transform.position = new Vector2(waterBar.transform.position.x, waterBar.transform.position.y - mod);

        //we'll make the water bar rise a bit slower than the 
        if (!water && waterBar.transform.localPosition.y < -5)
            waterBar.transform.position = new Vector2(waterBar.transform.position.x, waterBar.transform.position.y + .0002f);


    }

    public void OxygenManipulate()
    {
        //check the local position of the object(since the bar is attached to a canvas we can't use worldspace)
        if (oxy && oxyBar.transform.localPosition.y > -99)
            oxyBar.transform.position = new Vector2(oxyBar.transform.position.x, oxyBar.transform.position.y - mod);

        //we'll make the water bar rise a bit slower than the 
        if (!oxy && oxyBar.transform.localPosition.y < -7)
            oxyBar.transform.position = new Vector2(oxyBar.transform.position.x, oxyBar.transform.position.y + .0002f);


    }
}
