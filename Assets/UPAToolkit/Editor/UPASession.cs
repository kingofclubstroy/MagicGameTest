//-----------------------------------------------------------------
// This class hosts utility methods for handling session information.
//-----------------------------------------------------------------

using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;

public class UPASession {

	public static UPAImage CreateImage (int w, int h) {
		string path = EditorUtility.SaveFilePanel ("Create UPAImage",
		                                           "Assets/", "Pixel Image.asset", "asset");
		if (path == "") {
			return null;
		}
		
		path = FileUtil.GetProjectRelativePath(path);
		
		UPAImage img = ScriptableObject.CreateInstance<UPAImage>();
		AssetDatabase.CreateAsset (img, path);
		
		AssetDatabase.SaveAssets();
		
		img.Init(w, h);
		EditorUtility.SetDirty(img);
		UPAEditorWindow.CurrentImg = img;
		
		EditorPrefs.SetString ("currentImgPath", AssetDatabase.GetAssetPath (img));
		
		if (UPAEditorWindow.window != null)
			UPAEditorWindow.window.Repaint();
		else
			UPAEditorWindow.Init();

		img.gridSpacing = 10 - Mathf.Abs (img.width - img.height)/100f;
		return img;
	}

	public static UPAImage OpenImage () {
		string path = EditorUtility.OpenFilePanel(
			"Find an Image (.asset | .png | .jpg)",
			"Assets/",
			"Image Files;*.asset;*.jpg;*.png");


        Debug.Log("path running");
		
		if (path.Length != 0) {
			// Check if the loaded file is an Asset or Image
			if (path.EndsWith(".asset")) {

                Debug.Log("is asset");
                Debug.Log(path);
				path = FileUtil.GetProjectRelativePath(path);
				UPAImage img = AssetDatabase.LoadAssetAtPath(path, typeof(UPAImage)) as UPAImage;
				EditorPrefs.SetString ("currentImgPath", path);
				return img;
			}
			else
			{
                
				// Load Texture from file
				Texture2D tex = LoadImageFromFile(path);
				// Create a new Image with textures dimensions
				UPAImage img = CreateImage(tex.width, tex.height);
				// Set pixel colors
				img.layers[0].tex = tex;
				img.layers[0].tex.filterMode = FilterMode.Point;
				img.layers[0].tex.Apply ();
				for (int x = 0; x < img.width; x++) {
					for (int y = 0; y < img.height; y++) {
						img.layers[0].map[x + y * tex.width] = tex.GetPixel(x, tex.height - 1 - y);
					}
				}
			}
		}
		
		return null;
	}

	public static Texture2D LoadImageFromFile (string path) {

        
		Texture2D tex = null;
		byte[] fileData;
		if (File.Exists(path))     {
			fileData = File.ReadAllBytes(path);
			tex = new Texture2D(2, 2);
			tex.LoadImage(fileData); //..this will auto-resize the texture dimensions.
		}
		return tex;
	}
	
	public static UPAImage OpenImageByAsset (UPAImage img) {

		if (img == null) {
			Debug.LogWarning ("Image is null. Returning null.");
			EditorPrefs.SetString ("currentImgPath", "");
			return null;
		}

		string path = AssetDatabase.GetAssetPath (img);
		EditorPrefs.SetString ("currentImgPath", path);
		
		return img;
	}

    public static UPAImage OpenFolder(bool isTemplate)
    {
        string path = EditorUtility.OpenFolderPanel(
            "Choose Template Folder",
            "",
            "");


        Debug.Log("path running");
        Debug.Log(path);

        if (path.Length != 0)
        {
            
            //TODO: Want to filter out asset later
           

            return OpenImagesFromFolder(path, isTemplate);
                
            
        }

        return null;
    }

    public static UPAImage OpenImagesFromFolder(string path, bool isTemplate)
    {
        if (path.Length != 0)
        {

            DirectoryInfo dir = new DirectoryInfo(path);
            FileInfo[] info = dir.GetFiles("*.png");


            return loadImageFromFileInfo(info, isTemplate);

            //TODO: need to change this as i will be juggling lots of images for animations and templates
            //EditorPrefs.SetString("currentImgPath", path);
            //return img;
        }

        return null;

        
    }

