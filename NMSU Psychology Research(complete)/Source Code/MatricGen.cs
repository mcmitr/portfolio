using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.EventSystems;
using UnityEngine.UI;


//temp changes for demo: disabled timer, used a single matrix, set crit sentry to 1 for good measure(not current)
public class MatricGen : MonoBehaviour
{

    /*
        A list of what each public game object is used for: 
            mat1: holds the top normal matrix cells(for math problems)
            mat2: holds the bottom normal matrix cells(for codes)
            critMat1: holds the top critical matrix cells(for math problems)
            critMat2: holds the bottom critical matrix cells(for codes)
            box1: holds the top normal matrix(can access all components)
            box2: holds the bottom normal matrix(can access all components)
            critBox1: holds the top problem matrix(can access all components)
            critBox2: holds the bottom problem matrix(can access all components)
            firstArrow: arrow for error 0
            secArrow: arrow for error 1
            blackDrop: black screen that can be called up by researchers, pauses timer on critical matrices
            oxyC: accesses the caution symbol on the oxygen level bar, used to set as active or inactive
            tempC: accesses the caution symbol on the temperature level bar, used to set as active or inactive

    */
    public GameObject mat1, mat2, critMat1, critMat2, box1, box2, critBox1, critBox2, oxyC, tempC;
    public GameObject block, critError, complete, instruct, firstArrow, secArrow, blackDrop;
    /*
      A list of what each private game object is used for: 
          HoriList: used to access the horizontal lines of our matrice, used later for coloring 
          VertList: used to access the vertical lines of our matrice, used later for coloring 
          Border: used to access the border of our matrice, used later for coloring 
    */
    private GameObject HoriList, VertList, Border;// HoriList2, VertList2, Border2;

    private int sentry = 0;
   // private int critSentry = 0;
   // private int tracker = 0;
    //keeps track of which matrix we're on
    int counter = 1;
    int critCounter = 0;
    //tracks number of errors and time for results
    private int errorNum = 0;
    private float totalTime;

    string target;
    bool stopTime = false;
    bool done = false;

    //restartTime is a local bool just to keep track of whether the person is in a critical matrix or not, it will be set
    //initially to true. More explanation is given in the next if statement.
    bool restartTime = true;

    //we'll use this to stop critical errors if we aren't in regular matrix 3 just yet
    public bool pauseTime = false;

    string prev, curInst, errorInst;
    TextWriter writer;


    //stores our matrix data
    static FileInfo path = null;


    /*
     * What follows is a list of reader definitions:
     * 
     * reader: used to read text files for matrix data, fetches all the codes for the normal cells
     * reader2: used to read text files for matrix data, fetches all the codes for the critical cells
     * reader3: used to read text files for flavor text, such as messages between matrices and at the beginning and end
     * reader4: used to read text files for flavor text, critical matrices only
     * 
     * 
     * 
     * 
     * 
     * 
     * 
     */

    StreamReader reader, reader2, reader3, reader4, problems1, problems2, normInst, critInst;

    float timeLeft;
    Color ourColor, errorColor;

