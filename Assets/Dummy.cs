using System.Collections;
using UnityEngine;

public class Dummy : MonoBehaviour
{
    public Cloth cloth;

    IEnumerator Delayed(float seconds)
    {
        yield return new WaitForSeconds(seconds);

        var coefficients = cloth.coefficients;

        for (int i = 0; i < coefficients.Length; i++)
        {
            coefficients[i].maxDistance = 100f;
        }

        cloth.coefficients = coefficients;

        Debug.Log("maxDistance increased to allow more movement.");
    }

    private void Start()
    {
        var coefficients = cloth.coefficients;

        for (int i = 0; i < coefficients.Length; i++)
        {
            coefficients[i].maxDistance = 3f;
        }

        cloth.coefficients = coefficients;

        Debug.Log("Initial maxDistance set to 3f to constrain cloth movement.");

        StartCoroutine(Delayed(2f));
    }
}
