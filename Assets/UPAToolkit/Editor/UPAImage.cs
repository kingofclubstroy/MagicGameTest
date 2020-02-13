//-----------------------------------------------------------------
// This class stores all information about the image.
// It has a full pixel map, width & height properties and some private project data.
// It also hosts functions for calculating how the pixels should be visualized in the editor.
//-----------------------------------------------------------------

using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

[System.Serializable]
public class UPAImage : ScriptableObject {

	// HELPER GETTERS
	private Rect window {
		get { return UPAEditorWindow.window.position; }
	}
	

	// IMAGE DATA

	public int width;
	public int height;

	public List<UPALayer> layers;
	public int layerCount
	{
		get { return layers.Count; }
	}

	public Texture2D finalImg;

	// VIEW & NAVIGATION SETTINGS

	[SerializeField]
	private float _gridSpacing = 20f;
	public float gridSpacing {
		get { return _gridSpacing + 1f; }
		set { _gridSpacing = Mathf.Clamp (value, 0, 140f); }
	}

	public float gridOffsetY = 0;
	public float gridOffsetX = 0;

	//Make sure we always get a valid layer
	private int _selectedLayer = 0;
	public int selectedLayer {
		get {
			return Mathf.Clamp(_selectedLayer, 0, layerCount);
		}
		set {

            layerChanged(_selectedLayer, value);
            _selectedLayer = value;
        }
	}
	
	
	// PAINTING SETTINGS

	public Color selectedColor = new Color (1,0,0,1);
	public UPATool tool = UPATool.PaintBrush;
	public int gridBGIndex = 0;


	//MISC VARIABLES

	public bool dirty = false;      // Used for determining if data has changed

    #region custom variables

    List<List<Vector2>> templateObjects;

    List<Vector2> currentTemplate;

    int templateIndex = 0;

    int pixelIndex = 0;

    Color currentPixelColor;

    public Vector2 currentPixelPosition;

    #endregion


    // Class constructor
    public UPAImage () {
		// do nothing so far
	}

	// This is not called in constructor to have more control
	public void Init (int w, int h) {
		width = w;
		height = h;

		layers = new List<UPALayer>();
		UPALayer newLayer = new UPALayer (this);
		layers.Add ( newLayer );
		
		EditorUtility.SetDirty (this);
		dirty = true;
	}

	// Color a certain pixel by position in window in a certain layer
	public void SetPixelByPos (Color color, Vector2 pos, int layer) {
		Vector2 pixelCoordinate = GetPixelCoordinate (pos);

		if (pixelCoordinate == new Vector2 (-1, -1))
			return;

		Undo.RecordObject (layers[layer].tex, "ColorPixel");

        Debug.Log("SetPixelByPos");

        Debug.Log(pixelCoordinate.x);
        Debug.Log(pixelCoordinate.y);


		layers[layer].SetPixel ((int)pixelCoordinate.x, (int)pixelCoordinate.y, color);
		
		EditorUtility.SetDirty (this);
		dirty = true;
	}

	// Return a certain pixel by position in window
	public Color GetPixelByPos (Vector2 pos, int layer) {
		Vector2 pixelCoordinate = GetPixelCoordinate (pos);

		if (pixelCoordinate == new Vector2 (-1, -1)) {
			return Color.clear;
		} else {
			return layers[layer].GetPixel ((int)pixelCoordinate.x, (int)pixelCoordinate.y);
		}
	}

	public Color GetBlendedPixel (int x, int y) {
		Color color = Color.clear;

		for (int i = 0; i < layers.Count; i++) {
			if (!layers[i].enabled)
				continue;

			Color pixel = layers[i].tex.GetPixel(x,y);

			// This is a blend between two methods of calculating color blending; Alpha blending and premultiplied alpha blending
			// I have no clue why this actually works but it's very accurate :D
			float newR = Mathf.Lerp (1f * pixel.r + (1f - pixel.a) * color.r, pixel.a * pixel.r + (1f - pixel.a) * color.r, color.a);
			float newG = Mathf.Lerp (1f * pixel.g + (1f - pixel.a) * color.g, pixel.a * pixel.g + (1f - pixel.a) * color.g, color.a);
			float newB = Mathf.Lerp (1f * pixel.b + (1f - pixel.a) * color.b, pixel.a * pixel.b + (1f - pixel.a) * color.b, color.a);

			float newA = pixel.a + color.a * (1 - pixel.a);

			color = new Color (newR, newG, newB, newA);
		}

		return color;
	}
	
	public void ChangeLayerPosition (int from, int to) {
		if (from >= layers.Count || to >= layers.Count || from < 0 || to < 0) {
			Debug.LogError ("Cannot ChangeLayerPosition, out of range.");
			return;
		}
		
		UPALayer layer = layers[from];
		layers.RemoveAt(from);
		layers.Insert(to, layer);

		dirty = true;
	}