    void Start()
    {
        writer = new StreamWriter("results.txt", true);
        string participant = GameObject.Find("DataSentry").GetComponent<PartID>().ParticipantID;
        writer.Write("NEW SESSION \r\n------------------------\r\n");
        writer.Write("Participant: " + participant + "\r\n");

        //Debug.Log(Application.dataPath);
        string dataPath = Application.dataPath;

        //path = new FileInfo(dataPath + "/Assets/prob1");
        path = new FileInfo(dataPath + "/prob1.txt");
        problems1 = path.OpenText();

        path = new FileInfo(dataPath + "/MatrixData.txt");
        //path = new FileInfo("/Assets/MatrixData.txt");
        reader = path.OpenText();

        path = new FileInfo(dataPath + "/prob2.txt");
        //path = new FileInfo("/Assets/prob2.txt");
        problems2 = path.OpenText();

        path = new FileInfo(dataPath + "/normInst.txt");
        //path = new FileInfo("/Assets/prob2.txt");
        normInst = path.OpenText();

        path = new FileInfo(dataPath + "/critInst.txt");
        //path = new FileInfo("/Assets/prob2.txt");
        critInst = path.OpenText();

        path = new FileInfo(dataPath + "/CritMat.txt");
        //path = new FileInfo("/Assets/CritMat.txt");
        reader2 = path.OpenText();

        //the path to obtain flavor text
        path = new FileInfo(dataPath + "/flavorText.txt");
        //path = new FileInfo("/Assets/flavorText.txt");
        reader3 = path.OpenText();

        //the path to obtain flavor text
        path = new FileInfo(dataPath + "/CritFlavor.txt");
        //path = new FileInfo("/Assets/flavorText.txt");
        reader4 = path.OpenText();

        timeLeft = Random.Range(120, 150);

        curInst = normInst.ReadLine();
       //critSentry = 0;


        //set the first message for complete block from flavor text, and set it as active
        complete.transform.GetChild(0).GetComponent<Text>().text = reader3.ReadLine().ToString();
        complete.SetActive(true);

        ourColor = new Color(255, 0, 0);
        errorColor = new Color(0, 255, 0);

        NewMatrix();




    }

    private void Update()
    {

        //Here we check if we are still on matrice 2, where only one crit error is allowed. We may just replace the first bool with this later
        if (critCounter == 1 && counter == 2)
            pauseTime = true;
        else
            pauseTime = false;

        totalTime = totalTime + Time.deltaTime;
        if (stopTime != true && critCounter != 2 && !complete.activeSelf && pauseTime != true && counter != 1)
        {
            timeLeft -= Time.deltaTime;
            Debug.Log(timeLeft);
            if (timeLeft < 0)
            {
                manipulateUX();

                stopTime = true;

                complete.transform.GetChild(0).GetComponent<Text>().text = reader4.ReadLine().ToString();
                complete.SetActive(true);
                critProblem();
            }
        }


        if (Input.GetKey("escape"))
        {

            writer.Close();
            Application.Quit();
        }

        //a requested part of the script to be able to pause the system and cause a black screen to appear, which blocks out the interface
        if (Input.GetKeyDown(KeyCode.F1))
        {
            
            //this if statement will check to see if the blackDrop is currently active whenever F1 is pressed, if it isn't
            //then we'll set it as active and deal with the timers. If it is, we'll set it as inactive and deal with resetting 
            //timers if need be
            if (!blackDrop.activeSelf)
            {
                //restartTime is set to false if stopTime is currently true, because if stopTime is true 
                //then we need to make sure stopTime stays true so it doesn't trigger another crit matrix by restarting 
                //the timer after the black screen is dismissed

                if (stopTime == true)
                    restartTime = false;

                //we set the blackdrop to be true, so it appears and covers the screen
                blackDrop.SetActive(true);

                //stopTime is set to be active to make sure the participant doesn't come back to a different matrix
                stopTime = true;

            } else
            {
                //we set the blackdrop to be false, so it disappears
                blackDrop.SetActive(false);


                //we'll run a secondary check here to see if we still need to keep stopTime true. If not, we'll set it to false.
                //if restartTime has been set to false, we'll also reset it to true
                if (restartTime)
                    stopTime = false;
                else
                    restartTime = true;


            }//end if/else



        } //end if
    }//end Update()

    public void Highlight()
    {

        //Ok! So first we make a gameobject. This holds the current cell Gameobject 
        GameObject curr;

        //We take care of the Second cell first, since it relies on the name of the current selected cell from one
        //we use index 1 to grab the panel. We also set IP to the current input box. 
        curr = GameObject.Find(EventSystem.current.currentSelectedGameObject.name + "b");
        curr = curr.transform.GetChild(0).gameObject;

        //if it's active we set it inactive and vice versa
        if (curr.activeSelf)
        {
            //setting the other box as non-interactive
            curr.SetActive(false);
        }
        else
        {
            // setting the other box as interactive
            //IP.ActivateInputField();
            curr.SetActive(true);

        }
        //We take care of the first cell next. Grabbed from the eventsystem
        //we use index 1 to grab the panel
        curr = EventSystem.current.currentSelectedGameObject.transform.GetChild(0).gameObject;


        //if it's active we set it inactive and vice versa
        if (curr.activeSelf)
            curr.SetActive(false);
        else
            curr.SetActive(true);


    }

