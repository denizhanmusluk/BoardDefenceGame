using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class BrokenPart : MonoBehaviour
{
    public Vector3 targetPos;
    Vector3 _targetPos;
   public void Brake()
    {
        StartCoroutine(Exp());
    }
    IEnumerator Exp()
    {
        Vector3 startPos = transform.position;
        _targetPos = startPos + targetPos;
        Vector3 rotateDirection_Random = new Vector3(0, 0, -targetPos.x);
        float counter = 0f;
        float speed = 1f;
        float posY = 0f;
        float posY_Factor = 2f;
        float angle = 0f;
        while (counter < 1f)
        {
            counter +=  speed * Time.deltaTime;
            counter = Mathf.Clamp01(counter);

            angle = counter * Mathf.PI;
            posY = posY_Factor * Mathf.Sin(angle);

            Vector3 trgtPos = Vector3.Lerp(startPos, _targetPos, counter);
            trgtPos.y += posY;
            transform.position = trgtPos;

            transform.Rotate(rotateDirection_Random * Time.deltaTime * 200);
            yield return null;
        }
        Destroy(gameObject);
    }
}
