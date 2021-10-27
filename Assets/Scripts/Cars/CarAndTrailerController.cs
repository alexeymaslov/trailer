using System;
using System.Collections;
using UnityEngine;

namespace Cars
{
public class CarAndTrailerController : MonoBehaviour
{
    public Car car;
    public Trailer trailer;

    private void Start()
    {
        StartCoroutine(CheckCarIsStuckAndResetIfNeeded());
    }
    
    private IEnumerator CheckCarIsStuckAndResetIfNeeded()
    {
        while (true)
        {
            yield return 0;
            if (car != null && car.IsStuck() && trailer != null && trailer.IsStuck())
            {
                car.ResetTo();
                trailer.Reset();
            }
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