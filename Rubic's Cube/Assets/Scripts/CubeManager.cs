using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeManager : MonoBehaviour{
    public GameObject CubePiecePref;
    Transform CubeTransf;
    List<GameObject> AllCubePierces = new List<GameObject>();
    GameObject CubeCentrerPiece; //для 3х3 центральным кубиком будет 13-ый
    bool canShuffle=true,
        canRotate = true;

    List<GameObject> UpPieces
    {
        get
        {
            return AllCubePierces.FindAll(x => Mathf.Round(x.transform.localPosition.y) == 0);
        }
    }
    List<GameObject> DownPieces
    {
        get
        {
            return AllCubePierces.FindAll(x => Mathf.Round(x.transform.localPosition.y) == -2);
        }
    }
    List<GameObject> FrontPieces
    {
        get
        {
            return AllCubePierces.FindAll(x => Mathf.Round(x.transform.localPosition.x) == 0);
        }
    }
    List<GameObject> BackPieces
    {
        get
        {
            return AllCubePierces.FindAll(x => Mathf.Round(x.transform.localPosition.x) == -2);
        }
    }
    List<GameObject> LeftPieces
    {
        get
        {
            return AllCubePierces.FindAll(x => Mathf.Round(x.transform.localPosition.z) == 0);
        }
    }
    List<GameObject> RightPieces
    {
        get
        {
            return AllCubePierces.FindAll(x => Mathf.Round(x.transform.localPosition.z) == 2);
        }
    }
    List<GameObject> UpHorizintalPieces
    {
        get
        {
            return AllCubePierces.FindAll(x => Mathf.Round(x.transform.localPosition.x) == -1);
        }
    }
    List<GameObject> UpVerticalPieces
    {
        get
        {
            return AllCubePierces.FindAll(x => Mathf.Round(x.transform.localPosition.z) == 1);
        }
    }
    List<GameObject> FrontHorizintalPieces
    {
        get
        {
            return AllCubePierces.FindAll(x => Mathf.Round(x.transform.localPosition.y) == -1);
        }
    }

    Vector3[] RotationVectors =
    {
        new Vector3(0,1,0), new Vector3(0,-1,0),
        new Vector3(0,0,-1), new Vector3(0,0,1),
        new Vector3(1,0,0), new Vector3(-1,0,0)
    };

    void Start()
    {
        CubeTransf = transform;
        CreateCube();

    }

    // Update is called once per frame
    void Update()
    {
        if(canRotate)
        CheckInput();
    }

    void CreateCube()
    {
        foreach (GameObject go in AllCubePierces)
            DestroyImmediate(go);

        AllCubePierces.Clear();
        for (int x=0; x<3; x++)
            for (int y=0; y<3; y++)
                for(int z=0; z<3; z++)
                {
                    GameObject go = Instantiate(CubePiecePref, CubeTransf, false);
                    go.transform.localPosition = new Vector3(-x, -y, z);
                    go.GetComponent<CubePriceScript>().SetColor(-x, -y, z);
                    AllCubePierces.Add(go);
                }
        CubeCentrerPiece = AllCubePierces[13];
    }

    void CheckInput()
    {
        
        if (Input.GetKeyDown(KeyCode.W))
            StartCoroutine(Rotate(UpPieces, new Vector3(0, 1, 0)));
        else if (Input.GetKeyDown(KeyCode.S))
            StartCoroutine(Rotate(DownPieces, new Vector3(0, -1, 0)));
        else if (Input.GetKeyDown(KeyCode.A))
            StartCoroutine(Rotate(LeftPieces, new Vector3(0, 0, -1)));
        else if (Input.GetKeyDown(KeyCode.D))
            StartCoroutine(Rotate(RightPieces, new Vector3(0, 0, 1)));
        else if (Input.GetKeyDown(KeyCode.F))
            StartCoroutine(Rotate(FrontPieces, new Vector3(1, 0, 0)));
        else if (Input.GetKeyDown(KeyCode.B))
            StartCoroutine(Rotate(BackPieces, new Vector3(-1, 0, 0)));

        else if (Input.GetKeyDown(KeyCode.Space)&& canShuffle)
            StartCoroutine(Shuffle());
        else if (Input.GetKeyDown(KeyCode.E) && canShuffle)
            CreateCube();
    }

    IEnumerator Shuffle()
    {
        canShuffle = false;
        for(int moveCount = Random.Range(15,30); moveCount >=0; moveCount--)
        {
            int edge = Random.Range(0, 6);
            List<GameObject> edgePieces = new List<GameObject>();
            switch(edge)
            {
                    case 0: edgePieces = UpPieces; break;
                    case 1: edgePieces = DownPieces; break;
                    case 2: edgePieces = LeftPieces; break;
                    case 3: edgePieces = RightPieces; break;
                    case 4: edgePieces = FrontPieces; break;
                    case 5: edgePieces = BackPieces; break;
            }
            StartCoroutine(Rotate(edgePieces, RotationVectors[edge], 15));
            yield return new WaitForSeconds(.3f);

        }

        canShuffle = true;
    }

    IEnumerator Rotate(List<GameObject> pieces, Vector3 rotationVec, int speed=5)
    {
        canRotate = false;
        int angle = 0;
        while(angle<90)
        {
            foreach (GameObject go in pieces)
                go.transform.RotateAround(CubeCentrerPiece.transform.position, rotationVec, speed);
            angle += speed;
            yield return null;
        }
        CheckComplete();
        canRotate = true;
    }

    public void DetectRotate(List<GameObject> pieces, List<GameObject> planes)
    {
        if (!canRotate || !canShuffle)
            return;
        if (UpVerticalPieces.Exists(x => x == pieces[0]) &&
            UpVerticalPieces.Exists(x => x == pieces[1]))
            StartCoroutine(Rotate(UpVerticalPieces, new Vector3(0, 0, 1* DetectLeftMiddleRightSign(pieces))));
        else if (UpHorizintalPieces.Exists(x => x == pieces[0]) &&
            UpHorizintalPieces.Exists(x => x == pieces[1]))
            StartCoroutine(Rotate(UpHorizintalPieces, new Vector3(1*DetectFrontMiddleBackSign(pieces), 0, 0)));
        else if (FrontHorizintalPieces.Exists(x => x == pieces[0]) &&
            FrontHorizintalPieces.Exists(x => x == pieces[1]))
            StartCoroutine(Rotate(FrontHorizintalPieces, new Vector3(0, 1*DetectUpMiddleDownSign(pieces), 0)));

        else if (Detectside(planes, new Vector3(1, 0, 0), new Vector3(0, 0, 1), UpPieces))
            StartCoroutine(Rotate(UpPieces, new Vector3(0, 1 * DetectUpMiddleDownSign(pieces), 0)));

        else if (Detectside(planes, new Vector3(1, 0, 0), new Vector3(0, 0, 1), DownPieces))
            StartCoroutine(Rotate(DownPieces, new Vector3(0, 1 * DetectUpMiddleDownSign(pieces), 0)));

        else if (Detectside(planes, new Vector3(0, 0, 1), new Vector3(0, 1,0), FrontPieces))
            StartCoroutine(Rotate(FrontPieces, new Vector3(1*DetectFrontMiddleBackSign(pieces),0, 0)));

        else if (Detectside(planes, new Vector3(0, 0,1), new Vector3(0, 1,0), BackPieces))
            StartCoroutine(Rotate(BackPieces, new Vector3(1 * DetectFrontMiddleBackSign(pieces), 0, 0)));

        else if (Detectside(planes, new Vector3(1,0, 0), new Vector3(0, 1,0), LeftPieces))
            StartCoroutine(Rotate(LeftPieces, new Vector3(0, 0,1*DetectLeftMiddleRightSign(pieces))));

        else if (Detectside(planes, new Vector3(1, 0, 0), new Vector3(0, 1,0), RightPieces))
            StartCoroutine(Rotate(RightPieces, new Vector3(0, 0,1 * DetectLeftMiddleRightSign(pieces))));
    }

    bool Detectside(List<GameObject> planes, Vector3 fDirection, Vector3 sDirection, List<GameObject> side)
    {
        GameObject centerPiece = side.Find(x => x.GetComponent<CubePriceScript>().Planes.FindAll(y => y.activeInHierarchy).Count == 1);
        List<RaycastHit> hit1 = new List<RaycastHit>(Physics.RaycastAll(planes[1].transform.position, fDirection)),
            hit2 = new List<RaycastHit>(Physics.RaycastAll(planes[0].transform.position, fDirection)),
            hit1_m = new List<RaycastHit>(Physics.RaycastAll(planes[1].transform.position, -fDirection)),
            hit2_m = new List<RaycastHit>(Physics.RaycastAll(planes[0].transform.position, -fDirection)),

            hit3 = new List<RaycastHit>(Physics.RaycastAll(planes[1].transform.position, sDirection)),
            hit4 = new List<RaycastHit>(Physics.RaycastAll(planes[0].transform.position, sDirection)),
            hit3_m = new List<RaycastHit>(Physics.RaycastAll(planes[1].transform.position, sDirection)),
            hit4_m = new List<RaycastHit>(Physics.RaycastAll(planes[0].transform.position, sDirection));

        return hit1.Exists(x => x.collider.gameObject == centerPiece) ||
             hit2.Exists(x => x.collider.gameObject == centerPiece) ||
             hit1_m.Exists(x => x.collider.gameObject == centerPiece) ||
             hit2_m.Exists(x => x.collider.gameObject == centerPiece) ||
             hit3.Exists(x => x.collider.gameObject == centerPiece) ||
             hit4.Exists(x => x.collider.gameObject == centerPiece) ||
             hit3_m.Exists(x => x.collider.gameObject == centerPiece) ||
             hit4_m.Exists(x => x.collider.gameObject == centerPiece);
    }
    float DetectLeftMiddleRightSign(List<GameObject> pieces)
    {
        float sing = 0;
        if(Mathf.Round(pieces[1].transform.position.y)!=Mathf.Round(pieces[0].transform.position.y))
        {
            if (Mathf.Round(pieces[0].transform.position.x) == -2)
                sing = Mathf.Round(pieces[0].transform.position.y) - Mathf.Round(pieces[1].transform.position.y);
            else
                sing=Mathf.Round(pieces[0].transform.position.y) -
                                Mathf.Round(pieces[0].transform.position.y);
        }
        else
        {
            if (Mathf.Round(pieces[0].transform.position.y) == -2)
                sing = Mathf.Round(pieces[0].transform.position.x) -
                                Mathf.Round(pieces[1].transform.position.x);
            else
                sing = Mathf.Round(pieces[0].transform.position.x) -
                                Mathf.Round(pieces[0].transform.position.x);
        }
        return sing;
    }

    float DetectFrontMiddleBackSign(List<GameObject> pieces)
    {
        float sing = 0;
        if (Mathf.Round(pieces[1].transform.position.z) != Mathf.Round(pieces[0].transform.position.z))
        {
            if (Mathf.Round(pieces[0].transform.position.y) == 0)
                sing = Mathf.Round(pieces[1].transform.position.z) -
                                Mathf.Round(pieces[0].transform.position.z);
            else
                sing = Mathf.Round(pieces[0].transform.position.z) -
                                Mathf.Round(pieces[1].transform.position.z);
        }
        else
        {
            if (Mathf.Round(pieces[0].transform.position.z) == 0)
                sing = Mathf.Round(pieces[1].transform.position.y) -
                                Mathf.Round(pieces[1].transform.position.y);
            else
                sing = Mathf.Round(pieces[0].transform.position.y) -
                                Mathf.Round(pieces[1].transform.position.y);
        }
        return sing;
    }
    float DetectUpMiddleDownSign(List<GameObject> pieces)
    {
        float sing = 0;
        if (Mathf.Round(pieces[1].transform.position.z) != Mathf.Round(pieces[0].transform.position.z))
        {
            if (Mathf.Round(pieces[0].transform.position.x) == -2)
                sing = Mathf.Round(pieces[1].transform.position.z) -
                                Mathf.Round(pieces[0].transform.position.z);
            else
                sing = Mathf.Round(pieces[0].transform.position.z) -
                                Mathf.Round(pieces[1].transform.position.z);
        }
        else
        {
            if (Mathf.Round(pieces[0].transform.position.z) == 0)
                sing = Mathf.Round(pieces[0].transform.position.x) -
                                Mathf.Round(pieces[1].transform.position.x);
            else
                sing = Mathf.Round(pieces[1].transform.position.x) -
                                Mathf.Round(pieces[0].transform.position.x);
        }
        return sing;
    }

    void CheckComplete()
    {
        if (IsSideComplete(UpPieces) &&
            IsSideComplete(DownPieces) &&
            IsSideComplete(LeftPieces) &&
            IsSideComplete(RightPieces) &&
            IsSideComplete(FrontPieces) &&
            IsSideComplete(BackPieces))
            Debug.Log("complete!");
    }

    bool IsSideComplete(List<GameObject> pieces)
    {
        int mainPlaneIndex = pieces[4].GetComponent<CubePriceScript>().Planes.FindIndex(x => x.activeInHierarchy);
        for(int i=0; i<pieces.Count; i++)
        {
            if (!pieces[i].GetComponent<CubePriceScript>().Planes[mainPlaneIndex].activeInHierarchy ||
                pieces[i].GetComponent<CubePriceScript>().Planes[mainPlaneIndex].GetComponent<Renderer>().material.color !=
                pieces[4].GetComponent<CubePriceScript>().Planes[mainPlaneIndex].GetComponent<Renderer>().material.color)
            return false;
        }
        return true;
    }
}

