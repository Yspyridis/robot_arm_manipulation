using System.Collections;
using UnityEngine;

public class Dummy : MonoBehaviour
{
    public Cloth cloth;

    IEnumerator delayed(float seconds) {
        yield return new WaitForSeconds(seconds);


        var c = cloth.coefficients;
        cloth.coefficients[0].maxDistance = float.MaxValue;
        cloth.coefficients = c;

         Debug.Log(c.GetHashCode());
    }

    private void Start() {

        var c = cloth.coefficients;
        c[0].maxDistance = 0.1f;
        cloth.coefficients = c;

        Debug.Log(c.GetHashCode());

        StartCoroutine(delayed(2));
    }
}
