using UnityEngine;
using System;
using System.Collections;
using System.IO;
using System.Collections.Specialized;

/*
Usage:
1. Attach this script to your chosen camera's game object.
2. Set that camera's Clear Flags field to Solid Color.
3. Use the inspector to set frameRate and framesToCapture
4. Choose your desired resolution in Unity's Game window (must be less than or equal to your screen resolution)
5. Turn on "Maximise on Play"
6. Play your scene. Screenshots will be saved to YourUnityProject/Screenshots by default.
*/

public class sdfs : MonoBehaviour
{

    #region public fields
    [Tooltip("A folder will be created with this base name in your project root")]
    public string folderBaseName = "Screenshots";
    public string anglefolderBaseName = "Angles";
    public string labelFolderBaseName = "Labels";
    public string positionFolderBaseName = "Position";
    public string campositionFolderBaseName = "CamPosition";
    public string boundFolderBaseName = "bounds";
    public string matrixFolderBaseName = "Matrix";
    [Tooltip("How many frames should be captured per second of game time")]
    public int frameRate = 24;
    public GameObject BckBoundBox = null;
    [Tooltip("How many frames should be captured before quitting")]
    public int framesToCapture = 1;
    #endregion
    #region private fields
    private string folderName = "";
    private string labelFolderName = "";
    private string positionFolderName = "";
    private string campositionFolderName = "";
    private string matrixFolderName = "";
    private string anglefolderName = "";
    private string boundFolderName = "";
    //private GameObject whiteCamGameObject;
    //private Camera whiteCam;
    //private GameObject blackCamGameObject;
    //private Camera blackCam;
    private Camera mainCam;
    private int videoFrame = 0; // how many frames we've rendered
    private float originalTimescaleTime;
    private bool done = false;
    private int screenWidth;
    private int screenHeight;
    //private Texture2D textureBlack;
    //private Texture2D textureWhite;
    private Texture2D textureTransparentBackground;
    public GameObject boom = null;
    public GameObject arm = null;
    public GameObject bucket = null;
    #endregion

    void Awake()
    {
        Debug.Log("first");
        mainCam = gameObject.GetComponent<Camera>();

        CreateNewFolderForScreenshots();
        CreateNewFolderForAngles();
        CreateNewFolderForLabels();
        CreateNewFolderForPosition();
        CreateNewFolderForbounds();
        CreateNewFolderForMatrix();
        CreateNewFolderForCamPosition();
        CacheAndInitialiseFields();
        Time.captureFramerate = frameRate;
    }

    void LateUpdate()
    {
        if (!done)
        {
            StartCoroutine(CaptureFrame());
        }
        else
        {
            Debug.Log("Complete! " + videoFrame + " videoframes rendered. File names are 0 indexed)");
            Debug.Log("Label_Captured");
            Debug.Break();
        }
    }

    IEnumerator CaptureFrame()
    {
        yield return new WaitForEndOfFrame();
        if (videoFrame < framesToCapture)
        {


            GetScreenPoint(mainCam);
            GetPosition();
            GetMatrix(mainCam);
            GetCamPosition(mainCam);
            GetAngles();
            GetBounds();
            SavePng();
            videoFrame++;
            Debug.Log("Rendered frame " + videoFrame);
            //Debug.Log("localangleboom" + boom.transform.localEulerAngles);
            //videoFrame++;
        }
        else
        {
            done = true;
            StopCoroutine("CaptureFrame");
        }
    }

