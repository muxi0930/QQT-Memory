using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireController : MonoBehaviour {
    public Boolean _animatorDone = false;
    // Update is called once per frame
    void Update() {
        if (_animatorDone) {
            Destroy(gameObject);
        }
    }
}
