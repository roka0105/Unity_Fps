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
	MonsterManager M_Monster => MonsterManager.Instance;
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
		Collider[] cols = Physics.OverlapSphere(transform.position, m_Range, 1 << 8);
		if (cols.Length > 0)
		{
			for (int i = 0; i < cols.Length; ++i)
			{
				if (cols[i].tag == "Player")
				{
					target = cols[i].gameObject.transform;
				}
			}
		}
		else target = null;

		if(target!=null)
		{
			Vector3 dir = target.position - transform.position;
			transform.Translate(dir.normalized * m_Speed * Time.deltaTime);
		}
	}
	private void OnCollisionEnter(Collision collision)
	{
		switch(collision.gameObject.layer)
		{
			case (int)E_Layer.Block:
				break;
			case (int)E_Layer.CrushBlock:
				break;
			case (int)E_Layer.Player:
				break;
			case (int)E_Layer.PlayerBullet:
				--m_Hp;
				if(m_Hp<=0)
				{
					M_Monster.DeSpawn(this);
				}
				Debug.Log($"Monster HP:{m_Hp}");
				break;
		}
	}
	private void Update()
	{
		
		FollowMove();
		
	}
}
