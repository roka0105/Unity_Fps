using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : Singleton<CameraManager>
{
	protected PlayerManager M_Player => PlayerManager.Instance;
	protected BulletManager M_Bullet => BulletManager.Instance;
	
	bool movecheck;
	Vector2 arrival;
	public Vector3 GetPos()
	{
		Vector3 temp = M_Player.GetPos();
		return temp;
	}
	public Vector3 Arrival
    {
		get { return arrival; }
        set { arrival = value; }
    }
	//public Vector3 Shot
	public bool Movecheck
	{
		get { return movecheck; }
		set { movecheck = value; }
	}

}
