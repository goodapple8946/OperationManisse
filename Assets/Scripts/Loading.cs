using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Loading : MonoBehaviour
{
    private float time;
    private float timeMax = 3f;

    void Update()
    {
        if (time <= timeMax)
        {
            GetComponent<Image>().color = Color.Lerp(new Color(0, 0, 0, 1f), new Color(0, 0, 0, 0), time / timeMax);
            transform.GetChild(0).GetComponent<Text>().color = Color.Lerp(new Color(1, 1, 1, 1f), new Color(1, 1, 1, 0), time / timeMax);
            transform.GetChild(1).GetComponent<Text>().color = Color.Lerp(new Color(1, 1, 1, 1f), new Color(1, 1, 1, 0), time / timeMax);
            transform.GetChild(2).GetComponent<Text>().color = Color.Lerp(new Color(1, 1, 1, 1f), new Color(1, 1, 1, 0), time / timeMax);
            time += Time.deltaTime;
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
