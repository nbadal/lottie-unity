using System.Collections.Generic;
using System.Linq;
using Lottie.Model;
using UnityEngine;

namespace Lottie
{
    internal static class Builder
    {
        public static void Build(LottieAnimationController container, LottieAnimation animation)
        {
            // Throw error if this isn't an empty game object
            if (container.transform.childCount > 0)
            {
                Debug.LogError("LottieAnimationController must be attached to an empty GameObject");
                return;
            }

            // Create new root
            var root = new GameObject(animation.Name);
            root.transform.SetParent(container.transform);
            
            BuildRoot(container, animation);
        }

        private static void BuildRoot(LottieAnimationController container, LottieAnimation animation)
        {
            // Build layers
            var layers = new List<LayerController>();
            foreach (var layer in animation.Layers)
            {
                // Skip if layer is not a VisualLayer
                if (layer is VisualLayer)
                {
                    layers.Add(BuildLayer(container, layer));
                }
            }
            
            // Move layers under their parents
            foreach (var layer in layers)
            {
                if (layer.Data.Parent == null) continue;
                var parent = layers.FirstOrDefault(l => l.Data.Index == layer.Data.Parent);
                    
                if (parent == null)
                {
                    Debug.LogError($"Could not find parent {layer.Data.Parent} for layer {layer.Data.Name}");
                    continue;
                }
                    
                layer.transform.SetParent(parent.transform);
            }
        }

        private static LayerController BuildLayer(LottieAnimationController container, Layer layer)
        {
            // Create a game object for the layer
            var layerObject = new GameObject(layer.Name);
            layerObject.transform.SetParent(container.transform);
            
            // Add a layer component
            var layerComponent = layerObject.AddComponent<LayerController>();
            layerComponent.Build(container, layer);

            return layerComponent;
        }
    }

    public class LayerController : MonoBehaviour
    {
        private LottieAnimationController _controller;
        
        public Layer Data => _data;
        private Layer _data;
        
        public void Build(LottieAnimationController controller, Layer data)
        {
            _controller = controller;
            _data = data;
            
            // Build shapes
            var shapes = BuildShapes();
        }

        private List<ShapeController> BuildShapes()
        {
            var shapes = new List<ShapeController>();
            
            // Build shapes
            // TODO: Determine a list of rendered shapes
            switch (_data.Type)
            {
                case LayerType.Solid:
                    // TODO: Create rectangle shape
                    break;
                case LayerType.Shape:
                    // TODO: Composite shape from layer contents
                    break;
                case LayerType.PreComp:
                    // TODO: Composite shape recursively via refId
                    break;
            }

            return shapes;
        }
    }

    public class ShapeController : MonoBehaviour
    {
    }
}