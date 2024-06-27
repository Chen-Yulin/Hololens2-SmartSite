using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ArmTask
{
    public enum Type
    {
        Finished,
        MoveObject
    }
    public Type type = Type.Finished;

    // moveObject
    public Vector3 move_start;
    public Vector3 move_end;

    public void InitAsMoveObject(Vector3 start, Vector3 end)
    {
        type = Type.MoveObject;
        move_start = start;
        move_end = end;
    }
}
public class TaskManager : MonoBehaviour
{
    public ArmTask task = new ArmTask();

    public Transform ArmEnd;
    public IKController IKSolver;
    public RouteGenerator routeGenerator;

    Coroutine taskRoutine;

    GameObject taskSouce;

    public void GetTask(ArmTask t, GameObject sender)
    {
        
        if (task.type == ArmTask.Type.Finished)
        {
            task = t;
            taskSouce = sender;
            Debug.Log("New " + task.type.ToString() + " task assigned.");
            
            if (taskRoutine != null)
            {
            }
            else
            {
                taskRoutine = StartCoroutine(ExecuteMoveObject());
            }
        }
        else
        {
            Debug.Log("Current task not finished.");
        }
        
    }

    IEnumerator ExecuteMoveObject()
    {
        Debug.Log("Start move object coroutine.");
        // generate the action sequence
        List<KeyPoint> turningPoint = new List<KeyPoint>();
        turningPoint.Add(new KeyPoint(ArmEnd.position, false));
        turningPoint.Add(new KeyPoint(task.move_start, true));
        turningPoint.Add(new KeyPoint(task.move_end, false));
        routeGenerator.SetRoute(turningPoint);
        bool solveable = routeGenerator.CalculateSequence();

        if (!solveable)
        {
            Debug.Log("Unable to solve the arm action");
        }
        else
        {
            // execute action
            routeGenerator.GenerateGhost();
            yield return new WaitForSeconds(3);
            // cancel aim box when finish
            try
            {
                taskSouce.GetComponent<ObjectFrame>().CancelAim();
            }
            catch { }
        }
        taskRoutine = null;
        task = new ArmTask();
        
        
        Debug.Log("Task Finished");
        yield break;
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
