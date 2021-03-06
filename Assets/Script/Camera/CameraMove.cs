using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMove : MonoBehaviour
{
	public enum E_CameraType
	{
		None=-1,
		Main,
		Minimap,
		Max,
	}
	E_CameraType type;
	float Distance;
	Camera camera;
	protected CameraManager M_Camera => CameraManager.Instance;
	protected PlayerManager M_Player => PlayerManager.Instance;
	protected BulletManager M_Bullet => BulletManager.Instance;
	protected _GameManager M_GAME => _GameManager.Instance;
	void SetPos(E_CameraType _type)
	{
		Vector3 temp = M_Camera.GetPos();
	   switch(_type)
		{
			case E_CameraType.Main:
				Distance = -0.5f;
				//temp.z -= Distance;
				camera.nearClipPlane = 0.3f;
				break; 
			case E_CameraType.Minimap:
				Distance = 100f;
				temp.y += Distance;
				break;
		}
		this.transform.position = temp;
		
	}
	void Start()
	{
		if (this.tag == "MainCamera")
		{
			type = E_CameraType.Main;
		}
		else type = E_CameraType.Minimap;
		camera = Camera.main.GetComponent<Camera>();
	}
	private float xRotate = 0.0f;
	public void ShotRaycast()
    {
		RaycastHit hitinfo;
		GunData gundata = M_GAME.GetGunData(0);
		float range = gundata.m_AtkRange;
		int mask = ~(1 << (int)E_Layer.Player);

		Debug.DrawRay(transform.position,transform.forward * range, Color.green, float.PositiveInfinity);
		if (Physics.Raycast(transform.position,transform.forward, out hitinfo, range, mask))
		{
			M_Camera.Arrival = hitinfo.point;
		}
	}
	void MouseRotation()
	{
		// 좌우로 움직인 마우스의 이동량 * 속도에 따라 카메라가 좌우로 회전할 양 계산
		float yRotateSize = Input.GetAxis("Mouse X") * 3f;
		// 현재 y축 회전값에 더한 새로운 회전각도 계산
		float yRotate = transform.eulerAngles.y + yRotateSize;
		
		// 위아래로 움직인 마우스의 이동량 * 속도에 따라 카메라가 회전할 양 계산(하늘, 바닥을 바라보는 동작)
		float xRotateSize = -Input.GetAxis("Mouse Y") * 3f;
		// 위아래 회전량을 더해주지만 -45도 ~ 80도로 제한 (-45:하늘방향, 80:바닥방향)
		// Clamp 는 값의 범위를 제한하는 함수
		xRotate = Mathf.Clamp(xRotate + xRotateSize, -45, 80);
		M_Player.SetNozzleRotation(new Vector3(xRotate, yRotate, 0));
		M_Player.GetObjPlayer.transform.localEulerAngles = new Vector3(0f, yRotate,0);
		// 카메라 회전량을 카메라에 반영(X, Y축만 회전)
		transform.eulerAngles = new Vector3(xRotate, yRotate, 0);
	}
	void Update()
	{
		if (M_Camera.Movecheck)
		{
			SetPos(type);
			if (type != E_CameraType.Minimap)
			{
				MouseRotation();
			}
		}
	}
}
