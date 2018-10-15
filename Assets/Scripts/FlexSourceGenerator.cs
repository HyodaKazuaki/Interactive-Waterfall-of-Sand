using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlexSourceGenerator : MonoBehaviour {

    public GameObject prefabFlexSourceActor;
    public float distance = 0.5f;
    public Vector3 startPosition = new Vector3(-12.0f, 9.45f, 2.06f);
    public RandomRange randomRange = new RandomRange(-1.0f, 1.0f);
    public int count = 50;

	// Use this for initialization
	void Start () {
		for(int i = 0; i < count; i++)
        {
            GameObject obj = Instantiate(prefabFlexSourceActor, new Vector3(startPosition.x + distance * i, startPosition.y + Random.Range(randomRange.Min, randomRange.Max), startPosition.z), Quaternion.identity);
            obj.transform.parent = transform;
        }
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}

public class RandomRange {
    private float _min;
    private float _max;

    public RandomRange(float min, float max)
    {
        _min = (min < max) ? min : max;
        _max = (min < max) ? max : min;
    }

    public float Min
    {
        set { _min = (value < _max) ? value : _min; }
        get { return _min; }
    }

    public float Max
    {
        set { _max = (value > _min) ? value : _max;  }
        get { return _max; }
    }
}