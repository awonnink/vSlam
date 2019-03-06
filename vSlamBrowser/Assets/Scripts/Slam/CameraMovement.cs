using UnityEngine;
using System.Collections;

public class CameraMovement : MonoBehaviour {

	private float speed = 1.0f;
	private float zoomSpeed = 2.0f;

	public float minX = -360.0f;
	public float maxX = 360.0f;
	
	public float minY = -45.0f;
	public float maxY = 45.0f;

	public float sensX = 100.0f;
	public float sensY = 130.0f;
	
	float rotationY = 0.0f;
	float rotationX = 0.0f;
    public float deltaY = 0;
    Vector3 defaultEulerAngles = Vector3.zero;
    public bool evaluateMouse = true;

    float rotateY = 0;


    float hDis = 1;
    public void ReInit()
    {
        rotationY = 0.0f;
        rotationX = 0.0f;
        deltaY = 0;
    }
    private void Start()
    {
        defaultEulerAngles = transform.eulerAngles;
    }
    void Update () {
 
        if (evaluateMouse)
        {
            float scroll = -Input.GetAxis("Mouse ScrollWheel");
            if (Slam.Slam.Instance.AllowScroll)
            {
                transform.Translate(transform.forward * scroll * zoomSpeed, Space.World);
            }

            if (Slam.Slam.Instance != null && Slam.Slam.Instance.menu != null && !Slam.Slam.Instance.menu.activeSelf)
            {
                if (Input.GetKey(KeyCode.RightArrow))
                {
                    transform.position += transform.right * speed * Time.deltaTime;
                }
                if (Input.GetKey(KeyCode.LeftArrow))
                {
                    transform.position -= transform.right * speed * Time.deltaTime;
                }
                if (Input.GetKey(KeyCode.UpArrow))
                {
                    transform.position += transform.forward * speed * Time.deltaTime;
                }
                if (Input.GetKey(KeyCode.DownArrow))
                {
                    transform.position -= transform.forward * speed * Time.deltaTime;
                }
                if (Input.GetKey(KeyCode.A))
                {
                    rotationX -= sensX * 0.05f * Time.deltaTime;
                    transform.localEulerAngles = defaultEulerAngles + 3 * new Vector3(-rotationY, rotationX, 0);
                }
                if (Input.GetKey(KeyCode.D))
                {
                    rotationX = sensX * 0.05f * Time.deltaTime;
                    transform.localEulerAngles = defaultEulerAngles + 3 * new Vector3(-rotationY, rotationX, 0);
                }
                if (Input.GetKey(KeyCode.W))
                {
                    rotationY = 0.05f * sensY * Time.deltaTime;
                    rotationY = Mathf.Clamp(rotationY, minY, maxY);
                    transform.localEulerAngles = defaultEulerAngles + 3 * new Vector3(-rotationY, rotationX, 0);
                }
                if (Input.GetKey(KeyCode.S))
                {
                    rotationY -= 0.05f * sensY * Time.deltaTime;
                    rotationY = Mathf.Clamp(rotationY, minY, maxY);
                    transform.localEulerAngles = defaultEulerAngles + 3 * new Vector3(-rotationY, rotationX, 0);
                }

            }
            if (Input.GetMouseButton(1) )
            {
                rotationX -= Input.GetAxis("Mouse X") * sensX * Time.deltaTime;
                rotationY -= Input.GetAxis("Mouse Y") * sensY * Time.deltaTime;
                rotationY = Mathf.Clamp(rotationY, minY, maxY);
                transform.localEulerAngles = defaultEulerAngles + 1 * new Vector3(-rotationY, rotationX, 0);
            }
            else if (Mathf.Abs(deltaY) > 1.2f)
            {
                Debug.Log("Trying to rotate 0");
                var step = deltaY > 0 ? -0.15f : 0.15f;
                deltaY += step;
                rotationX -= step;
                transform.localEulerAngles = defaultEulerAngles + 3 * new Vector3(-rotationY, rotationX, 0);
            }
            else
            {
                rotationX = 0;
                rotationY = 0;
                defaultEulerAngles = new Vector3(transform.localEulerAngles.x, transform.localEulerAngles.y, 0);
            }
        }
        else if (Mathf.Abs(deltaY) > 1.2f)
        {
            //var step = deltaY > 0 ? -0.15f : 0.15f;
            //deltaY += step;
            //transform.RotateAround(Camera.main.transform.position, Vector3.up, -step);
            //transform.localEulerAngles = defaultEulerAngles + 3 * new Vector3(-rotationY, rotationX, 0);
            Debug.Log("Trying to rotate 1");
            transform.RotateAround(Camera.main.transform.position, Vector3.up, deltaY);
            deltaY = 0;

        }
        if(rotateY!=0)
        {
            float speedCorr = 1;
            if (Slam.Slam.Instance.IsHoloLens() && Slam.Slam.Instance.sceneRotator != null)
            {
                rotateY = Mathf.Abs(rotateY);
                var camPos = Camera.main.transform.position;
                var scenerotatorPos = Slam.Slam.Instance.sceneRotator.transform.position;
                var currPos = camPos + Camera.main.transform.forward * hDis;
                var v1 = camPos - scenerotatorPos;
                var v2 = camPos - currPos;
                v1 = new Vector3(v1.x, 0, v1.z);
                v2 = new Vector3(v2.x, 0, v2.z);


                if (v1.magnitude > 0 && v2.magnitude > 0)
                {
                    v1 = Vector3.Normalize(v1);
                    v2 = Vector3.Normalize(v2);

                    var cosAngle = Vector3.Dot(v1, v2);
                    var handedness = Mathf.Sign(Vector3.Dot(Vector3.Cross(Vector3.up, v1), v2));
                    var angle = Mathf.Acos(cosAngle);
                   // Debug.Log(string.Format("v1: {0}, v2: {1}, cossAngle: {2}, angle: {3}", v1, v2, cosAngle, angle));
                    if (angle < 0.1f)
                    {
                        angle = 0;
                    }
                    speedCorr = handedness * 8 * angle;
                }
            }
            if (speedCorr != 0)
            {
                var cangle = speedCorr * rotateY * Time.deltaTime;
                Debug.Log(string.Format("cangle: {0}", cangle));
                if (Mathf.Abs(cangle) < 3f)
                {
                    transform.RotateAround(Camera.main.transform.position, Vector3.up, cangle);
                }
            }
        }
    }
    public bool MouseScreenCheck()
    {
#if UNITY_EDITOR
        if (Input.mousePosition.x == 0 || Input.mousePosition.y == 0 || Input.mousePosition.x >= UnityEditor.Handles.GetMainGameViewSize().x - 1 || Input.mousePosition.y >= UnityEditor.Handles.GetMainGameViewSize().y - 1)
        {
            return false;
        }
#else
        if (Input.mousePosition.x == 0 || Input.mousePosition.y == 0 || Input.mousePosition.x >= Screen.width - 1 || Input.mousePosition.y >= Screen.height - 1) {
        return false;
        }
#endif
        else
        {
            return true;
        }
    }
    public void Rotate(float angle, float dis)
    {
        Debug.Log("Trying to rotate 2");
        hDis = dis;
        rotateY = angle;
       // transform.RotateAround(Camera.main.transform.position, Vector3.up, angle);
    }

 
}
