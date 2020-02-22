//-----------------------------------------------------------------
// This script handles the main Pixel Art Editor.
// It selects tools, finds the right pixels to color, handles input events & draws the toolbar gui.
// TODO: Tidy things up. Split functionality into smaller code portions. Make even a bit of optimization?
//-----------------------------------------------------------------

using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class UPAEditorWindow : EditorWindow {
	
	public static UPAEditorWindow window;	// The static instance of the window
	
	public static UPAImage CurrentImg;		// The img currently being edited

    public static UPAImage TemplateImage; // The template image in the corner

    public static List<UPAImage> animation; // The list of frames in an animation

    public static int animationIndex = 0;

    public static UPATool selectedTool;

    public static bool gettingPreviewArmor = false;

    Dictionary<string, string> pathDictionary = new Dictionary<string, string>();
	
	
	// HELPFUL GETTERS AND SETTERS
	
	private float gridSpacing {
		get { return CurrentImg.gridSpacing; }
		set { CurrentImg.gridSpacing = value; }
	}
	private float gridOffsetX {
		get { return CurrentImg.gridOffsetX; }
		set { CurrentImg.gridOffsetX = value; }
	}
	private float gridOffsetY {
		get { return CurrentImg.gridOffsetY; }
		set { CurrentImg.gridOffsetY = value; }
	}
	
	
	private Color32 selectedColor {
		get { return CurrentImg.selectedColor; }
		set { CurrentImg.selectedColor = value; }
	}
	private int gridBGIndex {
		get { return CurrentImg.gridBGIndex; }
		set { CurrentImg.gridBGIndex = value; }
	}
	
	
	// MISC TEMP VARIABLES
	
	// Stores the previous tool when temporarily switching
	private static UPATool lastTool = UPATool.Empty;
	
	
	// INITIALIZATION
	
	[MenuItem ("Window/Pixel Art Editor %#p")]
	public static void Init () {

        selectedTool = UPATool.Map;

        EditorPrefs.SetString("templateImgPath", "");
        EditorPrefs.SetString("currentAnimationPath", "");
        // Get existing open window or if none, make new one
        window = (UPAEditorWindow)EditorWindow.GetWindow (typeof (UPAEditorWindow));
		#if UNITY_4_3
		window.title = "Pixel Art Editor";
		#elif UNITY_4_6
		window.title = "Pixel Art Editor";
		#else
		window.titleContent = new GUIContent ("Pixel Art Editor");
		#endif
		
		string path = EditorPrefs.GetString ("currentAnimationPath", "");

        Debug.Log(path);
       

        string templatePath = EditorPrefs.GetString("templateImgPath", "");

        Debug.Log(templatePath);

        if (path.Length != 0)
        {
            Debug.Log("opening image at path");
            animation = UPASession.OpenAnimationsFromFolder(false, path);
            //CurrentImg.initilizeAlphas();
            animationIndex = 0;
           
        }

        if (templatePath.Length != 0)
        {
            Debug.Log("opening image at path");
            TemplateImage = UPASession.OpenImageAtPath(templatePath, true);
            //TemplateImage.initilizeAlphas();
            TemplateImage.loopThroughImage();
        }
    }
	
	
	// Draw the Pixel Art Editor.
	// This includes both toolbar and painting area.
	// TODO: Add comments
	void OnGUI () {
		if (window == null)
			Init ();
		
		if (CurrentImg == null || TemplateImage == null) {

            Debug.Log("one is null");
			
			string curImgPath = EditorPrefs.GetString ("currentAnimationPath", "");
            string templateImgPath = EditorPrefs.GetString("templateImgPath", "");

            


            if (curImgPath.Length != 0) {

                animation = UPASession.OpenAnimationsFromFolder(false, curImgPath);

                if (animation.Count > 0)
                {
                    CurrentImg = animation[0];
                    //CurrentImg.initilizeAlphas();
                    animationIndex = 0;
                }

            }

            if(templateImgPath.Length != 0)
            {
                TemplateImage = UPASession.OpenImageAtPath(templateImgPath, true);
                TemplateImage.initilizeAlphas();
                TemplateImage.loopThroughImage();

            }

            if (CurrentImg == null)
            {

                if (GUI.Button(new Rect(window.position.width / 2f - 140, window.position.height / 2f - 25, 130, 50), "Load Animation"))
                {
                    //UPASession.OpenImage();
                    animation = UPASession.OpenAnimationsFromFolder(false);
                    CurrentImg.initilizeAlphas();
                    animationIndex = 0;
                }

                

            } else if (TemplateImage == null)
            {
               

                if (GUI.Button(new Rect(window.position.width / 2f - 140, window.position.height / 2f - 25, 130, 50), "New Template"))
                {
                    UPAImageCreationWindow.Init();
                }
                if (GUI.Button(new Rect(window.position.width / 2f + 10, window.position.height / 2f - 25, 130, 50), "Open Template"))
                {
                    TemplateImage = UPASession.OpenFolder(true);
                    TemplateImage.LoadAllTexsFromMaps();
                    TemplateImage.initilizeAlphas();
                    TemplateImage.loopThroughImage();
                    return;
                }
            }

           
			
			return;
		}

        if(gettingPreviewArmor)
        {

            if (GUI.Button(new Rect(window.position.width / 2f, window.position.height / 8f, 130, 50), "Load Head"))
            {

                pathDictionary["head"] = EditorUtility.OpenFolderPanel(
                "Choose Animation Folder",
                "Assets/Sprites/Armor/Heads/",
                "");

            }

            if (GUI.Button(new Rect(window.position.width / 2f - 70, window.position.height / 8f + 60, 130, 50), "Load Arms"))
            {

                pathDictionary["arms"] = EditorUtility.OpenFolderPanel(
               "Choose Animation Folder",
               "Assets/Sprites/Armor/Arms/",
               "");

            }

            if (GUI.Button(new Rect(window.position.width / 2f + 70, window.position.height / 8f + 60, 130, 50), "Load Body"))
            {

                pathDictionary["body"] = EditorUtility.OpenFolderPanel(
               "Choose Animation Folder",
               "Assets/Sprites/Armor/Bodies/",
               "");

            }

            if (GUI.Button(new Rect(window.position.width / 2f, window.position.height / 8f + 120, 130, 50), "Load Legs"))
            {

                pathDictionary["legs"] = EditorUtility.OpenFolderPanel(
               "Choose Animation Folder",
               "Assets/Sprites/Legs/",
               "");

            }

            if (GUI.Button(new Rect(window.position.width / 2f + 10, window.position.height / 2f - 25, 130, 50), "Done"))
            {

                UPAImage armorTemplate = UPASession.LoadImageTemplate(pathDictionary);

                //now we have a map to draw things from lets now map the template to each proper layer

                List<UPAImage> newAnimation = new List<UPAImage>();

                foreach (UPAImage img in animation)
                {

                    UPAImage newImage = UPASession.CreateUPAImage(img.width, img.height);

                    for (int i = 0; i < img.layers.Count; i++)
                    {

                        UPALayer layer = new UPALayer(img.layers[i]);



                        UPALayer templateLayer = armorTemplate.GetLayer(layer.name);

                        if(templateLayer != null)
                        {

                            Debug.Log("layer does not == null!");

                            

                            foreach(Vector2 key in layer.colorMapDictionary.Keys)
                            {
                                
                                Vector2 value = layer.colorMapDictionary[key];
                                layer.SetPixel((int)key.x, (int)key.y, templateLayer.GetPixel((int)value.x, (int)value.y));
                            }

                        }

                        if(i != 0)
                        {
                            newImage.AddLayer();
                        }

                        newImage.layers[i] = layer;

                    }

                    newAnimation.Add(newImage);
                }

                animation = newAnimation;

                gettingPreviewArmor = false;
                pathDictionary.Clear();
               
            }

            return;
        }

       
		
		// Init the textures correctly, won't cost performance if nothing to load
		CurrentImg.LoadAllTexsFromMaps();

        TemplateImage.LoadAllTexsFromMaps();
		
		EditorGUI.DrawRect (window.position, new Color32 (30,30,30,255));
		
		
		#region Event handling
		Event e = Event.current;	//Init event handler
		
		//Capture mouse position
		Vector2 mousePos = e.mousePosition;
		
		// If key is pressed
		if (e.button == 0) {
			
			// Mouse buttons
			if (e.isMouse && mousePos.y > 40 && e.type != EventType.MouseUp) {
				if (!UPADrawer.GetLayerPanelRect (window.position).Contains (mousePos)) {
					
					if (selectedTool == UPATool.Eraser)
						CurrentImg.SetPixelByPos (Color.clear, mousePos, CurrentImg.selectedLayer);
					else if (selectedTool == UPATool.PaintBrush)
						CurrentImg.SetPixelByPos (selectedColor, mousePos, CurrentImg.selectedLayer);
					else if (selectedTool == UPATool.BoxBrush)
						Debug.Log ("TODO: Add Box Brush tool.");
					else if (selectedTool == UPATool.ColorPicker){
						Vector2 pCoord = CurrentImg.GetPixelCoordinate (mousePos);
						Color? newColor = CurrentImg.GetBlendedPixel( (int)pCoord.x, (int)pCoord.y );
						if (newColor != null && newColor != Color.clear){
							selectedColor = (Color)newColor;
						}
                        selectedTool = lastTool;
					} else if (selectedTool == UPATool.Map)
                    {
                        CurrentImg.mapPixelByPos(mousePos, TemplateImage.currentPixelPosition);
                    }
					
				}
			}
			
			// Key down
			if (e.type == EventType.KeyDown) {
				if (e.keyCode == KeyCode.W) {
                    changeLayer(1);
				}
				if (e.keyCode == KeyCode.S) {
                    changeLayer(-1);
				}
				if (e.keyCode == KeyCode.A) {
                    TemplateImage.focusPixel(-1);
                }
				if (e.keyCode == KeyCode.D) {
                    TemplateImage.focusPixel(1);
                }
				
				if (e.keyCode == KeyCode.Alpha1) {
                    selectedTool = UPATool.PaintBrush;
				}
				if (e.keyCode == KeyCode.Alpha2) {
                    selectedTool = UPATool.Eraser;
				}
				if (e.keyCode == KeyCode.P) {
					lastTool = selectedTool;
                    selectedTool = UPATool.ColorPicker;
				}
				
				if (e.keyCode == KeyCode.UpArrow) { 
                    CurrentImg.layers[0].setAlpha(true);
				}
				if (e.keyCode == KeyCode.DownArrow) {
                    CurrentImg.layers[0].setAlpha(false);
                }
                if (e.keyCode == KeyCode.LeftArrow)
                {
                    changeFrame(-1);
                }
                if (e.keyCode == KeyCode.RightArrow)
                {
                    changeFrame(1);
                }

            }
			
			if (e.control) {
				if (lastTool == UPATool.Empty) {
					lastTool = selectedTool;
                    selectedTool = UPATool.Eraser;
				}
			} else if (e.type == EventType.KeyUp && e.keyCode == KeyCode.LeftControl) {
				if (lastTool != UPATool.Empty) {
                    selectedTool = lastTool;
					lastTool = UPATool.Empty;
				}
			}
		}

        
		
		// TODO: Better way of doing this?
		// Why does it behave so weirdly with my mac tablet.
		if (e.type == EventType.ScrollWheel) {
			gridSpacing -= e.delta.y;
		}
		#endregion
		
		// DRAW IMAGE
		UPADrawer.DrawImage (CurrentImg , false);

        //Test draw another image
        UPADrawer.DrawImage(TemplateImage, true);

        UPADrawer.DrawToolbar (window.position, mousePos);
		
		UPADrawer.DrawLayerPanel ( window.position );

        UPADrawer.DrawFramePanel(window.position);
		
		e.Use();	// Release event handler
	}

    private void OnDisable()
    {
        Debug.Log("im disabled");
        TemplateImage.setBackColor();
        TemplateImage.setAllNormalAlpha();



        foreach (UPAImage image in animation)
        {
            image.setAllNormalAlpha();
        }

        CurrentImg.layers[CurrentImg.selectedLayer].setMappedColorsBack();
    }

    private void OnEnable()
    {
        Debug.Log("im enabled");
    }

    void changeFrame(int direction)
    {

        if(animation == null || animation.Count == 0)
        {
            return;
        }

        if(animationIndex + direction < 0)
        {
            animationIndex = animation.Count - 1;

        } else if (animationIndex + direction >= animation.Count)
        {
            animationIndex = 0;

        } else
        {
            animationIndex += direction;
        }

        int selectedLayer = CurrentImg.selectedLayer;

        CurrentImg = animation[animationIndex];

        CurrentImg.selectedLayer = selectedLayer;

        

    } 

    void changeLayer(int direction)
    {
        TemplateImage.findLayer(CurrentImg.changeLayer(direction));
       
    }
}