	// Get the rect of the image as displayed in the editor
	public Rect GetImgRect (bool isTemplate = false) {

        int pixelSize;
        float divisor;
        int offsetX;

        float yPos, xPos;

        float ratio, w, h;
        if (isTemplate)
        {
            pixelSize = 30;
            divisor = 4f;

            xPos = 130;
            yPos = 60;

            ratio = (float)height / (float)width;

            w = gridSpacing * pixelSize;
            h = ratio * gridSpacing * pixelSize;

            return new Rect(xPos, yPos, w, h);



        }
        else
        {
            pixelSize = 57;
            divisor = 2f;
            offsetX = width / 2;
            
        }

		ratio = (float)height / (float)width;
        w = gridSpacing * pixelSize;
        h = ratio * gridSpacing * pixelSize;


        xPos = window.width / divisor - w/divisor + gridOffsetX + (w * 0.58f);
		yPos = window.height / divisor - h/divisor + 20 + gridOffsetY + 20;

		return new Rect (xPos,yPos, w, h);
	}

	public Vector2 GetPixelCoordinate (Vector2 pos) {
		Rect texPos = GetImgRect();
			
		if (!texPos.Contains (pos)) {
			return new Vector2(-1f,-1f);
		}

		float relX = (pos.x - texPos.x) / texPos.width;
		float relY = (texPos.y - pos.y) / texPos.height;
		
		int pixelX = (int)( width * relX );
		int pixelY = (int)( height * relY ) - 1;

		return new Vector2(pixelX, pixelY);
	}
	
	public Vector2 GetReadablePixelCoordinate (Vector2 pos) {
		Vector2 coord = GetPixelCoordinate (pos);
		
		if (coord.x == -1)
			return coord;
		
		coord.x += 1;
		coord.y *= -1;
		return coord;
	}

	public Texture2D GetFinalImage (bool update) {

		if (!dirty && finalImg != null || !update && finalImg != null)
			return finalImg;

		finalImg = UPADrawer.CalculateBlendedTex(layers);
		finalImg.filterMode = FilterMode.Point;
		finalImg.Apply();

		dirty = false;
		return finalImg;
	}

	public void LoadAllTexsFromMaps () {
		for (int i = 0; i < layers.Count; i++) {
			if (layers[i].tex == null)
				layers[i].LoadTexFromMap();
		}
	}

	public void AddLayer () {
		Undo.RecordObject (this, "AddLayer");
		EditorUtility.SetDirty (this);
		this.dirty = true;

		UPALayer newLayer = new UPALayer (this);
		layers.Add(newLayer);
	}

	public void RemoveLayerAt (int index) {
		Undo.RecordObject (this, "RemoveLayer");
		EditorUtility.SetDirty (this);
		this.dirty = true;

		layers.RemoveAt (index);
		if (selectedLayer == index) {
			selectedLayer = index - 1;
		}
	}

    public void loopThroughImage()
    {

        List<List<Vector2>> unsortedTemplateObjects = new List<List<Vector2>>();

        for (int j = height - 1; j >= 0; j--)
        {

            for (int i = 0; i < width; i++)
            {

                Color color = layers[selectedLayer].GetPixel(i, j);

                if (color != Color.clear)
                {

                    bool found = false;
                    foreach(List<Vector2> keyTest in unsortedTemplateObjects)
                    {

                        Vector2 pos = new Vector2(i, j);

                        if (keyTest.Contains(pos))
                        {
                            found = true;
                            break;
                        }

                    }

                    if (! found)
                    {
                        unsortedTemplateObjects.Add(setupCrawl(i, j));
                    }
                    
                }

            }

        }

        templateObjects = sortTemplateObjects(unsortedTemplateObjects);

        currentTemplate = templateObjects[0];

        pixelIndex = 0;

        templateIndex = 0;

        currentPixelPosition = new Vector2(-1, -1);

        focusPixel();


    }


    void focusPixel()
    {

        setBackColor();


        setColor();


    }

    void setColor()
    {

        currentPixelPosition = currentTemplate[pixelIndex];

        currentPixelColor = layers[selectedLayer].GetPixel((int) currentPixelPosition.x, (int) currentPixelPosition.y);

        layers[selectedLayer].SetPixel((int)currentPixelPosition.x, (int)currentPixelPosition.y - height, Color.cyan);

        UPAEditorWindow.CurrentImg.layers[UPAEditorWindow.CurrentImg.selectedLayer].colorMappedPixels(currentPixelPosition);


    }

    public void focusPixel(int direction)
    {
        if(pixelIndex + direction < currentTemplate.Count && pixelIndex + direction >= 0)
        {
            //there is another pixel in this object
            pixelIndex += direction;

        } else
        {

            //there is another object, so lets set the current object and reset the pixel index
            templateIndex += direction;

            if(templateIndex >= templateObjects.Count)
            {
                templateIndex = 0;
            } else if(templateIndex < 0)
            {
                templateIndex = templateObjects.Count - 1;
            }
            currentTemplate = templateObjects[templateIndex];

            if (direction < 0)
            {
                pixelIndex = currentTemplate.Count - 1;
            }
            else
            {
                pixelIndex = 0;
            }
            
        }

        focusPixel();
    }

