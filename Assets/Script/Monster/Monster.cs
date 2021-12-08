using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monster : MonoBehaviour
{  
	[SerializeField]
	E_MonsterType m_type;
	[SerializeField]
	float m_Hp;
	[SerializeField]
	float m_Range;
	[SerializeField]
	float m_Speed;

	bool is_find_player;
	bool is_movepattern;//시간없어서 좌우로만 움직이게.

	Transform target;
	public E_MonsterType MonsterType
	{
		get { return m_type; }
	}
	public void Init(EnemyData enemydata)
	{
		m_Range = enemydata.m_Range;
		m_Hp = enemydata.m_Hp;
		m_Speed = enemydata.m_Speed;
	}
	public void SetPos(float x, float y, float z)
	{
		Vector3 temp = new Vector3(x, y, z);
		this.transform.position = temp;
	}
	void NomalMove()
	{

	}
	void FollowMove()
	{

	}

	private void Update()
	{
		if(is_find_player)
		{
			FollowMove();
		}
		else
		{
			NomalMove();
		}
	}
}
