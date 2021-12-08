using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block : MonoBehaviour, Dmginterface
{

	float m_HP;
	int m_Type;
	int m_Position;
	Block _this;
	List<Block> ActiveCrushblock;
	bool is_DoorOpen;
	string _this_name;

	protected BlockManager M_Block => BlockManager.Instance;
	protected MapManager M_Map => MapManager.Instance;

	public bool IS_DoorOpen
	{
		get { return is_DoorOpen; }
		set { is_DoorOpen = value; }
	}
	public List<Block> ColliderItem
	{
		set { ActiveCrushblock = value; }
		get { return ActiveCrushblock; }
	}
	public int P_Type
	{
		get { return m_Type; }
		set { m_Type = value; }
	}
	public int P_Position
	{
		get { return m_Position; }
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
	private void Awake()
	{
		//Bullet.OnCrush += DecreaseHP;
		ActiveCrushblock = new List<Block>();
		_this = gameObject.GetComponent<Block>();
		m_Type = -1;
		m_Position = -1;
		_this_name = gameObject.name;
	}
	void Dmginterface.Dmg(float dmg)
	{
		
		m_HP -= dmg;
		if ((m_HP == 0f || m_HP < 0f) && _this_name == "CrushCollider")
		{
			for (int i = 0; i < ActiveCrushblock.Count; ++i)
			{
				MeshRenderer[] child = new MeshRenderer[4];
				child = ActiveCrushblock[i].GetComponentsInChildren<MeshRenderer>();
				//M_Block.DeSpawn(2, ActiveCrushblock[i]);
				for (int j = 0; j < 4; ++j)
				{
					is_DoorOpen = true;
					child[j].enabled = false;
				}
			}
			//ActiveCrushblock.Clear();
			//Destroy(_this);//현재 오브젝트:crush block의 맵 콜라이더 이면 콜라이더 삭제;
			return;
		}
		Debug.Log($"HP:{m_HP}");
	}
	private void Update()
	{

	}
}
