using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum E_MonsterType
{
	None = -1,
	Nomal,
	Max
}
public class MonsterManager : Singleton<MonsterManager>
{
	protected _GameManager M_Game = _GameManager.Instance;
	Dictionary<E_MonsterType, MemoryPool<Monster>> MonsterPool;
	Transform Parent;
	public void Awake()
	{
		Parent = GameObject.Find("MonsterPool").GetComponent<Transform>();
		MonsterPool = new Dictionary<E_MonsterType, MemoryPool<Monster>>();

		MonsterPool.Add(E_MonsterType.Nomal, new MemoryPool<Monster>(M_Game.m_MonsterPrefeb, 100, Parent));
	}

	public Monster Spawn(E_MonsterType type)
	{

		switch (type)
		{
			case E_MonsterType.Nomal:
				Monster item = MonsterPool[E_MonsterType.Nomal].Spawn();

				item.name = "Monster";
				item.Init(M_Game.GetStageData().m_EnemyData);
				item.gameObject.SetActive(true);
				return item;
				break;
		}

		return null;
	}
	public void DeSpawn(Monster obj)
	{
		switch (obj.MonsterType)
		{
			case E_MonsterType.Nomal:
				MonsterPool[E_MonsterType.Nomal].DeSpawn(obj);
				break;
		}
	}
}
