using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Singleton<Player>
{
	PlayerManager M_Player => PlayerManager.Instance;
	Vector3 PlayerPos;
	float Speed;
	float Scale;

	public Vector3 GetPlayerPos
	{
		get { return this.transform.position; }
	}
	public bool ActiveSelf
	{
		get
		{
			return this.gameObject.activeSelf;
		}
	}
	public void SetPlayerPos(float x, float y, float z)
	{
		PlayerPos = this.transform.position;
		PlayerPos.x = x;
		PlayerPos.y = y;
		PlayerPos.z = z;
		this.transform.position = PlayerPos;
		//transform.localposition = PlayerPos;
	}
	//dir 0:x 1:z
	float RayCast(int dir, float number)
	{
		RaycastHit hitinfo;
		Vector3 temp = this.transform.position;
		float radius = Scale / 2;
		int mask = 1 << 7;
		bool[] is_doorcheck = new bool[2];
		Block crush_collider = null;
		switch (dir)
		{   //x
			case 0:
				if (number > 0)
				{
					#region 문이 열린지 체크
					RaycastHit[] hitlist = Physics.RaycastAll(temp, transform.right, number + radius );
					for (int i=0;i<hitlist.Length;++i)
					{
						if (hitlist[i].transform.gameObject.layer == (int)E_Layer.CrushBlock)
						{
							is_doorcheck[0] = true;
							crush_collider = hitlist[i].transform.GetComponent<Block>();
						}
	     //               if (hitlist[i].transform.gameObject.layer==(int)E_Layer.Block)
						//{
						//	is_doorcheck[1]= true;
						//}
					
					}
					if(is_doorcheck[0])//&&is_doorcheck[1])
					{
						if(!crush_collider.ColliderItem[0].GetComponentInChildren<MeshRenderer>().enabled)
						{
							return number;
						}
					}
					#endregion
					if (Physics.Raycast(temp, transform.right, out hitinfo, number + radius, mask))
					{ 
						//temp.x = temp.x + number - hitinfo.point.x;
						temp.x = hitinfo.point.x - (temp.x + radius);
						return temp.x;
					}
				}
				else if (number < 0)
				{
					#region 문이 열린지 체크
					RaycastHit[] hitlist = Physics.RaycastAll(temp, -(transform.right), number + radius * 2);
				
					for (int i = 0; i < hitlist.Length; ++i)
					{
						if (hitlist[i].transform.gameObject.layer == (int)E_Layer.CrushBlock)
						{
							is_doorcheck[0] = true;
							crush_collider = hitlist[i].transform.GetComponent<Block>();
						}
					
					}
					if (is_doorcheck[0])
					{
						if (!crush_collider.ColliderItem[0].GetComponentInChildren<MeshRenderer>().enabled)
						{
							return number;
						}
					}
					#endregion

					if (Physics.Raycast(temp, -(transform.right), out hitinfo, -number + radius, mask))
					{
						Debug.DrawRay(temp, transform.right * (number - radius), Color.blue, float.PositiveInfinity);
						//temp.x = temp.x - number + hitinfo.point.x;
						temp.x = hitinfo.point.x - (temp.x - radius);
						return temp.x;
					}
				}
				return number;
				break;
			//z
			case 1:
				if (number > 0)
				{
					#region 문이 열린지 체크
					RaycastHit[] hitlist = Physics.RaycastAll(temp, transform.forward, number + radius*2);
					
					for (int i = 0; i < hitlist.Length; ++i)
					{
						if (hitlist[i].transform.gameObject.layer == (int)E_Layer.CrushBlock)
						{
							is_doorcheck[0] = true;
							crush_collider = hitlist[i].transform.GetComponent<Block>();
						}
						
					}
					if (is_doorcheck[0])
					{
						if (!crush_collider.ColliderItem[0].GetComponentInChildren<MeshRenderer>().enabled)
						{
							return number;
						}
					}
					#endregion
					if (Physics.Raycast(temp, transform.forward, out hitinfo, number + radius, mask))
					{
						Debug.DrawRay(temp, transform.forward * (number + radius), Color.blue, float.PositiveInfinity);
						//temp.x = temp.x + number - hitinfo.point.x;
						temp.z = hitinfo.point.z - (temp.z + radius);
						return temp.z;
					}
				}
				else if (number < 0)
				{
					#region 문이 열린지 체크
					RaycastHit[] hitlist = Physics.RaycastAll(temp, -(transform.forward), number + radius * 2);
					for (int i = 0; i < hitlist.Length; ++i)
					{
						if (hitlist[i].transform.gameObject.layer == (int)E_Layer.CrushBlock)
						{
							is_doorcheck[0] = true;
							crush_collider = hitlist[i].transform.GetComponent<Block>();
						}
					}
					if (is_doorcheck[0])
					{
						if (!crush_collider.ColliderItem[0].GetComponentInChildren<MeshRenderer>().enabled)
						{
							return number;
						}
					}
					#endregion
					if (Physics.Raycast(temp, -(transform.forward), out hitinfo, -number + radius, mask))
					{
						Debug.DrawRay(temp, transform.forward * (number - radius), Color.blue, float.PositiveInfinity);
						//temp.x = temp.x - number + hitinfo.point.x;
						temp.z = hitinfo.point.z - (temp.z - radius);
						return temp.z;
					}
				}
				return number;
				break;
		}
		return 0;
	}
	public void Move()
	{
		PlayerPos = new Vector3(0, 0, 0);
		Vector3 moveAfterpt = new Vector3(0, 0, 0);
		moveAfterpt.x = Input.GetAxis("Horizontal") * Speed * Time.deltaTime;

		moveAfterpt.z = Input.GetAxis("Vertical") * Speed * Time.deltaTime;

		if (moveAfterpt.x != 0)
			moveAfterpt.x = RayCast(0, moveAfterpt.x);
		if (moveAfterpt.z != 0)
			moveAfterpt.z = RayCast(1, moveAfterpt.z);
		if (moveAfterpt.x != 0 || moveAfterpt.z != 0)
		{
			PlayerPos = transform.forward * moveAfterpt.z + transform.right * moveAfterpt.x;
		}
		PlayerPos.y = 0;
		this.transform.position += PlayerPos;
	}
	public void Shot()
	{
		if(Input.GetMouseButtonDown(0))
		{  //총매니저에서 가져온 현재 장착한 총 정보(총 타입, 한번 쏠때 몇개씩 나가는지)를 넣음
		   //아직 구현 안해서 기본값으로 진행
			M_Player.ShotBullet((int)E_BulletType.Nomal, 1);
		}
	}

	void Start()
	{
		PlayerPos = this.transform.position;
		Speed = 3f;
		Scale = this.transform.localScale.x;
	}
	void Update()
	{
		Move();
		Shot();
	}
}
