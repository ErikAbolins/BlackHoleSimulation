using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    public Transform camera;
    public float speed = 5f;
    public float mouseSensitivity = 2f;
    
    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");

        Vector3 move = transform.right * moveX + transform.forward * moveZ;

        if (Input.GetKey(KeyCode.Space))
        {
            move.y = 1f;
        }
        else if (Input.GetKey(KeyCode.LeftShift))
        {
            move.y = -1f;
        }
        else
        {
            move.y = 0f;
        }

        transform.position += move * speed * Time.deltaTime;

        Camera.main.transform.Rotate(Vector3.up * Input.GetAxis("Mouse X") * mouseSensitivity);
        Camera.main.transform.Rotate(Vector3.right * -Input.GetAxis("Mouse Y") * mouseSensitivity);
    }
}