    public void checkCodes()
    {
        
        InputField IP = null;

        for (int q = 1; q <= 3; q++)
        {
            for (int e = 1; e <= 4; e++)
            {
                IP = null;
                target = q + "x" + e + "b";

                string correct = GameObject.Find(target).GetComponent<MatrixData>().Code;

                if (GameObject.Find(target).transform.GetChild(0).gameObject.activeSelf)
                    IP = GameObject.Find(target).GetComponentInChildren<InputField>();

                //Case 1: The cell is unselected, and it should have a code entered
                if (IP == null && correct != " ")
                {
                    generateError(1);
                    q = 10;
                    break;

                }
                //Case 2: The cell is selected, and it should have no code entered
                if (IP != null && correct == " ")
                {
                    generateError(1);
                    q = 10;
                    break;

                }

                //Case 3: The cell is selected, and the code is incorrect
                if (IP != null)
                {
                    if (IP.text != correct)
                    {

                        generateError(0);
                        q = 10;

                        done = false;
                        break;
                    }

                }


                //Case 4: This cell is correct, so we see if we have checked each row and column, if not we move on
                //otherwise we set done to true, which is used to see if we need a new matrix
                if (q == 3 && e == 4)
                {
                    done = true;
                }
            }



        }
        //if all goes well here is where we reset the matrices, this resets critical matrices
        if (stopTime && done)
        {
            //generate the results for this matrix
            genResults();

            manipulateUX();

            block.SetActive(false);
            for (int r = 1; r <= 3; r++)
            {
                for (int w = 1; w <= 4; w++)
                {
                    target = r + "x" + w;
                    GameObject.Find(target).transform.GetChild(0).gameObject.SetActive(false);
                }

            }

            for (int j = 1; j <= 3; j++)
            {
                for (int l = 1; l <= 4; l++)
                {
                    target = j + "x" + l + "b";
                    GameObject.Find(target).transform.GetChild(0).GetComponent<InputField>().text = " ";
                    GameObject.Find(target).transform.GetChild(0).gameObject.SetActive(false);
                }

            }

            box1.SetActive(true);
            box2.SetActive(true);
            critBox1.SetActive(false);
            critBox2.SetActive(false);

            //just a bit of cleanup to make sure the arrows are inactive
            firstArrow.SetActive(false);
            secArrow.SetActive(false);

            instruct.GetComponentInChildren<Text>().text = curInst;


            timeLeft = Random.Range(120, 150);
            stopTime = false;
            done = false;



        }
        else if (!stopTime && done) //this resets normal matrices
        {
            //generate the results for this matrix
            genResults();

            complete.transform.GetChild(0).GetComponent<Text>().text = reader3.ReadLine().ToString();
            complete.SetActive(true);
            block.SetActive(false);
            

            for (int r = 1; r <= 3; r++)
            {
                for (int w = 1; w <= 4; w++)
                {
                    target = r + "x" + w;
                    GameObject.Find(target).transform.GetChild(0).gameObject.SetActive(false);
                }

            }

            for (int j = 1; j <= 3; j++)
            {
                for (int l = 1; l <= 4; l++)
                {
                    target = j + "x" + l + "b";
                    GameObject.Find(target).transform.GetChild(0).GetComponent<InputField>().text = " ";
                    GameObject.Find(target).transform.GetChild(0).gameObject.SetActive(false);
                }

            }

            done = false;
            //first = false;
            //critSentry =+ tracker;
            curInst = normInst.ReadLine();
            //just a bit of cleanup to make sure the arrows are inactive
            firstArrow.SetActive(false);
            secArrow.SetActive(false);
            NewMatrix();
        }

        

    }



