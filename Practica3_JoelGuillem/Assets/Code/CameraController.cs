using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform m_LookAt;
    public float m_MinDistance = 4f;
    public float m_MaxDistance = 7f;
    public float m_RotationalYawSpeed;
    public float m_RotationalPitchSpeed;
    public float m_MinPitch;
    public float m_MaxPitch;
    public LayerMask m_AvoidObstaclesLayerMask;
    public float m_OffsetAvoidObstacle;

    [Header("Camera Reset")]
    public float m_ResetPitch;
    public float m_TimeToResetCamera;
    float m_ResetCamTimer;

    private void LateUpdate()
    {

        transform.LookAt(m_LookAt.position);
        float l_Distance = Vector3.Distance(transform.position, m_LookAt.position);
        Vector3 l_EulerAngles = transform.rotation.eulerAngles;
        float l_Yaw = l_EulerAngles.y * Mathf.Deg2Rad;
        float l_Pitch = l_EulerAngles.x * Mathf.Deg2Rad;
        if (l_Pitch > Mathf.PI)
            l_Pitch -= 2f * Mathf.PI;

        float l_MouseX = Input.GetAxis("Mouse X");
        float l_MouseY = Input.GetAxis("Mouse Y");

        l_Yaw = l_Yaw + l_MouseX * (m_RotationalYawSpeed * Mathf.Deg2Rad) * Time.deltaTime;
        l_Pitch = l_Pitch + l_MouseY * (m_RotationalPitchSpeed * Mathf.Deg2Rad) * Time.deltaTime;
        l_Pitch = Mathf.Clamp(l_Pitch, m_MinPitch * Mathf.Deg2Rad, m_MaxPitch * Mathf.Deg2Rad);
        Vector3 l_Forward = new Vector3(Mathf.Sin(l_Yaw) * Mathf.Cos(-l_Pitch), Mathf.Sin(-l_Pitch), Mathf.Cos(l_Yaw) * Mathf.Cos(-l_Pitch));
        l_Distance = Mathf.Clamp(l_Distance, m_MinDistance, m_MaxDistance);
        Vector3 l_DesiredPosition = m_LookAt.position - l_Forward * l_Distance;

        Ray l_Ray = new Ray(m_LookAt.position, -l_Forward);
        RaycastHit l_RaycastHit;
        if (Physics.Raycast(l_Ray, out l_RaycastHit, l_Distance, m_AvoidObstaclesLayerMask.value))
        {
            l_DesiredPosition = l_RaycastHit.point + l_Forward * m_OffsetAvoidObstacle;
        }

        transform.position = l_DesiredPosition;
        transform.LookAt(m_LookAt.position);

        if (!Input.anyKey && Input.GetAxis("Mouse X") == 0 && Input.GetAxis("Mouse Y") == 0)
        {
            Debug.Log(m_ResetCamTimer);
            m_ResetCamTimer += Time.deltaTime;
            if (m_ResetCamTimer > m_TimeToResetCamera)
            {
                float l_rPitch = m_ResetPitch * Mathf.Deg2Rad;
                float l_rYaw = GameObject.FindGameObjectWithTag("Player").transform.rotation.eulerAngles.y * Mathf.Deg2Rad;

                float l_rDistance = Vector3.Distance(transform.position, m_LookAt.position);
                Vector3 l_rForward = new Vector3(Mathf.Sin(l_rYaw) * Mathf.Cos(-l_rPitch), Mathf.Sin(-l_rPitch), Mathf.Cos(l_rYaw) * Mathf.Cos(-l_rPitch));
                l_rDistance = Mathf.Clamp(l_rDistance, m_MinDistance, m_MaxDistance);
                Vector3 l_rDesiredPosition = m_LookAt.position - l_rForward * l_rDistance;

                Ray l_rRay = new Ray(m_LookAt.position, -l_rForward);
                RaycastHit l_rRaycastHit;
                if (Physics.Raycast(l_rRay, out l_rRaycastHit, l_rDistance, m_AvoidObstaclesLayerMask.value))
                {
                    l_rDesiredPosition = l_rRaycastHit.point + l_rForward * m_OffsetAvoidObstacle;
                }

                transform.position = l_rDesiredPosition;
                transform.LookAt(m_LookAt.position);

                m_ResetCamTimer = 0;
            }
        }
        else
        {
            m_ResetCamTimer = 0;
        }

    }
}