    public static List<UPAImage> OpenAnimationsFromFolder(bool isTemplate, string path = "")
    {

        if (path.Length == 0)
        {
            
            path = EditorUtility.OpenFolderPanel(
                "Choose Animation Folder",
                "Assets/Sprites",
                "");

        }



        if (path.Length != 0)
        {

            List<UPAImage> animation = new List<UPAImage>();

            DirectoryInfo dir = new DirectoryInfo(path);
            FileInfo[] info = dir.GetFiles("*.asset");
            Debug.Log("length = " + info.Length);
           


            if (info.Length > 0)
            {
                Debug.Log("info length greator than 0");

                Debug.Log(path);
               
                //We have some asset files, so lets load those in, instead of creating new ones
                //TODO: i dont know if these frames are loaded in order, may need to sort
                for (int i = 0; i < info.Length; i++ )
                {

                    string newPath;
                    
                    newPath = path + "/" + (i + 1) + ".asset";
                    newPath = FileUtil.GetProjectRelativePath(newPath);

                    Debug.Log("expected path = Assets / Sprites / Front RunTest / 1.asset");

                    Debug.Log("new path = ");



                    //newPath = "Assets/Sprites/Front RunTest/1";

                    Debug.Log(newPath);
                    
                    UPAImage frameImage = OpenFrameAtPath(newPath);
                    //frameImage.initilizeAlphas();
                    animation.Add(frameImage);
                }
            }
            else
            {
                info = dir.GetFiles("*.png");

                List<FileInfo[]> frames = sortFrames(info);

                int frameNumber = 1;


                foreach (FileInfo[] frame in frames)
                {

                    string newPath = path + "\\" + frameNumber + ".asset";

                    UPAImage i = loadImageFromFileInfo(frame, isTemplate, newPath);
                    i.initilizeAlphas();
                    animation.Add(i);

                    frameNumber += 1;

                }
            }

           

            UPAImage img = animation[0];

            Debug.Log(img.GetType());
            
            EditorPrefs.SetString("currentAnimationPath", path);
            UPAEditorWindow.CurrentImg = img;
            

            if (UPAEditorWindow.window != null)
                UPAEditorWindow.window.Repaint();
            else
                UPAEditorWindow.Init();

            

            return animation;
            
        }

        return null;

    }

    public static UPAImage CreateUPAImage(int w, int h)
    {
        UPAImage img = ScriptableObject.CreateInstance<UPAImage>();
        img.Init(w, h);
        img.gridSpacing = 10 - Mathf.Abs(img.width - img.height) / 100f;

        return img;
    }

    static UPAImage CreateAnimationFrame(int w, int h, string path)
    {

        //TODO: want to pass in the same path that the image was pulled from, so you don't have to navigate back
        
        if (path == "")
        {
            return null;
        }

        UPAImage img = CreateUPAImage(w, h);

        Debug.Log("creating custom image");
        Debug.Log(path);

        path = FileUtil.GetProjectRelativePath(path);
        Debug.Log(path);

        
        AssetDatabase.CreateAsset(img, path);

        AssetDatabase.SaveAssets();

       
        EditorUtility.SetDirty(img);


       
        return img;

    }

    static UPAImage customCreateImage(int w, int h, bool isTemplate)
    {

        //TODO: want to pass in the same path that the image was pulled from, so you don't have to navigate back
        string path = EditorUtility.SaveFilePanel("Create UPAImage",
                                                    "Assets/", "Pixel Image.asset", "asset");
        if (path == "")
        {
            return null;
        }

        Debug.Log("creating custom image");
        Debug.Log(path);

        path = FileUtil.GetProjectRelativePath(path);
        Debug.Log(path);

        UPAImage img = ScriptableObject.CreateInstance<UPAImage>();
        AssetDatabase.CreateAsset(img, path);

        AssetDatabase.SaveAssets();

        img.Init(w, h);
        EditorUtility.SetDirty(img);
        
        

        if (isTemplate)
        {

            EditorPrefs.SetString("templateImgPath", AssetDatabase.GetAssetPath(img));
            UPAEditorWindow.TemplateImage = img;

        } else
        {
            EditorPrefs.SetString("currentImgPath", AssetDatabase.GetAssetPath(img));
            UPAEditorWindow.CurrentImg = img;
        }

        if (UPAEditorWindow.window != null)
            UPAEditorWindow.window.Repaint();
        else
            UPAEditorWindow.Init();

        img.gridSpacing = 10 - Mathf.Abs(img.width - img.height) / 100f;
        return img;
        
    }

   