    public void NewMatrix()
    {
        if (sentry == 1)
        {
            genResults();
            return;
        }


        string text;

        instruct.transform.GetChild(0).GetComponent<Text>().text = curInst;

        //this for loop sets all codes to be blank
        for (int g = 1; g <= 3; g++)
        {
            for (int k = 1; k <= 4; k++)
            {
                target = g + "x" + k + "b";
                GameObject.Find(target).GetComponent<MatrixData>().Code = " ";
            }
        }



        text = reader.ReadLine();
        string[] splitter = text.Split(' ');

        //this set changes the colors for the top box(normal matrix)
        HoriList = box1.transform.Find("Horizontal Lines").gameObject;
        VertList = box1.transform.Find("Vertical Lines").gameObject;

        for (int c = 0; c <= 3; c++)
        {
            HoriList.transform.GetChild(c).GetComponent<Image>().color = ourColor;
        }

        for (int k = 0; k <= 3; k++)
        {
            VertList.transform.GetChild(k).GetComponent<Image>().color = ourColor;
        }

        box1.transform.Find("Border").GetComponent<Image>().color = ourColor;

        //this set changes the colors for the bottom box(normal matrix)
        HoriList = box2.transform.Find("Horizontal Lines").gameObject;
        VertList = box2.transform.Find("Vertical Lines").gameObject;
        for (int c = 0; c <= 3; c++)
        {
            HoriList.transform.GetChild(c).GetComponent<Image>().color = ourColor;
        }

        for (int k = 0; k <= 3; k++)
        {
            VertList.transform.GetChild(k).GetComponent<Image>().color = ourColor;
        }

        box2.transform.Find("Border").GetComponent<Image>().color = ourColor;

        //end of color changing




        while (!splitter[0].Contains("/"))
        {
            //Debug.Log(splitter[2]);
            target = splitter[0] + "x" + splitter[1] + "b";
            GameObject.Find(target).GetComponent<MatrixData>().Code = splitter[2];
            text = reader.ReadLine();
            splitter = text.Split(' ');

        }

        if (splitter[1].Contains("end"))
            sentry = 1;
        else
        {
            //we get the 3 ints after /, which is used as our R,G,B values in colorGen()
            ourColor = colorGen(int.Parse(splitter[1]), int.Parse(splitter[2]), int.Parse(splitter[3]));
           // tracker = int.Parse(splitter[4]);



        }

        text = problems1.ReadLine();
        splitter = text.Split(' ');

        while (!splitter[0].Contains("/"))
        {

            target = splitter[0] + "x" + splitter[1];
            GameObject.Find(target).GetComponent<Text>().text = splitter[2];
            text = problems1.ReadLine();
            splitter = text.Split(' ');

        }


    }


    public void generateError(int o)
    {
        block.SetActive(true);

        //block.transform.GetComponent<Image>().color = errorColor;
        if (o == 1)
        {
            errorNum++;
            firstArrow.SetActive(true);
            secArrow.SetActive(false);
            block.GetComponentInChildren<Text>().text = "Error in the number of selected problems. Please check matrix 1 for math errors.";
        }
        else
        {
            errorNum++;
            firstArrow.SetActive(false);
            secArrow.SetActive(true);
            block.GetComponentInChildren<Text>().text = "Error in one of the entered tool codes. Please make sure they have been entered properly in Matrix 2.";

        }
    }


    public void critProblem()
    {

        //just a bit of cleanup to make sure the arrows are inactive
        firstArrow.SetActive(false);
        secArrow.SetActive(false);
        block.SetActive(false);
        box1.SetActive(false);
        box2.SetActive(false);
        critBox1.SetActive(true);
        critBox2.SetActive(true);
        errorInst = critInst.ReadLine();

        //critSentry++;
        NewCritMatrix();

    }


