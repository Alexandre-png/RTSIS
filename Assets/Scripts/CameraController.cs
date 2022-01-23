using UnityEngine;

public class CameraController : MonoBehaviour
{

    public float panSpeed = 20f;
    public Vector2 panLimit;

    public float scrollSpeed = 20f;
    public float minY = 20f;
    public float maxY = 120f;
    public float rotationSpeed = 50f;

    // Update is called once per frame
    void Update()
    {
        //Déplacement de la caméra à l'aide des touches parametrées
        Vector3 pos = transform.position;
        if(Input.GetKey("z"))
        {
            pos.z += panSpeed * Time.deltaTime;
        }
        if(Input.GetKey("s"))
        {
            pos.z -= panSpeed * Time.deltaTime;
        }
        if(Input.GetKey("q"))
        {
            pos.x -= panSpeed * Time.deltaTime;
        }
        if(Input.GetKey("d"))
        {
            pos.x += panSpeed * Time.deltaTime;
        }

        float scroll = Input.GetAxis("Mouse ScrollWheel");
        pos.y -= scroll * scrollSpeed * 100f * Time.deltaTime;

        pos.x = Mathf.Clamp(pos.x, -panLimit.x, panLimit.x);
        pos.z = Mathf.Clamp(pos.z, -panLimit.y, panLimit.y);
        pos.y = Mathf.Clamp(pos.y, minY, maxY);

        transform.position =pos;

        //Rotation de la caméra à l'aide des touches parametrées
        /*
        if(Input.GetKey("a"))
        {
            transform.Rotate(Vector3.up, -1 * Time.deltaTime * rotationSpeed, Space.World);
        }

        if(Input.GetKey("e"))
        {
            transform.Rotate(Vector3.up, 1 * Time.deltaTime * rotationSpeed, Space.World);
        }
        */
    }
}
