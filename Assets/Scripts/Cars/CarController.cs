using System;
using System.Collections;
using UnityEngine;

namespace Cars
{
public class CarController : MonoBehaviour
{
    public Car car;

    private void Start()
    {
        StartCoroutine(CheckCarIsStuckAndResetIfNeeded());
    }
    
    private IEnumerator CheckCarIsStuckAndResetIfNeeded()
    {
        while (true)
        {
            yield return new WaitForSeconds(2);
            if (car != null && car.IsStuck())
                car.ResetTo();
        }
    }

    private void Update()
    {
        if (car == null) return;
        
        car.horizontalInput = Input.GetAxis("Horizontal");
        car.verticalInput = Input.GetAxis("Vertical");
    }
}
}