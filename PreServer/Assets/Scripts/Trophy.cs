using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trophy : MonoBehaviour
{
	// you're winnner
    public bool rotateX;
    public bool rotateY;
    public bool rotateZ;
    public bool scale;
    public Vector3 rotationConstraintMin;
    public Vector3 rotationConstraintMax;
    public Collider col;
    public GameObject trophy;
    float timer = 10f;
    public AudioSource audioSource;
    // Start is called before the first frame update
    void Start()
    {
        current = transform.rotation.eulerAngles;
    }
    Vector3 current;
    // Update is called once per frame
    void Update()
    {
        if (rotateX)
        {
            float oscilationRange = (rotationConstraintMax.x - rotationConstraintMin.x) / 2;
            float oscilationOffset = oscilationRange + rotationConstraintMin.x;
            current.x = oscilationOffset + Mathf.Sin(Time.time) * oscilationRange;
        }
        if (rotateY)
        {
            float oscilationRange = (rotationConstraintMax.y - rotationConstraintMin.y) / 2;
            float oscilationOffset = oscilationRange + rotationConstraintMin.y;
            current.y = oscilationOffset + Mathf.Sin(Time.time) * oscilationRange;
        }
        if (rotateZ)
        {
            float oscilationRange = (rotationConstraintMax.z - rotationConstraintMin.z) / 2;
            float oscilationOffset = oscilationRange + rotationConstraintMin.z;
            current.z = oscilationOffset + Mathf.Sin(Time.time) * oscilationRange;
        }
        if (scale)
        {
            float oscilationRange = (0.75f - 0.5f) / 2;
            float oscilationOffset = oscilationRange + 0.5f;
            transform.localScale = Vector3.one * (oscilationOffset + Mathf.Sin(Time.time) * oscilationRange);
        }
        transform.eulerAngles = current;
        if(!col.enabled)
        {
            if(timer <= 0)
            {
                col.enabled = true;
                trophy.SetActive(true);
            }
            timer -= Time.deltaTime;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.tag == "Player")
        {
            col.enabled = false;
            trophy.SetActive(false);
            timer = 10f;
            audioSource.Play();
        }
    }
}
