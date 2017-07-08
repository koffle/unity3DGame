using UnityEngine;
using System.Collections;

[System.Serializable]
public class Player {
	public NetworkPlayer pl;
	public float health = 100;
	public float energy = 5;
	public bool firing = false;
	public bool aiming = false;
    public bool moving = false;
	public PlayerController Manager;
	public string PlayerName;
	public NetworkPlayer OnlinePlayer;
	public Texture2D [] PlayerItemSprites;
	public Texture2D PlayerSprite;
	public int[] PlayerItems = new int[] { 2, 2, 0, 2, 2, 2 };
	//public int[] PlayerItems = new int[] { 1, 1, 2, 1, 1, 1 };
}