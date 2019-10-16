using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class Sketch_Post : MonoBehaviour {

	public Material material;

  	void OnRenderImage (RenderTexture source, RenderTexture destination) {
		material.color = new Color(transform.forward.x,transform.forward.y,transform.forward.z,1f);
    	Graphics.Blit(source, destination, material);
  	}
}
