using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum E_BulletType
{
	None=-1,
	Nomal,
	Max,
}
public class BulletManager : Singleton<BulletManager>
{
	protected _GameManager M_Game => _GameManager.Instance;
	protected PlayerManager M_Player => PlayerManager.Instance;
	protected CameraManager M_Camera => CameraManager.Instance;
	protected BlockManager M_Block=>BlockManager.Instance;

	Dictionary<E_BulletType, MemoryPool<Bullet>> BulletPoollist;
	Transform BulletParent;
	GunData Gundata;

	public void Init()
	{
		BulletPoollist = new Dictionary<E_BulletType, MemoryPool<Bullet>>();
		BulletParent = GameObject.Find("BulletPool").GetComponent<Transform>();
		for (int i = 0; i < M_Game.m_GunTypeCount; ++i)
		{
			BulletPoollist.Add((E_BulletType)i, new MemoryPool<Bullet>(M_Game.BulletPrefeblist[i], 100, BulletParent));
		}
	}
	public void Awake()
	{
		//delegate_crush_block+=M_
	}
	public Bullet Spawn(int type,int count=1)
	{
		Bullet item = null;
		switch((E_BulletType)type)
		{
			case E_BulletType.Nomal:
				for (int i = 0; i < count; ++i)
				{
					item = BulletPoollist[E_BulletType.Nomal].Spawn();
					Gundata = M_Game.GetGunData((int)E_BulletType.Nomal);
					item.Init(Gundata);
					item.gameObject.SetActive(true);
					//원래는 총 구현 후 총 노즐에서 나와야 하는데 총알 먼저 구현중이라 플레이어 위치에서 쏴지게 함.
					item.transform.position = M_Player.GetObjPlayer.transform.position;
					item.SetStartPos = item.transform.position;
					//item.GetBullet;
					item.SetDir=M_Player.GetObjPlayer.transform.forward;
					if (item.name != "Bullet")
						item.name = "Bullet";
				}
				break;
		}
		return item;
	}
	public void DeSpawn(int type,Bullet obj)
	{
		switch((E_BulletType)type)
		{
			case E_BulletType.Nomal:
				BulletPoollist[E_BulletType.Nomal].DeSpawn(obj);
				break;
		}
	}
}
