using System;
using UnityEngine;

namespace Cars
{
    public class YRotator90 : MonoBehaviour
    {
        public Transform rotatable;
        public float speed = 1;

        private void Update()
        {
            if (rotatable == null) return;

            var eulerAngles = rotatable.localEulerAngles;
            var y = eulerAngles.y;
            y += speed * Time.deltaTime;
            y = DegreesHelper.MapDegreeTo0360(y);
            if (y > 90 && y < 270)
                speed = -speed;

            eulerAngles.y = y;
                
            rotatable.localEulerAngles = eulerAngles;
        }
    }
}