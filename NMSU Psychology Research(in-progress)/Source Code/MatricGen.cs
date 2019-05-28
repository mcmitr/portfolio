using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.EventSystems;
using UnityEngine.UI;



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


    */
    public GameObject mat1, mat2, critMat1, critMat2, box1, box2, critBox1, critBox2;
    public GameObject block, critError, complete, instruct;
    /*
      A list of what each private game object is used for: 
          HoriList: used to access the horizontal lines of our matrice, used later for coloring 
          VertList: used to access the vertical lines of our matrice, used later for coloring 
          Border: used to access the border of our matrice, used later for coloring 
    */
    private GameObject HoriList, VertList, Border;// HoriList2, VertList2, Border2;

    private int sentry = 0;
    private int critSentry = 0;
    private int tracker = 0;
    string target;
    bool stopTime = false;
    bool done = false;
    bool first = true;

    string prev, curInst, errorInst;
    

    //stores our matrix data
    static FileInfo path = null;
    StreamReader reader, reader2, problems1, problems2, normInst, critInst= null;

    float timeLeft;
    Color ourColor,errorColor;

    void Start()
    {

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

        timeLeft = Random.Range(60, 90);

        curInst = normInst.ReadLine();
        critSentry = 0;

        ourColor = new Color(255, 0, 0);
        errorColor = new Color(0,255,0);

        NewMatrix();

        

        
    }

    private void Update()
    {
        if (stopTime != true && first != true && critSentry != 0) { 
                timeLeft -= Time.deltaTime;
            Debug.Log(timeLeft);
            if (timeLeft < 0)
            {
                stopTime = true;

                
                critProblem();
            }
        }
        //Debug.Log(timeLeft);

        if (Input.GetKey("escape"))
        {
            Application.Quit();
        }
    }

        public void Highlight ()
    {

        //Ok! So first we make a gameobject. This holds the current cell Gameobject 
        GameObject curr;

        //We take care of the Second cell first, since it relies on the name of the current selected cell from one
        //we use index 1 to grab the panel. We also set IP to the current input box. 
        curr = GameObject.Find(EventSystem.current.currentSelectedGameObject.name + "b");
        //IP = curr.GetComponent<UnityEngine.UI.InputField>();
        curr = curr.transform.GetChild(0).gameObject;

        //if it's active we set it inactive and vice versa
        if (curr.activeSelf)
        {
            //setting the other box as non-interactive
            //IP.DeactivateInputField();

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

        for (int q = 1; q <= 4; q++)
        {
            for (int e = 1; e <= 4; e++)
            {
                IP = null;
                target = q + "x" + e + "b";
                
                string correct = GameObject.Find(target).GetComponent<MatrixData>().Code;
                if (GameObject.Find(target).transform.GetChild(0).gameObject.activeSelf)
                    IP = GameObject.Find(target).GetComponentInChildren<InputField>();

                if (IP == null && correct != " ")
                {
                    generateError(1);
                    q = 10;
                    break;

                }


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

                Debug.Log(q + " " + e);

                if (q == 4 && e == 4)
                {
                    done = true;
                }
            }


            
        }
        //if all goes well here is where we reset the matrices
        if (stopTime && done)
        {
            complete.SetActive(true);
            block.SetActive(false);
            for (int r = 1; r <= 4; r++)
            {
                for (int w = 1; w <= 4; w++)
                {
                    target = r + "x" + w;
                    GameObject.Find(target).transform.GetChild(0).gameObject.SetActive(false);
                }

            }

            for (int j = 1; j <= 4; j++)
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

            instruct.GetComponentInChildren<Text>().text = curInst;

            
            timeLeft = Random.Range(60, 90);
            stopTime = false;
            done = false;
            
         

        }
        else if (!stopTime && done)
        {
            block.SetActive(false);
            complete.SetActive(true);

            for (int r = 1; r <= 4; r++)
            {
                for (int w = 1; w <= 4; w++)
                {
                    target = r + "x" + w;
                    GameObject.Find(target).transform.GetChild(0).gameObject.SetActive(false);
                }

            }

            for (int j = 1; j <= 4; j++)
            {
                for (int l = 1; l <= 4; l++)
                {
                    target = j + "x" + l + "b";
                    GameObject.Find(target).transform.GetChild(0).GetComponent<InputField>().text = " ";
                    GameObject.Find(target).transform.GetChild(0).gameObject.SetActive(false);
                }

            }

            done = false;
            first = false;
            critSentry =+ tracker;
            curInst = normInst.ReadLine();
            NewMatrix();
        }



    }



    public void NewMatrix()
    {
        if (sentry == 1)
            return;
       
        string text;

        instruct.transform.GetChild(0).GetComponent<Text>().text = curInst;

        //this for loop sets all codes to be blank
        for (int g = 1; g <= 4; g++)
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
            tracker = int.Parse(splitter[4]);


            
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
            block.GetComponentInChildren<Text>().text = "Error in the number of tool codes entered. Please check matrix 1 for math errors.";
        }else
        {
            block.GetComponentInChildren<Text>().text = "Error in one of the entered tool codes. Please make sure they have been entered properly in Matrix 2.";

        }
    }


    public void critProblem()
    {
        critError.SetActive(true);

        box1.SetActive(false);
        box2.SetActive(false);
        critBox1.SetActive(true);
        critBox2.SetActive(true);

        errorInst = critInst.ReadLine();

        critSentry--;
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

        for (int g = 1; g <= 4; g++)
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
        GetColor = new Color32((byte) r,(byte) g,(byte) b,255);


        return GetColor;
    }
}