    public void setBackColor()
    {
        //need to set the current pixel to original color if there was a past color

        if (currentPixelPosition != new Vector2(-1, -1))
        {
            layers[selectedLayer].SetPixel((int)currentPixelPosition.x, (int)currentPixelPosition.y - height, currentPixelColor);
        }


        
    }


    List<Vector2> setupCrawl(int x, int y)
    {

        List<Vector2> key = new List<Vector2>();

        return crawlThrough(new Vector2(x, y), key);

    }

    List<Vector2> crawlThrough(Vector2 pos, List<Vector2> key)
    {

        if(pos.x < 0 || pos.x >= width || pos.y < 0 || pos.y >= height || key.Contains(pos))
        {
            return key;
        }

        Color color = layers[selectedLayer].GetPixel((int)pos.x, (int)pos.y);

        if (color == Color.clear)
        {
            //clear so not part of object
            // or we already have added this to the dictionary
            return key;
        }

        //we know its part of the object, so lets add it to the key dictionary, and then check other neighbouring areas
        key.Add(pos);

        //East
        key = crawlThrough(new Vector2(pos.x + 1, pos.y), key);

        //SouthEast
        key = crawlThrough(new Vector2(pos.x + 1, pos.y + 1), key);

        //South
        key = crawlThrough(new Vector2(pos.x, pos.y + 1), key);

        //SouthWest
        key = crawlThrough(new Vector2(pos.x - 1, pos.y + 1), key);

        //West
        key = crawlThrough(new Vector2(pos.x - 1, pos.y), key);

        //NorthWest
        key = crawlThrough(new Vector2(pos.x - 1, pos.y - 1), key);

        //North
        key = crawlThrough(new Vector2(pos.x, pos.y - 1), key);

        //NorthEast
        key = crawlThrough(new Vector2(pos.x + 1, pos.y - 1), key);

        return key;

    }

    List<List<Vector2>> sortTemplateObjects(List<List<Vector2>> objs)
    {

        List<List<Vector2>> finalObjects = new List<List<Vector2>>();

        foreach(List<Vector2> obj in objs)
        {

            List<Vector2> sortedList = new List<Vector2>();

            for (int y = height - 1; y >= 0; y--)
            {

                List<Vector2> tempList = new List<Vector2>();

                foreach(Vector2 pos in obj)
                {

                    if(pos.y == y)
                    {
                        tempList.Add(pos);
                    }

                }

                for (int x = 0; x < width; x++)
                {

                    foreach(Vector2 pos in tempList)
                    {
                        if (pos.x == x)
                        {
                            sortedList.Add(pos);
                        }
                    }


                }

            }

            finalObjects.Add(sortedList);

        }


        return finalObjects;

    }

    
    public string changeLayer(int direction)
    {

        int tempLayer = (selectedLayer + direction) % (layerCount);
        if(tempLayer < 0)
        {
            tempLayer = layerCount - 1;
        }

        selectedLayer = tempLayer;

        //layers[selectedLayer].colorMappedPixels(UPAEditorWindow.TemplateImage.currentPixelPosition);

        Debug.Log(selectedLayer);
        dirty = true;

        return layers[selectedLayer].name;

    }

    public void findLayer(string layerName)
    {

        for (int i = 0; i < layers.Count; i++)
        {

            if(layers[i].name == layerName)
            {
                setBackColor();
                selectedLayer = i;
                loopThroughImage();
                dirty = true;
                
            }

        }

    }

    void layerChanged(int previousLayer, int newLayer)
    {
        layers[previousLayer].setMappedColorsBack();
        layers[previousLayer].setAlpha(true);
        
        layers[newLayer].setAlpha(false);
        layers[newLayer].colorMappedPixels(UPAEditorWindow.TemplateImage.currentPixelPosition);

    }

    public void setAllNormalAlpha()
    {
        foreach(UPALayer layer in layers)
        {
            layer.setAlpha(false);
        }
    }

    public void initilizeAlphas()
    {

        for(int i = 0; i < layerCount; i++)
        {
            if(i != selectedLayer)
            {
                layers[i].setAlpha(true);
            }
            
        }

    }

    public void mapPixelByPos(Vector2 pos, Vector2 templatePos)
    {
        Vector2 pixelCoordinate = GetPixelCoordinate(pos);

        if (pixelCoordinate == new Vector2(-1, -1))
            return;

        layers[selectedLayer].mapPixel(templatePos, pixelCoordinate);

        EditorUtility.SetDirty(this);
        dirty = true;
    }



    public UPALayer GetLayer(string layerName)
    {

        foreach(UPALayer layer in layers)
        {
            if(layer.name == layerName)
            {
                return layer;
            }
        }

        return null;
    }

}
