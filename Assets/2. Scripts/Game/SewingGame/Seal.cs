using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PrebyopiaVR
{
    public class Seal : MonoBehaviour
    {
        public enum Area { None, A, B, PointA, PointB, PointC, PointD, Start, Finish }

        public Area area { get; private set; }

        private void OnTriggerEnter(Collider other)
        {
            switch (other.name)
            {
                case "A":
                    area = Area.A;
                    break;

                case "B":
                    area = Area.B;
                    break;

                case "PointA":
                    area = Area.PointA;
                    break;

                case "PointB":
                    area = Area.PointB;
                    break;

                case "PointC":
                    area = Area.PointC;
                    break;

                case "PointD":
                    area = Area.PointD;
                    break;

                case "Start":
                    area = Area.Start;
                    break;

                case "Finish":
                    area = Area.Finish;
                    break;
            }
        }
    }
}