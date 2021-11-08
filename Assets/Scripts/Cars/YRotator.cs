using System;
using UnityEngine;

namespace Cars
{
    public class YRotator : MonoBehaviour
    {
        public Transform rotatable;
        public float speed = 1;

        private void Update()
        {
            if (rotatable == null) return;

            var eulerAngles = rotatable.localEulerAngles;
            var y = eulerAngles.y;
            y += speed * Time.deltaTime;
            eulerAngles.y = y;
            rotatable.localEulerAngles = eulerAngles;
        }
    }
}