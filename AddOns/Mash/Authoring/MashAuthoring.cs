using Unity.Entities;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Latios.Mash.Authoring
{
    [DisallowMultipleComponent]
    [AddComponentMenu("Latios/Mash/Mash Input (Mash)")]
    public class MashAuthoring : MonoBehaviour
    {
        [Tooltip("The .inputactions asset to bring into ECS")]
        public InputActionAsset inputActions;
    }

#if UNITY_EDITOR
    public class MashAuthoringBaker : Baker<MashAuthoring>
    {
        static System.Collections.Generic.Dictionary<string, IMashGeneratedAsset> s_generatedAssetsByGuid;

        public override void Bake(MashAuthoring authoring)
        {
            if (authoring.inputActions == null)
            {
                Debug.LogWarning($"Mash: \"{authoring.name}\" has no Input Actions asset assigned and will be skipped.", authoring);
                return;
            }

            var assetPath = UnityEditor.AssetDatabase.GetAssetPath(authoring.inputActions);
            var guid      = UnityEditor.AssetDatabase.AssetPathToGUID(assetPath);
            var generated = FindGeneratedAsset(guid);
            if (generated == null)
            {
                Debug.LogError(
                    $"Mash: No generated code was found for \"{authoring.inputActions.name}\". Make sure the asset has been saved and the project has recompiled since.",
                    authoring);
                return;
            }

            generated.Bake(this, GetEntity(TransformUsageFlags.None), authoring.inputActions);
        }

        static IMashGeneratedAsset FindGeneratedAsset(string sourceAssetGuid)
        {
            if (s_generatedAssetsByGuid == null)
            {
                s_generatedAssetsByGuid = new System.Collections.Generic.Dictionary<string, IMashGeneratedAsset>();
                foreach (var assembly in System.AppDomain.CurrentDomain.GetAssemblies())
                {
                    System.Type[] types;
                    try
                    {
                        types = assembly.GetTypes();
                    }
                    catch (System.Reflection.ReflectionTypeLoadException e)
                    {
                        types = e.Types;
                    }
                    if (types == null)
                        continue;

                    foreach (var type in types)
                    {
                        if (type == null || type.IsAbstract || type.IsInterface || !typeof(IMashGeneratedAsset).IsAssignableFrom(type))
                            continue;
                        if (System.Activator.CreateInstance(type) is IMashGeneratedAsset descriptor)
                        {
                            // A duplicate GUID means a stale generated file survived a rename or move.
                            // Report it instead of letting assembly order pick a winner.
                            if (s_generatedAssetsByGuid.TryGetValue(descriptor.sourceAssetGuid, out var existing))
                            {
                                Debug.LogError(
                                    $"Mash: {type.FullName} and {existing.GetType().FullName} were both generated for source asset GUID {descriptor.sourceAssetGuid}. One of them is left over from an .inputactions asset that was renamed or moved. Delete the stale generated file.");
                                continue;
                            }
                            s_generatedAssetsByGuid.Add(descriptor.sourceAssetGuid, descriptor);
                        }
                    }
                }
            }
            s_generatedAssetsByGuid.TryGetValue(sourceAssetGuid, out var result);
            return result;
        }
    }
#endif
}
