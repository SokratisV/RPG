﻿using UnityEngine;

namespace RPG.Core
{
    public class CameraFacing : MonoBehaviour
    {
        Transform mainCamera;

        private void Start()
        {
            mainCamera = Camera.main.transform;
        }
        private void Update()
        {
            transform.forward = mainCamera.forward;
        }
    }
}