    void GetScreenPoint(Camera cam)
    {
        GameObject[] LabelList = GameObject.FindGameObjectsWithTag("Label");
        Debug.Log(LabelList[1]);
        int size = LabelList.Length;

        string[] ScreenPoints = new string[size];

        for (int i = 0; i < size; i++)
        {
            ScreenPoints[i] = LabelList[i].name + ", " + cam.WorldToScreenPoint(LabelList[i].transform.position).x.ToString() + ", " + cam.WorldToScreenPoint(LabelList[i].transform.position).y.ToString() + ", " + cam.WorldToScreenPoint(LabelList[i].transform.position).z.ToString() + "\n";
        }

        string labelname = string.Format("{0}/{1:D04}.txt", labelFolderName, videoFrame);
        FileStream file = File.Create(labelname);
        using (StreamWriter streamwriter = new StreamWriter(file, System.Text.Encoding.UTF8))
        {
            streamwriter.Write("name, x, y, z\n");

            for (int i = 0; i < size; i++)
            {
                streamwriter.Write(ScreenPoints[i]);
            }
        }
    }
    void GetPosition()
    {
        GameObject[] LabelList = GameObject.FindGameObjectsWithTag("Label");
        int size = LabelList.Length;

        string[] Position = new string[size];

        for (int i = 0; i < size; i++)
        {
            Position[i] = LabelList[i].name + ", " + LabelList[i].transform.position.x.ToString() + ", " + LabelList[i].transform.position.y.ToString() + ", " + LabelList[i].transform.position.z.ToString() + "\n";
        }
        string positionname = string.Format("{0}/{1:D04}.txt", positionFolderName, videoFrame);
        FileStream file = File.Create(positionname);
        using (StreamWriter streamwriter = new StreamWriter(file, System.Text.Encoding.UTF8))
        {
            streamwriter.Write("name, x, y, z\n");

            for (int i = 0; i < size; i++)
            {
                streamwriter.Write(Position[i]);
            }
        }
    }
    void GetMatrix(Camera cam)
    {
        Matrix4x4 mat = cam.worldToCameraMatrix;

        string Matrx = mat[0, 0] + ", " + mat[0, 1] + ", " + mat[0, 2] + ", " + mat[0, 3] + ", " + mat[1, 0] + ", " + mat[1, 1] + ", " + mat[1, 2] + ", " + mat[1, 3] + ", " + mat[2, 0] + ", " + mat[2, 1] + ", " + mat[2, 2] + ", " + mat[2, 3] + ", " + mat[3, 0] + ", " + mat[3, 1] + ", " + mat[3, 2] + ", " + mat[3, 3];


        string matrixname = string.Format("{0}/{1:D04}.txt", matrixFolderName, videoFrame);
        FileStream file = File.Create(matrixname);
        using (StreamWriter streamwriter = new StreamWriter(file, System.Text.Encoding.UTF8))
        {
            streamwriter.Write("M00, M01, M02, M03, M10, M11, M12, M13, M20, M21, M22, M23, M30, M31, M32, M33\n");
            streamwriter.Write(Matrx);
        }
    }
    void GetAngles()
    {
        string[] Angles = new string[3];
        Angles[0] = "Boom" + ", " + boom.transform.localEulerAngles.x.ToString() + ", " + boom.transform.localEulerAngles.y.ToString() + ", " + boom.transform.localEulerAngles.z.ToString() + "\n";
        Angles[1] = "Arm" + ", " + arm.transform.localEulerAngles.x.ToString() + ", " + arm.transform.localEulerAngles.y.ToString() + ", " + arm.transform.localEulerAngles.z.ToString() + "\n";
        Angles[2] = "bucket" + ", " + bucket.transform.localEulerAngles.x.ToString() + ", " + bucket.transform.localEulerAngles.y.ToString() + ", " + bucket.transform.localEulerAngles.z.ToString() + "\n";
        string Anglesname = string.Format("{0}/{1:D04}.txt", anglefolderName, videoFrame);
        FileStream file = File.Create(Anglesname);
        using (StreamWriter streamwriter = new StreamWriter(file, System.Text.Encoding.UTF8))
        {
            streamwriter.Write("name, x, y, z\n");

            for (int i = 0; i < 3; i++)
            {
                streamwriter.Write(Angles[i]);
            }
        }
    }
    public Vector2[] GUI3dRectWithObject(GameObject go, Camera cam)
    {

        Vector3 cen = go.GetComponent<Renderer>().bounds.center;
        Vector3 ext = go.GetComponent<Renderer>().bounds.extents;
        Vector3[] extentPoints1 = new Vector3[8]
        {
            cam.WorldToScreenPoint(new Vector3(cen.x-ext.x, cen.y-ext.y, cen.z-ext.z)),
            cam.WorldToScreenPoint(new Vector3(cen.x+ext.x, cen.y-ext.y, cen.z-ext.z)),
            cam.WorldToScreenPoint(new Vector3(cen.x-ext.x, cen.y-ext.y, cen.z+ext.z)),
            cam.WorldToScreenPoint(new Vector3(cen.x+ext.x, cen.y-ext.y, cen.z+ext.z)),
            cam.WorldToScreenPoint(new Vector3(cen.x-ext.x, cen.y+ext.y, cen.z-ext.z)),
            cam.WorldToScreenPoint(new Vector3(cen.x+ext.x, cen.y+ext.y, cen.z-ext.z)),
            cam.WorldToScreenPoint(new Vector3(cen.x-ext.x, cen.y+ext.y, cen.z+ext.z)),
            cam.WorldToScreenPoint(new Vector3(cen.x+ext.x, cen.y+ext.y, cen.z+ext.z))
        };
        Vector2[] extentPoints = new Vector2[8]
        {
            new Vector2(extentPoints1[0].x,1280-extentPoints1[0].y),
            new Vector2(extentPoints1[1].x,1280-extentPoints1[1].y),
            new Vector2(extentPoints1[2].x,1280-extentPoints1[2].y),
            new Vector2(extentPoints1[3].x,1280-extentPoints1[3].y),
            new Vector2(extentPoints1[4].x,1280-extentPoints1[4].y),
            new Vector2(extentPoints1[5].x,1280-extentPoints1[5].y),
            new Vector2(extentPoints1[6].x,1280-extentPoints1[6].y),
            new Vector2(extentPoints1[7].x,1280-extentPoints1[7].y),
        };
        Vector2 min = extentPoints[0];
        Vector2 max = extentPoints[0];

        foreach (Vector2 v in extentPoints)
        {
            min = Vector2.Min(min, v);
            max = Vector2.Max(max, v);
        }
        return new Vector2[4] { new Vector2(min.x, min.y), new Vector2(min.x, max.y), new Vector2(max.x, min.y), new Vector2(max.x, max.y) };
    }
    void GetBounds()
    {
        string[] BoundPoints = new string[4];
        
        Vector2[] bb = GUI3dRectWithObject(BckBoundBox, mainCam);
        for (int i = 0; i < 4; i++)
        {
            BoundPoints[i] = bb[i].x.ToString() + ", " + bb[i].y.ToString() + "\n";
        }

            
        string BoundPointsname = string.Format("{0}/{1:D04}.txt", boundFolderName, videoFrame);
        FileStream file = File.Create(BoundPointsname);
        using (StreamWriter streamwriter = new StreamWriter(file, System.Text.Encoding.UTF8))
        {
            streamwriter.Write("x, y\n");

            for (int i = 0; i < 4; i++)
            {
                streamwriter.Write(BoundPoints[i]);
            }
        }

    }
    void GetCamPosition(Camera cam)
    {

        string CamPosition;


        CamPosition = cam.transform.position.x.ToString() + ", " + cam.transform.position.y.ToString() + ", " + cam.transform.position.z.ToString() + "\n";

        string campositionname = string.Format("{0}/{1:D04}.txt", campositionFolderName, videoFrame);
        FileStream file = File.Create(campositionname);
        using (StreamWriter streamwriter = new StreamWriter(file, System.Text.Encoding.UTF8))
        {
            streamwriter.Write("x, y, z\n");
            streamwriter.Write(CamPosition);

        }
    }
    void RenderCamToTexture(Camera cam, Texture2D tex)
    {
        cam.enabled = true;
        cam.Render();
        WriteScreenImageToTexture(tex);
        cam.enabled = false;
    }



