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
	void MouseRotation()
	{
		// �¿�� ������ ���콺�� �̵��� * �ӵ��� ���� ī�޶� �¿�� ȸ���� �� ���
		float yRotateSize = Input.GetAxis("Mouse X") * 3f;
		// ���� y�� ȸ������ ���� ���ο� ȸ������ ���
		float yRotate = transform.eulerAngles.y + yRotateSize;
		
		// ���Ʒ��� ������ ���콺�� �̵��� * �ӵ��� ���� ī�޶� ȸ���� �� ���(�ϴ�, �ٴ��� �ٶ󺸴� ����)
		float xRotateSize = -Input.GetAxis("Mouse Y") * 3f;
		// ���Ʒ� ȸ������ ���������� -45�� ~ 80���� ���� (-45:�ϴù���, 80:�ٴڹ���)
		// Clamp �� ���� ������ �����ϴ� �Լ�
		xRotate = Mathf.Clamp(xRotate + xRotateSize, -45, 80);

		M_Player.GetObjPlayer.transform.localEulerAngles = new Vector3(0f, yRotate,0);
		// ī�޶� ȸ������ ī�޶� �ݿ�(X, Y�ุ ȸ��)
		transform.eulerAngles = new Vector3(xRotate, yRotate, 0);
	}
	void Update()
	{
		if(M_Camera.Movecheck)
		{
			SetPos(type);
			if (type != E_CameraType.Minimap)
				MouseRotation();
		}
	}
}
