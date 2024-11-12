using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraBound : MonoBehaviour
{


   
        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                var mainCamera = Camera.main;
                if (mainCamera == null)
                {
                    Debug.LogError("Main Camera not found.");
                    return;
                }

                var cinemachineBrain = mainCamera.GetComponent<CinemachineBrain>();
                if (cinemachineBrain == null)
                {
                    Debug.LogError("CinemachineBrain component not found on the main camera.");
                    return;
                }

                if (cinemachineBrain.ActiveVirtualCamera == null)
                {
                   // Debug.LogError("No active virtual camera.");
                    return;
                }

                var confiner = cinemachineBrain.ActiveVirtualCamera.VirtualCameraGameObject.GetComponent<CinemachineConfiner>();
                if (confiner == null)
                {
                    Debug.LogError("CinemachineConfiner component not found on the active virtual camera.");
                    return;
                }

                var boxCollider = GetComponent<BoxCollider>();
                if (boxCollider == null)
                {
                    Debug.LogError("BoxCollider component not found on the GameObject this script is attached to.");
                    return;
                }

                confiner.m_BoundingVolume = boxCollider;
            }
        }
    }