    void CreateNewFolderForScreenshots()
    {
        // Find a folder name that doesn't exist yet. Append number if necessary.
        folderName = folderBaseName;
        int count = 1;
        while (System.IO.Directory.Exists(folderName))
        {
            folderName = folderBaseName + count;
            count++;
        }
        System.IO.Directory.CreateDirectory(folderName); // Create the folder
    }
    void CreateNewFolderForAngles()
    {
        anglefolderName = anglefolderBaseName;
        int count = 1;
        while (System.IO.Directory.Exists(anglefolderName))
        {
            anglefolderName = anglefolderBaseName + count;
            count++;
        }
        System.IO.Directory.CreateDirectory(anglefolderName); // Create the folder
    }
    void CreateNewFolderForLabels()
    {
        // Find a folder name that doesn't exist yet. Append number if necessary.
        labelFolderName = labelFolderBaseName;
        int count = 1;
        while (System.IO.Directory.Exists(labelFolderName))
        {
            labelFolderName = labelFolderBaseName + count;
            count++;
        }
        System.IO.Directory.CreateDirectory(labelFolderName); // Create the folder
    }
    void CreateNewFolderForbounds()
    {
        boundFolderName = boundFolderBaseName;
        int count = 1;
        while (System.IO.Directory.Exists(boundFolderName))
        {
            boundFolderName = boundFolderBaseName + count;
            count++;
        }
        System.IO.Directory.CreateDirectory(boundFolderName); // Create the folder
    }
    void CreateNewFolderForPosition()
    {
        // Find a folder name that doesn't exist yet. Append number if necessary.
        positionFolderName = positionFolderBaseName;
        int count = 1;
        while (System.IO.Directory.Exists(positionFolderName))
        {
            positionFolderName = positionFolderBaseName + count;
            count++;
        }
        System.IO.Directory.CreateDirectory(positionFolderName); // Create the folder
    }
    void CreateNewFolderForCamPosition()
    {
        // Find a folder name that doesn't exist yet. Append number if necessary.
        campositionFolderName = campositionFolderBaseName;
        int count = 1;
        while (System.IO.Directory.Exists(campositionFolderName))
        {
            campositionFolderName = campositionFolderBaseName + count;
            count++;
        }
        System.IO.Directory.CreateDirectory(campositionFolderName); // Create the folder
    }
    void CreateNewFolderForMatrix()
    {
        matrixFolderName = matrixFolderBaseName;
        int count = 1;
        while (System.IO.Directory.Exists(matrixFolderName))
        {
            matrixFolderName = matrixFolderBaseName + count;
            count++;
        }
        System.IO.Directory.CreateDirectory(matrixFolderName); // Create the folder
    }

