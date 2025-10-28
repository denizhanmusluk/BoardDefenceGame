using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class SelfClose : MonoBehaviour
{
    public float closeDelay = 1f;
    private void OnEnable()
    {
        StartCoroutine(CloseDelay());
    }
    IEnumerator CloseDelay()
    {
        yield return new WaitForSeconds(closeDelay);
        gameObject.SetActive(false);
    }
}
