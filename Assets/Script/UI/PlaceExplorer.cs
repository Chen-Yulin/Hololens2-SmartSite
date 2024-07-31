using Microsoft.MixedReality.Toolkit.UI;
using Microsoft.MixedReality.Toolkit.UI.BoundsControl;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaceExplorer : MonoBehaviour
{
    public bool active = false;

    public GameObject SelectedButton = null;

    public Transform field;

    public RouteGenerator routeGenerator;

    public void ButtonPressed(GameObject button)
    {
        CancelButton();
        SelectedButton = button;
    }

    public void CancelButton()
    {
        if (SelectedButton)
        {
            //SelectedButton.GetComponent<PressableButtonHoloLens2>().ButtonPressed.Invoke();
        }
    }

    public void ClearObject()
    {
        for (int i = 0; i < field.childCount; i++)
        {
            Destroy(field.transform.GetChild(i).gameObject);
        }
    }

    public void DeployObject()
    {
        if (SelectedButton)
        {
            GameObject obj = Instantiate(SelectedButton.transform.Find("ButtonContent").GetChild(0).gameObject);
            obj.SetActive(true);
            obj.transform.parent = field;
            Obstacle obstacle = obj.AddComponent<Obstacle>();
            obstacle.routeGenerator = routeGenerator;
            ObjectManipulator OM = obj.AddComponent<ObjectManipulator>();
            OM.OnHoverEntered.AddListener(obj.GetComponent<Obstacle>().hoverEnter);
            OM.OnHoverExited.AddListener(obj.GetComponent<Obstacle>().hoverExit);
            obj.transform.localScale *= 20f;
            obj.transform.position = Camera.main.transform.position + Camera.main.transform.forward * 0.8f;
            BoundsControl BC = obj.AddComponent<BoundsControl>();
            BC.ScaleHandlesConfig.ScaleBehavior = Microsoft.MixedReality.Toolkit.UI.BoundsControlTypes.HandleScaleMode.NonUniform;
            obj.layer = obj.transform.parent.gameObject.layer;
        }
    }

    public void ToggleActive()
    {
        active = !active;
        foreach (Transform item in transform)
        {
            item.gameObject.SetActive(active);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        foreach (Transform item in transform)
        {
            item.gameObject.SetActive(active);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
