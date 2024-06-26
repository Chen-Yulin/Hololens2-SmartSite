using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyPoint
{
    public Vector3 pos = Vector3.zero;
    public bool grab = false;
    public bool wait = false;
    public KeyPoint(Vector3 pos, bool grab = false, bool wait = false)
    {
        this.pos = pos;
        this.grab = grab;
        this.wait = wait;
    }
}

public class Route
{
    public List<KeyPoint> routes = new List<KeyPoint>();
    public Route(List<KeyPoint> turningPoints)
    {
        if (turningPoints.Count < 2)
        {
            return;
        }
        for (int i = 1; i < turningPoints.Count; i++)
        {
            KeyPoint start = turningPoints[i-1];
            KeyPoint end = turningPoints[i];
            float dist = (start.pos - end.pos).magnitude;
            int sigment = (int)(dist * 20f);
            for (int j = 0; j <= sigment; j++)
            {
                routes.Add(new KeyPoint(start.pos + ((float)j) / sigment * (end.pos - start.pos), start.grab));
            }
        }
    }
}

public class RouteGenerator : MonoBehaviour
{

    public IKController IK;

    public Route route;
    // Start is called before the first frame update
    void Start()
    {
    }

    public void SetRoute(List<KeyPoint> turningPoints)
    {
        route = new Route(turningPoints);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
