using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/*
 * The PlantSize class is used to store the highest size a plant can grow to while 
 * being watered and how fast it grows. The script must be attached to the plant object itself, and allows
 * the maxSize and growthRate variables to be referenced as needed. 
 * 
 */

public class PlantSize : MonoBehaviour
{
    //the scalars are multiplied by the initial size of the plant object to get maxSize and growthRate
    private float desiredSizeScalar = 2.0f;
    private float desiredGrowthScalar = .02f;
    public float maxSize, growthRate;

    /*
     * The Start method sets the reference values from the plant gameobject to maxSize
     * and growthRate.
     * 
     */
    void Start()
    {
        // since the plant models being used are not uniform, a scalar 
        // will work better than a hard number multiplier. just getting 
        // the x scale is ok since the plant will grow evenly
        maxSize = (this.transform.localScale.x * desiredSizeScalar);

        growthRate = (this.transform.localScale.x * desiredGrowthScalar);

    }// end Start

}// end PlantSize
