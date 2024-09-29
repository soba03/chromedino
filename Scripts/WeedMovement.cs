using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class weedMovement : MonoBehaviour
{
    [SerializeField] public GameObject warningIcon;

    private GameObject warningObject = null;
    // Start is called before the first frame update
    void Start()
    {
        warningObject = Instantiate(warningIcon, new Vector3(-9.5f, 2.6f, 0), Quaternion.identity);
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.position.x > -10f)
        {
            Destroy(warningObject);
        }
    }
}
