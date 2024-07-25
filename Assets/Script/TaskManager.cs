using System;
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
    public Vector3 move_start_right;
    public Vector3 move_end;
    public Vector3 move_end_right;

    public void InitAsMoveObject(Vector3 start, Vector3 end, Vector3 start_right, Vector3 end_right)
    {
        type = Type.MoveObject;
        move_start = start;
        move_end = end;
        move_start_right = start_right;
        move_end_right = end_right;
    }
}
public class TaskManager : MonoBehaviour // hold the coroutine of tasks
{
    public ArmTask task = new ArmTask();

    public Transform ArmEnd;
    public IKController IKSolver;
    public RouteGenerator routeGenerator;
    public JakaController jaka;
    

    Coroutine taskRoutine;

    GameObject taskSouce;

    public bool jakaRunning = false;


    public void JakaFinished()
    {
        jakaRunning = false;
    }

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
        turningPoint.Add(new KeyPoint(ArmEnd.position, task.move_start_right, false));
        turningPoint.Add(new KeyPoint(task.move_start + Vector3.up * 0.07f, task.move_start_right,false));
        turningPoint.Add(new KeyPoint(task.move_start, task.move_start_right, true));
        turningPoint.Add(new KeyPoint(task.move_start + Vector3.up * 0.07f, task.move_end_right, true));
        turningPoint.Add(new KeyPoint(task.move_end + Vector3.up * 0.07f, task.move_end_right, true));
        turningPoint.Add(new KeyPoint(task.move_end, task.move_end_right, false));
        turningPoint.Add(new KeyPoint(task.move_end + Vector3.up * 0.07f, default, false));
        routeGenerator.SetRoute(turningPoint);
        bool solveable = routeGenerator.CalculateSequence();

        if (!solveable)
        {
            Debug.Log("Unable to solve the arm action");
        }
        else
        {
            // generate vis
            routeGenerator.GenerateGhost();

            // execute action
            bool pre_grab = false;
            foreach (ArmAction action in routeGenerator.actionSequence)
            {
                
                jaka.SetJointRot(action.angles);
                yield return new WaitForSeconds(0.1f);
                if (pre_grab != action.grab)
                {
                    while((action.position - ArmEnd.position).magnitude > 0.04f)
                    {
                        Debug.Log("Arm end not here yet");
                        yield return new WaitForSeconds(0.1f);
                    }
                    pre_grab = action.grab;
                    jaka.SetGripper(!action.grab);
                    yield return new WaitForSeconds(0.5f);
                }
            }
            // cancel aim box when finish
            try
            {
                taskSouce.GetComponent<ObjectFrame>().CancelAim();
            }
            catch { }
        }
        routeGenerator.ResetRoute();
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
