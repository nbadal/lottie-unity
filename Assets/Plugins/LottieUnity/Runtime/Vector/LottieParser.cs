using System;
using System.Collections.Generic;
using Lottie.Model;
using Unity.VectorGraphics;
using UnityEngine;

namespace Lottie.Vector
{
    public class LottieParser
    {
        private readonly LottieAnimation _lottie;
        private readonly Scene _scene = new Scene();
        internal readonly List<AnimatedNode> _animators = new List<AnimatedNode>();
        private readonly Dictionary<Layer, SceneNode> _layerNodes = new Dictionary<Layer, SceneNode>();
        private readonly Dictionary<SceneNode, Layer> _nodeLayers = new Dictionary<SceneNode, Layer>();

        public static LottieVectorAnimation Parse(string lottieJson)
        {
            var lottie = LottieAnimation.FromJson(lottieJson);
            return new LottieParser(lottie).Convert();
        }

        private LottieParser(LottieAnimation lottie)
        {
            _lottie = lottie;
        }

        private LottieVectorAnimation Convert()
        {
            CreateLayerHierarchy();

            TraverseLayers(SetLayerTransforms);
            TraverseLayers(NodesFromLayer);

            return new LottieVectorAnimation(
                scene: _scene,
                animators: _animators,
                inPoint: _lottie.InPoint,
                outPoint: _lottie.OutPoint,
                frameRate: _lottie.FrameRate
            );
        }

        private void CreateLayerHierarchy()
        {
            // Build the scene graph
            var idToNodes = new Dictionary<int, SceneNode>();
            foreach (var layer in _lottie.Layers)
            {
                var node = new SceneNode
                {
                    Children = new List<SceneNode>(),
                };
                if (layer.Index.HasValue)
                {
                    idToNodes.Add(layer.Index.Value, node);
                }

                _layerNodes.Add(layer, node);
                _nodeLayers.Add(node, layer);
            }

            _scene.Root = new SceneNode
            {
                Children = new List<SceneNode>(),
            };
            for (var index = _lottie.Layers.Count - 1; index >= 0; index--)
            {
                var layer = _lottie.Layers[index];
                if (layer.Parent.HasValue)
                {
                    var parent = idToNodes[layer.Parent.Value];
                    parent.Children.Add(_layerNodes[layer]);
                }
                else
                {
                    _scene.Root.Children.Add(_layerNodes[layer]);
                }
            }
        }

        private void TraverseScene(Action<SceneNode> action)
        {
            TraverseScene(_scene.Root, action);
        }

        private void TraverseScene(SceneNode node, Action<SceneNode> action)
        {
            action(node);
            foreach (var child in node.Children)
            {
                TraverseScene(child, action);
            }
        }

        private void TraverseLayers(Action<Layer, SceneNode> action)
        {
            foreach (var child in _scene.Root.Children)
            {
                if (!_nodeLayers.TryGetValue(child, out var layer)) return;
                TraverseLayers(layer, action);
            }
        }

        private void TraverseLayers(Layer layer, Action<Layer, SceneNode> action)
        {
            if (!_layerNodes.TryGetValue(layer, out var node)) return;
            action(layer, node);
            foreach (var child in node.Children)
            {
                if (_nodeLayers.TryGetValue(child, out var childLayer))
                {
                    TraverseLayers(childLayer, action);
                }
            }
        }

        private void SetLayerTransforms(Layer l, SceneNode node)
        {
            if (!(l is VisualLayer layer)) return;
            var t = layer.Transform;
            node.Transform = t.ToMatrix2D(_animators);
        }

        private void NodesFromLayer(Layer layer, SceneNode layerNode)
        {
            switch (layer.Type)
            {
                case LayerType.Solid:
                    var solid = (SolidColorLayer)layer;
                    var shape = new Unity.VectorGraphics.Shape();
                    VectorUtils.MakeRectangleShape(shape, new Rect(0, 0, (float)solid.Width, (float)solid.Height));
                    ColorUtility.TryParseHtmlString(solid.Color, out var color);
                    shape.Fill = new SolidFill { Color = color };
                    layerNode.Shapes = new List<Unity.VectorGraphics.Shape> { shape };
                    break;
                case LayerType.Shape:
                    this.ParseShapes(((ShapeLayer)layer).Shapes, layerNode);
                    break;
                case LayerType.PreComp:
                case LayerType.Image:
                case LayerType.Text:
                    throw new NotImplementedException();
                case LayerType.Null:
                case LayerType.Audio:
                case LayerType.VideoPlaceholder:
                case LayerType.ImageSequence:
                case LayerType.Video:
                case LayerType.ImagePlaceholder:
                case LayerType.Guide:
                case LayerType.Adjustment:
                case LayerType.Camera:
                case LayerType.Light:
                case LayerType.Data:
                default:
                    break;
            }
        }
    }
}