    public static UPAImage OpenImageAtPath (string path, bool isTemplate) {

        string variableName;
        if(isTemplate)
        {
            variableName = "templateImgPath";
        } else
        {
            variableName = "currentImgPath";
        }


		if (path.Length != 0) {
			UPAImage img = AssetDatabase.LoadAssetAtPath(path, typeof(UPAImage)) as UPAImage;

            

			if (img == null) {
				EditorPrefs.SetString (variableName, "");
				return null;
			}

            //TODO: need to change this as i will be juggling lots of images for animations and templates
			EditorPrefs.SetString (variableName, path);
			return img;
		}
		
		return null;
	}

	public static bool ExportImage (UPAImage img, TextureType type, TextureExtension extension) {
		string path = EditorUtility.SaveFilePanel(
			"Export image as " + extension.ToString(),
			"Assets/",
			img.name + "." + extension.ToString().ToLower(),
			extension.ToString().ToLower());
		
		if (path.Length == 0)
			return false;
		
		byte[] bytes;
		if (extension == TextureExtension.PNG) {
			// Encode texture into PNG
			bytes = img.GetFinalImage(true).EncodeToPNG();
		} else {
			// Encode texture into JPG
			
			#if UNITY_4_2
			bytes = img.GetFinalImage(true).EncodeToPNG();
			#elif UNITY_4_3
			bytes = img.GetFinalImage(true).EncodeToPNG();
			#elif UNITY_4_5
			bytes = img.GetFinalImage(true).EncodeToJPG();
			#else
			bytes = img.GetFinalImage(true).EncodeToJPG();
			#endif
		}
		
		path = FileUtil.GetProjectRelativePath(path);
		
		//Write to a file in the project folder
		File.WriteAllBytes(path, bytes);
		AssetDatabase.Refresh();
		
		TextureImporter texImp = AssetImporter.GetAtPath(path) as TextureImporter; 
		
		if (type == TextureType.texture)
			texImp.textureType = TextureImporterType.Default;
		else if (type == TextureType.sprite) {
			texImp.textureType = TextureImporterType.Sprite;

			#if UNITY_4_2
			texImp.spritePixelsToUnits = 10;
			#elif UNITY_4_3
			texImp.spritePixelsToUnits = 10;
			#elif UNITY_4_5
			texImp.spritePixelsToUnits = 10;
			#else
			texImp.spritePixelsPerUnit = 10;
			#endif
		}
		
		texImp.filterMode = FilterMode.Point;
		texImp.textureFormat = TextureImporterFormat.AutomaticTruecolor;
		
		AssetDatabase.ImportAsset(path); 
		
		return true;
	}

    static List<FileInfo[]> sortFrames(FileInfo[] info)
    {

        List<FileInfo[]> finalAnimation = new List<FileInfo[]>();

        List<FileInfo> tempList = new List<FileInfo>();

        foreach(FileInfo f in info)
        {
            tempList.Add(f);
        }

        List<FileInfo> currentFrame = new List<FileInfo>();

        int frameIndex = 1;

        
        while (tempList.Count > 0)
        {
            bool found = false;

            // Go through all file paths and grab the paths that belong to the same frame
            for(int x = tempList.Count - 1; x >= 0; x--)
            {
                FileInfo f = tempList[x];
                string[] split = splitPath(f.ToString());

               
                if(int.Parse(split[0]) == frameIndex)
                {
                    found = true;
                    currentFrame.Add(f);
                    tempList.Remove(f);

                    if(tempList.Count == 0)
                    {
                        found = false;
                    }
                }
            }

            if (found == false)
            {
                //increment frame index so the next loop will be looking for the next frame
                frameIndex += 1;

                //Now we have to sort by layer
                List<FileInfo> finalFrame = new List<FileInfo>();

                int layer = 1;

                while (currentFrame.Count > 0)
                {

                    found = false;

                    // Go through all files and find the one that belongs to the correct layer
                    for (int i = currentFrame.Count - 1; i >= 0; i--)
                    {
                        FileInfo f = currentFrame[i];

                        string[] split = splitPath(f.ToString());
                        if (int.Parse(split[1]) == layer)
                        {
                            found = true;
                            finalFrame.Add(f);
                            currentFrame.Remove(f);

                            layer += 1;

                            break;
                        }


                    }

                    if (found == false)
                    {
                        //Didnt find anything for that layer, so lets just increase by 1
                        layer += 1;
                    }


                }

                //now we have a final sorted frame, but need to convert to an array for some reason? TODO: fix this if proformance needs to be improved

                FileInfo[] frame = finalFrame.ToArray();

                //Add the frame to the final list of all frames for that animation
                finalAnimation.Add(frame);
            }

              
        }

        return finalAnimation;

    }

