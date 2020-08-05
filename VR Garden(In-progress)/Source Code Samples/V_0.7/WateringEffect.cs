using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/*
 * The WateringEffect class is used to control the particle effect on the watering can. It's able to 
 * accept the particle system as a gameobject variable, but the script must be attached to the can itself 
 * to get the right euler angle for the pouring effect.
 * 
 */

public class WateringEffect : MonoBehaviour
{
    // droplets references the gameobject with the particle system attached
    public GameObject droplets;

    // the lower and upper tilt angles are used to set the euler boundaries for the pouring effect
    private int lowerTiltAngle = 20;
    private int upperTiltAngle = 110;


    /*
     * The Update method checks every frame to see if the can falls between the lowerTiltAngle and
     * upperTiltAngle. This allows a realistic pouring effect from the can based on motion control. 
     * 
     */
    void Update()
    {
        // euler angles should be sufficient for the pour effect. quaternions can be used if gimbal lock becomes 
        // an issue, but it shouldn't be a problem given the way a watering can is designed
        if (transform.rotation.eulerAngles.z >= lowerTiltAngle && transform.rotation.eulerAngles.z <= upperTiltAngle)
        {

            droplets.SetActive(true);

        } else
        {
 
            droplets.SetActive(false);

        }// end if
        
    } // end Update
    
} // end WateringEffect
