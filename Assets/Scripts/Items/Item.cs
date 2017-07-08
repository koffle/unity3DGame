using UnityEngine;
using System.Collections;
using System.IO;

using System.Collections.Generic;
public class Item{
	public int id;
	public string name;
	public string desc;
	public Texture2D image;
	public string spriteName;

	public Item(){
	}
	public Item(DataRow dr){
		id = (System.Int32)dr["id"];
		name = (System.String)dr["name"];
		desc = (System.String)dr["description"];
		image = new Texture2D(0,0);
		image.LoadImage((System.Byte[])dr["image"]);
		spriteName = (System.String)dr["spriteName"];
	}
}