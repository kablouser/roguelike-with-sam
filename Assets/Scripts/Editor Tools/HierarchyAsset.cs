using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class HierarchyAsset : ScriptableObject
{
#if UNITY_EDITOR
    protected virtual ScriptableObject ParentAsset(ScriptableObject asset)
    {
        string myPath = AssetDatabase.GetAssetPath(this);

        bool isSub = AssetDatabase.IsSubAsset(asset);
        string path = AssetDatabase.GetAssetPath(asset);

        if (isSub && path == myPath)
            //its already a child
            return asset;

        var copy = Instantiate(asset);
        //retain original name, not the "(clone)" tail
        copy.name = asset.name;
        AssetDatabase.AddObjectToAsset(copy, myPath);

        //don't delete if it's a sub-asset of something else
        if (isSub == false && AssetDatabase.Contains(asset))
            AssetDatabase.DeleteAsset(path);

        //update inspector
        AssetDatabase.ImportAsset(myPath);
        EditorGUIUtility.PingObject(this);

        return copy;
    }

    [ContextMenu("Clear Children")]
    protected virtual void ClearChildren()
    {
        Undo.RecordObject(this, "Clear Children");

        string myPath = AssetDatabase.GetAssetPath(this);

        //get all sub-assets that are not in my effects list        
        Object[] subAssets = AssetDatabase.LoadAllAssetRepresentationsAtPath(myPath);
        foreach (Object subAsset in subAssets)
            AssetDatabase.RemoveObjectFromAsset(subAsset);

        //update inspector
        AssetDatabase.ImportAsset(myPath);
        EditorGUIUtility.PingObject(this);
    }
#endif
}
