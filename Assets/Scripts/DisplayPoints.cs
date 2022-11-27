using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DisplayPoints : MonoBehaviour
{
    [SerializeField] private Text _pointsLabel;
    void Update()
    {
        _pointsLabel.text = "POINTS: " + UserProfile.Points;
    }
}
