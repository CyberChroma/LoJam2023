using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerActivateTactorBeam : MonoBehaviour
{
    public GameObject tractorBeam;

    // Start is called before the first frame update
    void Start()
    {
        tractorBeam.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetAxis("TractorBeam") != 0) {
            tractorBeam.SetActive(true);
        } else {
            tractorBeam.SetActive(false);
        }
    }
}
