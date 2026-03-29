using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneNameForUI : MonoBehaviour
{

    void Start()
    {
        Text label = GetComponent<Text>();
        if(label != null )
        {
            string name = SceneManager.GetActiveScene().name;
            label.text = name.Split(" - ")[1];
        }
    }
}
