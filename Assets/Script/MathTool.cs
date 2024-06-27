using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;


namespace WW2NavalAssembly
{
    public static class MathTool
    {
        public static float GetArea(Vector3 v3)
        {
            float[] a = new float[] { v3.x, v3.y, v3.z };
            Array.Sort(a);
            return a[2] * a[1];
        }
        public static Vector2 GetRotatePosition(Vector2 targetPosition, Vector2 centerPosition, float angle)
        {
            float endX = (targetPosition.x - centerPosition.x) * Mathf.Cos(angle * Mathf.Deg2Rad) - (targetPosition.y - centerPosition.y) * Mathf.Sin(angle * Mathf.Deg2Rad) + centerPosition.x;
            float endY = (targetPosition.y - centerPosition.y) * Mathf.Cos(angle * Mathf.Deg2Rad) + (targetPosition.x - targetPosition.x) * Mathf.Sin(angle * Mathf.Deg2Rad) + centerPosition.y;
            return new Vector2(endX, endY);
        }
        public static void AxisLookAt(Transform tr_self, Vector3 lookPos, Vector3 directionAxis, float speed)
        {
            var rotation = tr_self.rotation;
            var targetDir = lookPos - tr_self.position;
            //ָ���ĸ��ᳯ��Ŀ��,�����޸�Vector3�ķ���
            var fromDir = tr_self.rotation * directionAxis;
            //���㴹ֱ�ڵ�ǰ�����Ŀ�귽�����
            var axis = Vector3.Cross(fromDir, targetDir).normalized;
            //���㵱ǰ�����Ŀ�귽��ļн�
            var angle = Vector3.Angle(fromDir, targetDir);
            //����ǰ������Ŀ�귽����תһ���Ƕȣ�����Ƕ�ֵ��������ֵ
            tr_self.rotation = Quaternion.Lerp(rotation, Quaternion.AngleAxis(angle, axis) * rotation, speed);

        }//from CSDN
        public static Vector2 Get2DCoordinate(Vector3 v)
        {
            return new Vector2(v.x, v.z);
        }
        public static float Get2DDistance(Vector3 v1, Vector3 v2)
        {
            return Vector2.Distance(Get2DCoordinate(v1), Get2DCoordinate(v2));
        }
        public static Vector2 PointRotate(Vector2 center, Vector2 p1, float angle)
        {
            Vector2 tmp = new Vector2();
            double angleHude = angle * Math.PI / 180;/*�Ƕȱ�ɻ���*/
            double x1 = (p1.x - center.x) * Math.Cos(angleHude) + (p1.y - center.y) * Math.Sin(angleHude) + center.x;
            double y1 = -(p1.x - center.x) * Math.Sin(angleHude) + (p1.y - center.y) * Math.Cos(angleHude) + center.y;
            tmp.x = (float)x1;
            tmp.y = (float)y1;
            return tmp;
        }
        public static float SignedAngle(Vector2 v1, Vector2 v2)
        {
            if (v1.x * v2.y - v1.y * v2.x < 0)
            {
                return -Vector2.Angle(v1, v2);
            }
            else
            {
                return Vector2.Angle(v1, v2);
            }
        }
        public static float GetInitialVel(float caliber, bool AA)
        {
            return (700 + 0.2f * (caliber - 100) + ((20000) / (caliber + 30))) * (AA ? 1.5f : 1f) / 2; // for 1:10
            //return (130 + 0.08f * (caliber + 50) + ((18000) / (caliber + 100))) * (AA ? 2 : 1); // for 1:20
        }
        public static int GetQueueIndex<T>(Queue<T> queue, T target)
        {
            int index = 0;
            foreach (T element in queue)
            {
                if (EqualityComparer<T>.Default.Equals(element, target))
                {
                    return index;
                }
                index++;
            }
            return -1; // Element not found
        }
    }
}
