using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Atmosphere
{
    public float pressureKPA, temperatureC;

    private const float mathE = 2.71828f;
    public Atmosphere(Vector3 position)
    {
        //   https://www.grc.nasa.gov/www/k-12/airplane/atmosmrm.html
        //   for altitudes below 7000m:
        //   p = .699 * exp(-0.00009 * h)
        //   T = -31 - 0.000998 * h

        float altitude = Environment.GetAltitude(position);
        pressureKPA = 0.699f * Mathf.Pow(mathE, -0.00009f * altitude);
        temperatureC = -31 - 0.000998f * altitude;
    }

    public Atmosphere(float pressureKPA, float temperatureC)
    {
        this.pressureKPA = pressureKPA;
        this.temperatureC = temperatureC;
    }
}
