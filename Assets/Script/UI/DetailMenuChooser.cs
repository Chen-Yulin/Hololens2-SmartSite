using Microsoft.MixedReality.Toolkit;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetailMenuChooser : MonoBehaviour
{
    // Start is called before the first frame update
    public void ChooseMenu(string name)
    {
        string wholeName = name + " Menu";
        foreach (Transform child in transform)
        {
            if (child.name == wholeName)
            {
                child.gameObject.SetActive(true);
            }
            else
            {
                child.gameObject.SetActive(false);
            }
        }
    }
}
