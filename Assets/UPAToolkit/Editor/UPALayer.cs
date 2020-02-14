using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

[System.Serializable]
public class UPALayer {
	public enum BlendMode {
		NORMAL, MULTIPLY, SCREEN
	};

	public string name;
	public Color[] map;
	public Texture2D tex;
	public bool enabled;
	public float opacity;
	public BlendMode mode;
	public bool locked;
	
	public UPAImage parentImg;

    #region custom variables

    public VectorDictionary colorMapDictionary;


    public VectorColorDictionary originalColorDictionary;
    

    #endregion

    // Constructor
    public UPALayer (UPAImage img) {
		name = "Layer " + (img.layers.Count + 1);
		opacity = 1;
		mode = BlendMode.NORMAL;
		
		map = new Color[img.width * img.height];
		tex = new Texture2D (img.width, img.height);
		
		for (int x = 0; x < img.width; x++) {
			for (int y = 0; y < img.height; y++) {
				map[x + y * img.width] = Color.clear;
				tex.SetPixel (x,y, Color.clear);
			}
		}
		
		tex.filterMode = FilterMode.Point;
		tex.Apply ();
		
		enabled = true;
		locked = false;
		parentImg = img;
		
		// Because Unity won't record map (Color[]) as an undo,
		// we instead register a callback to LoadMapFromTex since undoing textures works fine
		Undo.undoRedoPerformed += LoadMapFromTex; // subscribe to the undo event
	}

	// Create clone of other UPALayer
	public UPALayer(UPALayer original) {
		name = original.name;
		opacity = 1;
		mode = original.mode;

		map = (Color[]) original.map.Clone();
		tex = new Texture2D (original.parentImg.width, original.parentImg.height);

        Texture2D parentTex = original.tex;

        if (parentTex != null)
        {

            //TODO: may be bad to turn this off
            tex.SetPixels(original.tex.GetPixels());

        } else
        {
            Debug.LogError("no parent tex!!");
        }

		tex.filterMode = FilterMode.Point;
		tex.Apply ();
		
		enabled = true;
		locked = original.locked;
		parentImg = original.parentImg;

        colorMapDictionary = original.colorMapDictionary;



		// Because Unity won't record map (Color[]) as an undo,
		// we instead register a callback to LoadMapFromTex since undoing textures works fine
		Undo.undoRedoPerformed += LoadMapFromTex; // subscribe to the undo event
	}
	
	void LoadMapFromTex() {
	
		for (int x = 0; x < parentImg.width; x++) {
			for (int y = 0; y < parentImg.height; y++) {
				map[x + y * parentImg.width] = tex.GetPixel (x, parentImg.height - y - 1);
			}
		}

	}
	
	public Color GetPixel (int x, int y) {
		return tex.GetPixel (x, y);
	}
	
	public void SetPixel (int x, int y, Color color) {
		if (!locked) {
			tex.SetPixel (x, y, color);
			tex.Apply ();
		
			//map [x + y * - 1 * parentImg.width - parentImg.height] = color;
		}
	}
	
	public void LoadTexFromMap () {

        Debug.Log("loading texture from map");
		tex = new Texture2D (parentImg.width, parentImg.height);

		for (int x = 0; x < parentImg.width; x++) {
			for (int y = 0; y < parentImg.height; y++) {
				tex.SetPixel (x, parentImg.height - y - 1, map[x + y * parentImg.width]);
			}
		}
		
		tex.filterMode = FilterMode.Point;
		tex.Apply();
	}
	
	public int GetOrder () {
		return parentImg.layers.IndexOf (this);
	}

    public void setAlpha(bool addAlpha)
    {

        float newAlpha;
        if(addAlpha)
        {
            newAlpha = 0.5f;
        } else
        {
            newAlpha = 1f;
        }

        for(int y = 0; y < parentImg.height; y++)
        {
            for(int x = 0; x < parentImg.width; x++)
            {

                Color color = GetPixel(x, y);

                if (color.a > 0)
                {
                    
                    color.a = newAlpha;
                    SetPixel(x, y, color);
                }

            }
        }

    }

    public void mapPixel(Vector2 templatePixel, Vector2 layerPixel)
    {

        if (colorMapDictionary == null)
        {
            colorMapDictionary = new VectorDictionary();
        }

        if (originalColorDictionary == null)
        {
            originalColorDictionary = new VectorColorDictionary();
        }

        if(colorMapDictionary.ContainsValue(templatePixel)) {

            Vector2 keyToSwap = Vector2.negativeInfinity;

            //We know that this pixel is already mapped to something on this layer
            //so lets go through and find which one it is
            foreach (Vector2 key in colorMapDictionary.Keys)
            {
                if(colorMapDictionary[key] == templatePixel)
                {
                    if(key != layerPixel)
                    {
                        //the pixel we are mapping isn't the same as the already set pixel, so we shall replace these
                        keyToSwap = key;

                    } else
                    {
                        //We are mapping the same pixel, so lets just stop here
                        return;
                    }
                }
            }

            if(keyToSwap != Vector2.negativeInfinity)
            {
                colorMapDictionary.Remove(keyToSwap);
                SetPixel((int)keyToSwap.x, (int)keyToSwap.y, originalColorDictionary[keyToSwap]);

                originalColorDictionary.Remove(keyToSwap);
            }

        }

        
        originalColorDictionary[layerPixel] = GetPixel((int) layerPixel.x, (int) layerPixel.y);

        colorMapDictionary[layerPixel] = templatePixel;



        colorMappedPixels(templatePixel);

    }

    public void colorMappedPixels(Vector2 currentlySelected)
    {

        if (colorMapDictionary != null)
        {

            foreach (Vector2 key in colorMapDictionary.Keys)
            {
                Color color;
                if (colorMapDictionary[key] == currentlySelected)
                {
                    color = UPAColors.SelectedColor;
                }
                else
                {
                    color = UPAColors.MappedColor;
                }

                SetPixel((int)key.x, (int)key.y, color);

            }
        }

    }

    public void setMappedColorsBack()
    {
        if (originalColorDictionary != null)
        {

            foreach (Vector2 key in originalColorDictionary.Keys)
            {
                Color color = originalColorDictionary[key];


                SetPixel((int)key.x, (int)key.y, color);

            }
        }
    }

    
}
