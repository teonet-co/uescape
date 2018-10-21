﻿using System;
using UnityEngine;
using UnityEditor;
[CreateAssetMenu(fileName = "Readme", menuName = "Inventory/List", order = 1)]
public class Readme : ScriptableObject {
	public Texture2D icon;
	public string title;
	public Section[] sections;
	public bool loadedLayout;
	
	[Serializable]
	public class Section {
		public string heading, text, linkText, url;

	}
    public void InsertText()
    {
        sections = new Section[1];
        sections[0] = new Section
        {
            heading = "asd",
            linkText = "asd link",
            text = "asd text",
            url = "www.google.com"
        };
        title = "ReadMe please";
        //var ids = AssetDatabase.FindAssets("");
        string[] results1 = AssetDatabase.FindAssets("LOGO_NEW");
        string textutrePath = AssetDatabase.GUIDToAssetPath(results1[0]);
        icon = (Texture2D)AssetDatabase.LoadAssetAtPath(textutrePath, typeof(Texture2D));
    }
}
