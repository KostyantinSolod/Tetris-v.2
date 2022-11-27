using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BackButton : MonoBehaviour
{
    
    private void Start()
    {
        var button = GetComponent<Button>();
        button.onClick.AddListener(()=>SceneManager.LoadScene("Main Menu"));
    }
}
