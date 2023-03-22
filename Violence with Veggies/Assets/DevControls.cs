using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DevControls : MonoBehaviour
{
    //an array of all th differnt soils
    public GameObject[] soils;

    //instantly do differnt actions
    public bool instaTil;
    public bool instaWater;
    public bool instaGrow;



    private void Update()
    {
        //instantly grow all crops
        if (instaGrow)
        {
            for (int i = 0; i < GameObject.FindGameObjectsWithTag("Soil").Length; i++)
                GameObject.FindGameObjectsWithTag("Soil")[i].GetComponent<SoilController>().cropTime = GameObject.FindGameObjectsWithTag("Soil")[i].GetComponent<SoilController>().cropReady;
            instaGrow = false;
        }
        //instantly till all crops
        if (instaTil)
        {
            for (int i = 0; i < GameObject.FindGameObjectsWithTag("Soil").Length; i++)
            {
                if (GameObject.FindGameObjectsWithTag("Soil")[i].GetComponent<SoilController>().stage == 0)
                    GameObject.FindGameObjectsWithTag("Soil")[i].GetComponent<SoilController>().stage = 1;
            }
            instaTil = false;
        }
        //instantly water all crops
        if (instaWater)
        {
            for (int i = 0; i < GameObject.FindGameObjectsWithTag("Soil").Length; i++)
                GameObject.FindGameObjectsWithTag("Soil")[i].GetComponent<SoilController>().watered = true;
            instaWater = false;
        }



    }
}
