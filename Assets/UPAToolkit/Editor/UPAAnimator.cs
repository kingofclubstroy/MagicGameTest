using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

public class UPAAnimator
{
    
    public static void MakeAnimationClip(List<UPAImage> animationImages)
    {

        Sprite[] sprites = new Sprite[animationImages.Count];

        string path = "Assets/Sprites/TestSpritesAnimation/";

        for (int i = 0; i < animationImages.Count; i++)
        {

            string newPath = path + i;

            UPASession.ExportImage(animationImages[i], TextureType.texture, TextureExtension.PNG, "");

            //Texture2D texture = animationImages[i].GetFinalImage(true);

            //Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));



            //sprites[i] = SaveSpriteToEditorPath(sprite, newPath);

            //Debug.LogError(sprites[i].texture.GetPixel(16, 16));

            //Debug.LogError(sprites[i].GetInstanceID());

        }

        

       

        //AnimationClip animClip = new AnimationClip();

        //animClip.frameRate = 25;   // FPS
        //animClip.wrapMode = WrapMode.Loop;

        //EditorCurveBinding spriteBinding = new EditorCurveBinding();
        //spriteBinding.type = typeof(SpriteRenderer);
        //spriteBinding.path = "";
        //spriteBinding.propertyName = "m_Sprite";
        //ObjectReferenceKeyframe[] spriteKeyFrames = new ObjectReferenceKeyframe[sprites.Length];

        //for (int i = 0; i < (sprites.Length); i++)
        //{
        //    spriteKeyFrames[i] = new ObjectReferenceKeyframe();
        //    spriteKeyFrames[i].time = i;
        //    spriteKeyFrames[i].value = sprites[i];
        //}

        //AnimationUtility.SetObjectReferenceCurve(animClip, spriteBinding, spriteKeyFrames);

       
        //AssetDatabase.CreateAsset(animClip, "assets/walk.anim");
        //AssetDatabase.SaveAssets();
        //AssetDatabase.Refresh();

    }

    static Sprite SaveSpriteToEditorPath(Sprite sp, string path)
    {

        string dir = Path.GetDirectoryName(path);

        Directory.CreateDirectory(dir);

        File.WriteAllBytes(path, sp.texture.EncodeToPNG());
        AssetDatabase.Refresh();
        AssetDatabase.AddObjectToAsset(sp, path);
        AssetDatabase.SaveAssets();

        TextureImporter ti = AssetImporter.GetAtPath(path) as TextureImporter;

        ti.spritePixelsPerUnit = sp.pixelsPerUnit;
        ti.mipmapEnabled = false;
        EditorUtility.SetDirty(ti);
        ti.SaveAndReimport();

        return AssetDatabase.LoadAssetAtPath(path, typeof(Sprite)) as Sprite;
    }


}
