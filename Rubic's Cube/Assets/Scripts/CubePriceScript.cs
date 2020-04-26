﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubePriceScript : MonoBehaviour
{
   /* public GameObject UpPlane, DownPlane,
                      FrontPlane, BackPlane,
                      LeftPlane, RightPlane;*/
    public List<GameObject> Planes = new List<GameObject>();
    public void SetColor(int x, int y, int z)
    {
        if (y == 0)
            Planes[0].SetActive(true);
        if (y == -2)
            Planes[1].SetActive(true);

        if (z == 0)
            Planes[3].SetActive(true);
        if (z == 2)
            Planes[2].SetActive(true);

        if (x == 0)
            Planes[4].SetActive(true);
        if (x == -2)
            Planes[5].SetActive(true);

    }
}
