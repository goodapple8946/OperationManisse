using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spring : MonoBehaviour
{
	public GameObject obj1;
	public GameObject obj2;

	// Start is called before the first frame update
	void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
		Vector2 pos1 = obj1.transform.position;
		Vector2 pos2 = obj2.transform.position;
		float scaleRatio = Vector2.Distance(pos1, pos2) / 0.6f;
		float angle = Vector2.SignedAngle(Vector2.right, pos2 - pos1);

		transform.localEulerAngles = new Vector3(0, 0, angle);
		transform.position = (pos1 + pos2) / 2;
		transform.localScale = new Vector3(scaleRatio, 1, 1);

    }
}
