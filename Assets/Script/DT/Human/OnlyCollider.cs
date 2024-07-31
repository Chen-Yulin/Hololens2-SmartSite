using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class OnlyCollider : MonoBehaviour
{
    // Keep data transmittion between pyKinect and Unity
    private HumanUdpSocket udp;

    // Colliders of the Human DT
    private CapsuleCollider Left_Upper_arm_collider;
    private CapsuleCollider Left_Forearm_collider;
    private CapsuleCollider Right_Upper_arm_collider;
    private CapsuleCollider Right_Forearm_collider;
    private BoxCollider Chest_collider;

    // GameObject type of the body parts
    private GameObject Left_upper_arm;
    private GameObject Right_upper_arm;
    private GameObject Left_forearm;
    private GameObject Right_forearm;
    private GameObject Left_hand;
    private GameObject Right_hand;
    private GameObject Chest;
    private GameObject Head;

    // Removes brackets and spaces
    private readonly string pattern = @"\[\s*(-?\d+\.\d+)\s+(-?\d+\.\d+)\s+(-?\d+\.\d+)\]";

    // Change human body parts in Unity
    public bool ChangeUpperarmRadius;
    public bool ChangeForearmRadius;
    public float UpperarmRadius;
    public float ForearmRadius;

    private void Start()
    {
        udp = gameObject.GetComponent<HumanUdpSocket>();

        Left_upper_arm = GameObject.Find("left_upperarm");
        Right_upper_arm = GameObject.Find("right_upperarm");
        Left_forearm = GameObject.Find("left_forearm");
        Right_forearm = GameObject.Find("right_forearm");
        //Left_hand = GameObject.Find("left_hand");
        //Right_hand = GameObject.Find("right_hand");
        Chest = GameObject.Find("chest");
        Head = GameObject.Find("head");

        Left_Upper_arm_collider = Left_upper_arm.GetComponent<CapsuleCollider>();
        Left_Forearm_collider = Left_forearm.GetComponent<CapsuleCollider>();
        //Left_Hand_collider = Left_hand.GetComponent<SphereCollider>();
        Right_Upper_arm_collider = Right_upper_arm.GetComponent<CapsuleCollider>();
        Right_Forearm_collider = Right_forearm.GetComponent<CapsuleCollider>();
        //Right_Hand_collider = Right_hand.GetComponent<SphereCollider>();
        Chest_collider = Chest.GetComponent<BoxCollider>();

        ChangeUpperarmRadius = false;
        ChangeForearmRadius = false;
        UpperarmRadius = Right_Upper_arm_collider.radius;
        ForearmRadius = Right_Forearm_collider.radius;
    }

    private void FixedUpdate()
    {
        if (ChangeForearmRadius)
            Change_Forearm_Radius();

        if (ChangeUpperarmRadius)
            Change_Upperarm_Radius();
        string inf_handpos = udp.received_pos_hand;
        string inf_hand = udp.received_data_hand;
        string inf_body = udp.received_data_body;

        List<Vector3> hand_pos = TurnMessageIntoVector3(inf_handpos);
        List<Vector3> hand_pointing_towards = TurnMessageIntoVector3(inf_hand);
        List<Vector3> body_pos = TurnMessageIntoVector3(inf_body);

        if (hand_pos.Count == 6 && hand_pointing_towards.Count == 4)
        {
            hand_pos = Adjust_Vector3(hand_pos);
            hand_pointing_towards = Adjust_Vector3(hand_pointing_towards);

            Vector3 l_hand = hand_pos[0];
            Vector3 l_arm = hand_pos[1];
            Vector3 l_shoulder = hand_pos[2];
            Vector3 r_hand = hand_pos[3];
            Vector3 r_arm = hand_pos[4];
            Vector3 r_shoulder = hand_pos[5];
            // Debug.Log($"Pos: {l_hand}, {l_arm}, {l_shoulder}, {r_hand}, {r_arm}, {r_shoulder}");

            Vector3 l_forearm_center = 0.5f * (l_hand + l_arm);
            Vector3 r_forearm_center = 0.5f * (r_hand + r_arm);
            Vector3 l_upperarm_center = 0.5f * (l_shoulder + l_arm);
            Vector3 r_upperarm_center = 0.5f * (r_shoulder + r_arm);

            float l_upperarm_length = Vector3.Distance(l_shoulder, l_arm);
            float r_upperarm_length = Vector3.Distance(r_shoulder, r_arm);
            float l_forearm_length = Vector3.Distance(l_hand, l_arm);
            float r_forearm_length = Vector3.Distance(r_hand, r_arm);

            Adjust_Collider(l_forearm_center, r_forearm_center, l_upperarm_center, r_upperarm_center, l_hand, r_hand,
                l_upperarm_length, r_upperarm_length, l_forearm_length, r_forearm_length,
                hand_pointing_towards);
        }

        if (hand_pos.Count == 6 && body_pos.Count == 3)
        {
            body_pos = Adjust_Vector3(body_pos);
            Vector3 head = body_pos[0];
            Vector3 neck = body_pos[1];
            Vector3 naval = body_pos[2];
            Vector3 to_right = hand_pos[5] - hand_pos[2];

            Vector3 chest_center = 0.5f * (neck + naval);

            Vector3 chest_up = (neck - naval).normalized;

            Adjust_Body_Collider(head, chest_center, chest_up, to_right);
        }
    }

    private List<Vector3> TurnMessageIntoVector3(string information)
    {
        List<Vector3> result = new List<Vector3>();

        foreach (System.Text.RegularExpressions.Match m in System.Text.RegularExpressions.Regex.Matches(information, pattern))
        {
            Vector3 vec = new Vector3(
                float.Parse(m.Groups[1].Value),
                float.Parse(m.Groups[2].Value),
                float.Parse(m.Groups[3].Value));
            // Debug.Log(vec);
            result.Add(vec);
        }

        return result;
    }

    private void Adjust_Collider(
        Vector3 l_forearm_center,
        Vector3 r_forearm_center,
        Vector3 l_upperarm_center,
        Vector3 r_upperarm_center,
        Vector3 l_hand_center,
        Vector3 r_hand_center,
        float l_upperarm_length,
        float r_upperarm_length,
        float l_forearm_length,
        float r_forearm_length,
        List<Vector3> hand_pointing_towards)
    {
        Left_Upper_arm_collider.height = l_upperarm_length;
        Right_Upper_arm_collider.height = r_upperarm_length;
        Left_Forearm_collider.height = l_forearm_length;
        Right_Forearm_collider.height = r_forearm_length;

        Left_Upper_arm_collider.transform.localPosition = l_upperarm_center;
        Right_Upper_arm_collider.transform.localPosition = r_upperarm_center;
        Left_Forearm_collider.transform.localPosition = l_forearm_center;
        Right_Forearm_collider.transform.localPosition = r_forearm_center;
        //Left_Hand_collider.transform.localPosition = l_hand_center;
        //Right_Hand_collider.transform.localPosition = r_hand_center;

        Left_Upper_arm_collider.transform.localRotation = Quaternion.LookRotation(-hand_pointing_towards[0].normalized, 
                                                                                Left_Upper_arm_collider.transform.up);
        Right_Upper_arm_collider.transform.localRotation = Quaternion.LookRotation(-hand_pointing_towards[1].normalized,
                                                                                Right_Upper_arm_collider.transform.up);
        Left_Forearm_collider.transform.localRotation = Quaternion.LookRotation(-hand_pointing_towards[2].normalized,
                                                                                Left_Forearm_collider.transform.up);
        Right_Forearm_collider.transform.localRotation = Quaternion.LookRotation(-hand_pointing_towards[3].normalized,
                                                                                Right_Forearm_collider.transform.up);
    }

    private void Adjust_Body_Collider(
    Vector3 head,
    Vector3 chest_center,
    Vector3 chest_up,
    Vector3 to_right
    )
    {
        Chest_collider.transform.localPosition = chest_center;

        Chest_collider.transform.localRotation = Quaternion.Lerp(Chest_collider.transform.localRotation, Quaternion.LookRotation(chest_up, Vector3.Cross(chest_up, to_right)
                                                                                ), 0.1f);
    }

    private Vector3 Adjust_Vector3(Vector3 vec)
    {
        Vector3 result = vec / 1000;
        result.y = -result.y;
        return result;
    }

    private List<Vector3> Adjust_Vector3(List<Vector3> vec_list)
    {
        List<Vector3> result = new List<Vector3>();
        foreach (var vec in vec_list)
            result.Add(Adjust_Vector3(vec));
        return result;
    }

    private void Change_Upperarm_Radius()
    {
        Left_Upper_arm_collider.radius = UpperarmRadius;
        Right_Upper_arm_collider.radius = UpperarmRadius;
    }

    private void Change_Forearm_Radius()
    {
        Left_Forearm_collider.radius = ForearmRadius;
        Right_Forearm_collider.radius = ForearmRadius;
    }
}