    static UPAImage loadImageFromFileInfo(FileInfo[] info, bool isTemplate, string path = "", bool save = false)
    {
        Debug.Log("loading image from file info");
        Texture2D[] textures = new Texture2D[info.Length];

        Debug.Log("info length = " + info.Length);

        for (int i = 0; i < info.Length; i++)
        {
            FileInfo f = info[i];
            
            textures[i] = LoadImageFromFile(f.ToString());

        }

        Texture2D tex0 = textures[0];
        UPAImage img;

        if (path.Length != 0)
        {
            Debug.Log("path != 0 it = " + path);
            //TODO: i set some things that were set when an image was created after all animations run,
            //then the layers are added lower down... is this a problem or we good?
            img = CreateAnimationFrame(tex0.width, tex0.height, path);
        }
        else
        {
            if (save)
            {
                img = CreateUPAImage(tex0.width, tex0.height);
            }
            else
            {
                Debug.Log("no path");
                img = customCreateImage(tex0.width, tex0.height, isTemplate);

            }
        }

        Debug.Log("length of textures = " + textures.Length);

        for (int layerNum = 0; layerNum < textures.Length; layerNum++)
        {

            Texture2D tex = textures[layerNum];

            if (layerNum != 0)
            {
                img.AddLayer();
            }
            // Set pixel colors
            img.layers[layerNum].tex = tex;
            img.layers[layerNum].tex.filterMode = FilterMode.Point;
            img.layers[layerNum].tex.Apply();

            if(isTemplate)
            {
                img.layers[layerNum].name = splitPath(info[layerNum].ToString())[0].Split('.')[0];

            } else
            {
                img.layers[layerNum].name = splitPath(info[layerNum].ToString())[2].Split('.')[0];
            }

            

            for (int x = 0; x < img.width; x++)
            {
                for (int y = 0; y < img.height; y++)
                {
                    img.layers[layerNum].map[x + y * tex.width] = tex.GetPixel(x, tex.height - 1 - y);
                }
            }
        }


        return img;
    }

    public static UPAImage LoadImageTemplate(Dictionary<string, string> paths)
    {

        List<FileInfo> infoList = new List<FileInfo>();

        foreach (string path in paths.Values)
        {
            DirectoryInfo dir = new DirectoryInfo(path);
            FileInfo[] tempInfo = dir.GetFiles("*.png");

            infoList.AddRange(tempInfo);
        }

        FileInfo[] info = infoList.ToArray();


        return loadImageFromFileInfo(info, true, "", true);

    }

    static string[] splitPath(string path)
    {
        string[] tempSplit = path.Split('\\');
        return tempSplit[tempSplit.Length -1].Split('-');
    }

    public static UPAImage OpenFrameAtPath(string path)
    {


        if (path.Length != 0)
        {
            UPAImage img = AssetDatabase.LoadAssetAtPath(path, typeof(UPAImage)) as UPAImage;

            

            if(img == null)
            {
                Debug.Log("image is null!?!");
            } else
            {
                Debug.Log("image found!");
                Debug.Log("number layers = ");
                Debug.Log(img.layers.Count);
            }


            return img;
        }

        return null;
    }
}
