using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : Singleton<PlayerManager>
{
    Player M_Playerobj => Player.Instance;
    BulletManager M_Bullet => BulletManager.Instance;
   
    public void ShotBullet(int guntype,int count)
	{
        Bullet item=M_Bullet.Spawn(guntype, count);
        item.transform.position= M_Playerobj.NozzleRotation.transform.position;
        item.transform.localEulerAngles = new Vector3(M_Playerobj.NozzleRotation.transform.eulerAngles.x+90, M_Playerobj.transform.eulerAngles.y,0f);
        item.SetDir = item.transform.up;
    }
   
    public bool ActiveSelf()
	{
        return M_Playerobj.ActiveSelf;
	}
    public void SetActivePlayer(bool active)
	{
        M_Playerobj.gameObject.SetActive(active);
	}
    public void SetPos(float x, float y,float z)
	{
        M_Playerobj.SetPlayerPos(x,y,z);
	}
    public Vector3 GetPos()
	{
        return M_Playerobj.GetPlayerPos;
	}
    public GameObject GetObjPlayer
    {
        get { return M_Playerobj.gameObject; }
    }
    public void SetNozzleRotation(Vector3 angle)
	{
        M_Playerobj.NozzleRotation.transform.eulerAngles =angle;
	}
   
}
