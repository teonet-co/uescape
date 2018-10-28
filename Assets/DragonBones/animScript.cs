using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DragonBones;

public class animScript : MonoBehaviour {

    void Start()
    {
        UnityFactory.factory.LoadDragonBonesData("Ubbie/Ubbie_ske"); // DragonBones file path (without suffix)
        UnityFactory.factory.LoadTextureAtlasData("Ubbie/Ubbie_tex"); //Texture atlas file path (without suffix) 

        // Create armature.
        var armatureComponent = UnityFactory.factory.BuildArmatureComponent("ubbie");
        // Input armature name
        
        // Play animation.
        armatureComponent.animation.Play("walk");

        // Change armatureposition.
        armatureComponent.transform.localPosition = new Vector3(0.0f, 0.0f, 0.0f);
    }

    // Update is called once per frame
    void Update () {
		
	}
}