    void WriteScreenImageToTexture(Texture2D tex)
    {
        tex.ReadPixels(new Rect(0, 0, screenWidth, screenHeight), 0, 0);
        tex.Apply();
    }



    void SavePng()
    {
        string name = string.Format("{0}/{1:D04}.jpg", folderName, videoFrame);
        textureTransparentBackground = ScreenCapture.CaptureScreenshotAsTexture();
        var pngShot = textureTransparentBackground.EncodeToJPG();
        File.WriteAllBytes(name, pngShot);
    }

    /*
    void SaveLabels(string[] pointlist)
    {
        string labelname = string.Format("{0}/{1:D04}.txt", labelFolderName, videoFrame);
        System.IO.FileStream file = new System.IO.FileStream(labelname, System.IO.FileMode.Create);
        StreamWriter streamwriter = new StreamWriter(file);


        int size_of_list = pointlist.Length;

        for (int i = 0; i < size_of_list; i++)
        {
            string stringtowrite = pointlist[i];
            streamwriter.Write(stringtowrite);
        }

        streamwriter.Close();                
    }
    */

    void CacheAndInitialiseFields()
    {
        originalTimescaleTime = Time.timeScale;
        screenWidth = Screen.width;
        screenHeight = Screen.height;

        textureTransparentBackground = new Texture2D(screenWidth, screenHeight, TextureFormat.RGB24, false);
    }
}