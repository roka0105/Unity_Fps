using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block : MonoBehaviour
{
	
	float m_HP;
	int m_Type;
	int m_Position;
	

	protected BlockManager M_Block => BlockManager.Instance;
	protected MapManager M_Map => MapManager.Instance;

	public int P_Type
	{
		get{ return m_Type; }
		set{ m_Type = value; }
	}
	public int P_Position
	{
		get { return m_Position;}
		set { m_Position = value; }
	}
	public float P_HP
	{
		get { return m_HP; }
		set { m_HP = value; }
	}
	public void Init(StageData stagedata)
	{
		m_HP = stagedata.m_BlockData.m_Hp;
	}
	void DecreaseHP(float dmg)
	{
		m_HP -= dmg;
		Debug.Log($"HP:{m_HP}");
		return;
	}
	private void Awake()
	{
		Bullet.OnCrush += DecreaseHP;
		m_Type = -1;
		m_Position = -1;
	}
	//void 
}