    public void NewCritMatrix()
    {

        string text;

        instruct.transform.GetChild(0).GetComponent<Text>().text = errorInst;

        //this set changes the colors for the top box(normal matrix)
        HoriList = critBox1.transform.Find("Horizontal Lines").gameObject;
        VertList = critBox1.transform.Find("Vertical Lines").gameObject;

        for (int c = 0; c <= 3; c++)
        {
            HoriList.transform.GetChild(c).GetComponent<Image>().color = errorColor;
        }

        for (int k = 0; k <= 3; k++)
        {
            VertList.transform.GetChild(k).GetComponent<Image>().color = errorColor;
        }

        critBox1.transform.Find("Border").GetComponent<Image>().color = errorColor;

        //this set changes the colors for the bottom box(normal matrix)
        HoriList = critBox2.transform.Find("Horizontal Lines").gameObject;
        VertList = critBox2.transform.Find("Vertical Lines").gameObject;
        for (int c = 0; c <= 3; c++)
        {
            HoriList.transform.GetChild(c).GetComponent<Image>().color = errorColor;
        }

        for (int k = 0; k <= 3; k++)
        {
            VertList.transform.GetChild(k).GetComponent<Image>().color = errorColor;
        }

        critBox2.transform.Find("Border").GetComponent<Image>().color = errorColor;

        //end of color changing

        for (int g = 1; g <= 3; g++)
        {
            for (int k = 1; k <= 4; k++)
            {
                target = g + "x" + k + "b";
                GameObject.Find(target).GetComponent<MatrixData>().Code = " ";
            }
        }



        text = reader2.ReadLine();
        string[] splitter = text.Split(' ');

        while (!splitter[0].Contains("/"))
        {
            //Debug.Log(splitter[2]);
            target = splitter[0] + "x" + splitter[1] + "b";
            GameObject.Find(target).GetComponent<MatrixData>().Code = splitter[2];
            text = reader2.ReadLine();
            splitter = text.Split(' ');

        }

        if (splitter[1] != "end")
        {
            errorColor = colorGen(int.Parse(splitter[1]), int.Parse(splitter[2]), int.Parse(splitter[3]));
            Debug.Log(errorColor);
        }


        text = problems2.ReadLine();
        splitter = text.Split(' ');


        while (!splitter[0].Contains("/"))
        {
            //Debug.Log(splitter[2]);
            target = splitter[0] + "x" + splitter[1];
            GameObject.Find(target).GetComponent<Text>().text = splitter[2];
            text = problems2.ReadLine();
            splitter = text.Split(' ');

        }


    }


    public Color colorGen(int r, int g, int b)
    {
        Color GetColor;


        //We get a new color using the color32 constructor. I chose this over the color costructor to make it easy for 
        //the exterior files, as Color uses a 0-1 scale instead of 255 RGB
        GetColor = new Color32((byte)r, (byte)g, (byte)b, 255);


        return GetColor;
    }


    public void genResults()
    {


        //we need to have this keep track of Matrices, since we need to gen results after each one
        if(stopTime)
        {
            writer.Write("Critical Matrix: " + critCounter + "\r\n");
            critCounter++;

        } else
        {


            writer.Write("Normal Matrix: " + counter + "\r\n");
            counter++;


        }

        writer.Write("Total Number of Errors: " + errorNum + "\r\n");
        writer.Write("Time for Completion: " + (totalTime / 60) + " minutes \r\n");

        totalTime = 0;
        errorNum = 0;

    }

    
    public void manipulateUX()
    {

        if (stopTime)
        {
            this.GetComponent<BarManipulation>().temp = false;
            this.GetComponent<BarManipulation>().oxy = false;
            oxyC.SetActive(false);
            tempC.SetActive(false);

        } else
        {

            if (critCounter == 0)
            {
                this.GetComponent<BarManipulation>().oxy = true;
                oxyC.SetActive(true);
            }

            if (critCounter == 1)
            {
                this.GetComponent<BarManipulation>().temp = true;
                tempC.SetActive(true);
            }




        }//end if/else

    }
}

