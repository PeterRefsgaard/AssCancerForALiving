#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

public class ForceAndroidResolver
{
    [MenuItem("Tools/Force Android Dependency Resolve")]
    public static void ForceResolve()
    {
        System.Type resolver = System.Type.GetType("GooglePlayServices.PlayServicesResolver, ExternalDependencyManager");
        if (resolver != null)
        {
            var method = resolver.GetMethod("ResolveSync", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);
            if (method != null)
            {
                method.Invoke(null, null);
                Debug.Log("✅ Android Dependencies Force Resolved!");
            }
            else
            {
                Debug.LogError("❌ Could not find ResolveSync method.");
            }
        }
        else
        {
            Debug.LogError("❌ Could not find PlayServicesResolver type.");
        }
    }
}
#endif

