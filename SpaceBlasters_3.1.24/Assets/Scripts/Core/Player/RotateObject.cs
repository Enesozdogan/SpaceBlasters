using UnityEngine;

public class RotateObject : MonoBehaviour
{
    [SerializeField] Camera mainCamera;

    // Update is called once per frame
    private void Awake()
    {
        mainCamera = FindObjectOfType<Camera>();
    }
    void Update()
    {
        Vector3 mousepos = Input.mousePosition;
        Vector3 gunposition = mainCamera.WorldToScreenPoint(transform.position);
        mousepos.x = mousepos.x - gunposition.x;
        mousepos.y = mousepos.y - gunposition.y;
        float gunAngle = Mathf.Atan2(mousepos.y, mousepos.x) * Mathf.Rad2Deg;
        if (mainCamera.ScreenToWorldPoint(Input.mousePosition).x < transform.position.x)
        {
            transform.rotation = Quaternion.Euler(new Vector3(180f, 0f, -gunAngle));
            transform.parent.rotation = Quaternion.Euler(new Vector3(0, 180f, 0f));
        }
        else
        {
            transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, gunAngle));
            transform.parent.rotation = Quaternion.Euler(new Vector3(0, 0f, 0f));
        }
    }
}
