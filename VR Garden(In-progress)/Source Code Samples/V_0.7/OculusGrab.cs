using System.Collections;

using System.Collections.Generic;

using UnityEngine;

/*
 * The OculusGrab class allows the virtual right hand to grasp objects. Note that the 
 * gameobject must have both a rigibody and a collider to be grasped. Attach this script
 * to the CustomHandRight object.
 * 
 */
public class OculusGrab : MonoBehaviour
{

    
    // CollidingObject temporarily holds objects that collided with the hand in case the 
    // player wants to pick them up. if picked up it's then stored in the 
    public GameObject CollidingObject;
    public GameObject ObjectInHand;

    /*
     * The OnTriggerEnter class checks to see if the collided item is a pot with a rigidbody,
     * and if so then it's temporarily stored in CollidingObject in case the player grips it.
     * 
     */

    public void OnTriggerEnter(Collider other)

    {

        if (other.gameObject.GetComponent<Rigidbody>() && other.gameObject.tag == "Pot")

        {
            CollidingObject = other.gameObject;

        }// end if

    }// end OnTriggerEnter

   /*
    * The OnTriggerExit class sets CollidingObject back to null if the hand is no longer
    * colliding with an object.
    * 
    */

    public void OnTriggerExit(Collider other) 

    {

        CollidingObject = null;

    }// end OnTriggerExit


    /*
     * The Update class checks for trigger button input from the Oculus controller. Once it exceeds a pressure of 0.2
     * an object is grabbed if able, or if it's less than 0.2 then any grabbed object is released.
     * 
     */

    void Update() 

    {

        if (Input.GetAxis("Oculus_CrossPlatform_PrimaryHandTrigger") > 0.2f && CollidingObject)

        {

            GrabObject();

        }// end if

        if (Input.GetAxis("Oculus_CrossPlatform_PrimaryHandTrigger") < 0.2f && ObjectInHand)

        {

            ReleaseObject();

        }// end if

    }// end Update


    /*
     * The GrabObject method sets a gameobject to be the child of the virtual hand. That causes it 
     * to move with the controller and be movable by the player.
     * 
     */

    public void GrabObject() 

    {

        ObjectInHand = CollidingObject;

        ObjectInHand.transform.SetParent(this.transform);

        // the object is set to kinematic so that it doesn't get knocked away while grabbed
        ObjectInHand.GetComponent<Rigidbody>().isKinematic = true;

    }// end GrabObject

    /*
     * The ReleaseObject class removes the parent-child relationship between the held gameobject
     * and the virtual hand. That causes it to drop from the players grasp and stop following the 
     * controller.
     * 
     */

    private void ReleaseObject() 

    {

        ObjectInHand.transform.SetParent(null);

        ObjectInHand = null;

        //the object can no longer be kinematic, so that it interacts with other gameobjects again
        ObjectInHand.GetComponent<Rigidbody>().isKinematic = false;

    }// end ReleaseObject

}// end OculusGrab

