﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Strafer : MinigameBehaviour
{
    public float strafeSpeed = 5;
    public float strafeRange = 10;
    Vector3 startPos;
    static bool running = false;
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        startPos = transform.position;    
    }

    protected override void OnStateEnter() {
        if (!running) running = true;
    }

    protected override void OnStateExit() {
        if (running) running = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (running) {
            transform.position = startPos + transform.right * ((float) Math.Sin((double) (Time.time * strafeSpeed))) * strafeRange;
        }
    }
}
