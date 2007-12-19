// Copyright 2006, 2007 - Rory Plaire (codekaizen@gmail.com)
//
// This file is part of SharpMap.
// SharpMap is free software; you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation; either version 2 of the License, or
// (at your option) any later version.
// 
// SharpMap is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Lesser General Public License for more details.

// You should have received a copy of the GNU Lesser General Public License
// along with SharpMap; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA 

using System;
using System.Collections.Generic;
using SharpMap.Data;
using SharpMap.Geometries;
using SharpMap.Styles;
using SharpMap.Layers;

namespace SharpMap.Rendering.Rendering2D
{
    /// <summary>
    /// The base class for 2D feature renderers which produce labels.
    /// </summary>
    /// <typeparam name="TRenderObject">Type of render object to generate.</typeparam>
	public class BasicLabelRenderer2D<TRenderObject>
        : FeatureRenderer2D<LabelStyle, TRenderObject>, ILabelRenderer<Point2D, Size2D, Rectangle2D, TRenderObject>
    {
		/// <summary>
		/// 
		/// </summary>
		/// <param name="feature"></param>
		/// <returns></returns>
		//private delegate String LabelTextFormatter(FeatureDataRow feature);

		private TextRenderer2D<TRenderObject> _textRenderer;
		private Dictionary<LabelStyle, LabelCollisionDetection2D> collisionDetectors = new Dictionary<LabelStyle, LabelCollisionDetection2D>();
		private Dictionary<LabelStyle, LabelLayer.LabelTextFormatter> textFormatters = new Dictionary<LabelStyle, LabelLayer.LabelTextFormatter>();

        #region Object construction and disposal
        public BasicLabelRenderer2D(TextRenderer2D<TRenderObject> textRenderer, 
            VectorRenderer2D<TRenderObject> vectorRenderer)
            : this(textRenderer, vectorRenderer, StyleTextRenderingHint.SystemDefault)
        {
        }

        public BasicLabelRenderer2D(TextRenderer2D<TRenderObject> textRenderer, 
            VectorRenderer2D<TRenderObject> vectorRenderer, StyleTextRenderingHint renderingHint)
            : base(vectorRenderer)
        {
            if (textRenderer == null) throw new ArgumentNullException("textRenderer");

            TextRenderer = textRenderer;
            TextRenderer.TextRenderingHint = renderingHint;
        }

        ~BasicLabelRenderer2D()
        {
            Dispose(false);
        }
        #endregion

        public TextRenderer2D<TRenderObject> TextRenderer
        {
            get { return _textRenderer; }
            private  set { _textRenderer = value; }
        }

        #region ILabelRenderer<Point2D,ViewSize2D,Rectangle2D,TRenderObject> Members

        public virtual IEnumerable<TRenderObject> RenderLabel(Label2D label)
        {
			Size2D textSize = TextRenderer.MeasureString(label.Text, label.Font);
			//Size2D size = new Size2D(textSize.Width + 2*label.CollisionBuffer.Width, textSize.Height + 2*label.CollisionBuffer.Height);
			//Rectangle2D layoutRectangle =
			//    new Rectangle2D(new Point2D(label.Location.X-label.CollisionBuffer.Width, label.Location.Y-label.CollisionBuffer.Height), size);

			Rectangle2D layoutRectangle =
			    new Rectangle2D(label.Location, textSize);

            if (label.Style.Halo != null)
            {
                StylePen halo = label.Style.Halo;
                StyleBrush background = label.Style.Background;

                IEnumerable<TRenderObject> haloPath = VectorRenderer.RenderPaths(
                    generateHaloPath(layoutRectangle), background, background, background,
                    halo, halo, halo, RenderState.Normal);

                foreach (TRenderObject ro in haloPath)
                {
                    yield return ro;
                }
            }

            IEnumerable<TRenderObject> renderedText = TextRenderer.RenderText(
                label.Text, label.Font, layoutRectangle, label.FlowPath, label.Style.Foreground, label.Transform);

            foreach (TRenderObject ro in renderedText)
            {
                yield return ro;
            }
        }

        #endregion

        protected override IEnumerable<TRenderObject> DoRenderFeature(IFeatureDataRecord inFeature, LabelStyle style,
                                                                      RenderState renderState, ILayer inLayer)
        {
        	FeatureDataRow feature = inFeature as FeatureDataRow;
        	LabelLayer layer = inLayer as LabelLayer;

			if(style == null)
			{
				throw new ArgumentNullException("style", "LabelStyle is a required argument to properly render the label");
			}

        	Label2D newLabel = null;
			LabelLayer.LabelTextFormatter formatter = null;
			LabelCollisionDetection2D collisionDetector = null;

			if (layer != null)
			{
				if(!layer.RenderCache.TryGetValue(feature.GetOid(), out newLabel))
				{
					formatter = layer.TextFormatter;
				}
				collisionDetector = layer.CollisionDetector;
				collisionDetector.TextRenderer = TextRenderer;
			}
			else
			{
				if (!textFormatters.TryGetValue(style, out formatter))
				{
					// setup formatter based on style.LabelFormatExpression
					formatter = delegate(FeatureDataRow feature2)
									{
										return feature2[style.LabelFormatExpression].ToString();
									};

					textFormatters.Add(style, formatter);
				}

				if (!collisionDetectors.TryGetValue(style, out collisionDetector))
				{
					collisionDetector = new LabelCollisionDetection2D();
					collisionDetector.TextRenderer = TextRenderer;
					collisionDetectors.Add(style, collisionDetector);
				}
			}

			if(newLabel == null)
			{
				Point p = feature.Geometry.GetBoundingBox().GetCentroid();
				newLabel = new Label2D(formatter.Invoke(feature), new Point2D(p.X, p.Y), style);

				if (layer != null)
				{
					layer.RenderCache.Add(feature.GetOid(), newLabel);
				}
			}

			// now find out if we even need to render this label...
			if (style.CollisionTest != LabelStyle.CollisionTestType.None)
			{
				if (style.CollisionTest == LabelStyle.CollisionTestType.Simple)
				{
					if (collisionDetector.SimpleCollisionTest(newLabel))
					{
						// we are not going to render this label
						if (layer != null)
						{
							layer.RenderCache.Remove(newLabel);
						}
						yield break;
					}
				}
				else if (style.CollisionTest == LabelStyle.CollisionTestType.Advanced)
				{
					if (collisionDetector.AdvancedCollisionTest(newLabel))
					{
						// we are not going to render this label
						if (layer != null)
						{
							layer.RenderCache.Remove(newLabel);
						}
						yield break;
					}
				}
			}

			foreach(TRenderObject ro in RenderLabel(newLabel))
			{
				yield return ro;
			}
        }

        private static IEnumerable<Path2D> generateHaloPath(Rectangle2D layoutRectangle)
        {
            Path2D path = new Path2D(layoutRectangle.GetVertices(), true);
            yield return path;
        }

        ///// <summary>
        ///// Renders the layer to the <paramref name="view"/>.
        ///// </summary>
        ///// <param name="view"><see cref="IMapView2D"/> to render layer on.</param>
        ///// <param name="region">Area of the map to render.</param>
        //protected void Render(BoundingBox region)
        //{
        //    LabelLayer layer = null;

        //    if (Style.Enabled && Style.MaxVisible >= ViewTransformer.Zoom && Style.MinVisible < ViewTransformer.Zoom)
        //    {
        //        if (layer.DataSource == null)
        //            throw new InvalidOperationException("DataSource property not set on layer '" + layer.LayerName + "'");

        //        TextRenderingHint = Style.TextRenderingHint;
        //        StyleRenderingMode = Style.SmoothingMode;

        //        BoundingBox envelope = ViewTransformer.ViewEnvelope; //View to render

        //        if (layer.CoordinateTransformation != null)
        //            envelope = GeometryTransform.TransformBox(envelope, layer.CoordinateTransformation.MathTransform.Inverse());

        //        //Initialize label collection
        //        List<Label> labels = new List<Label>();

        //        //Render labels
        //        foreach (FeatureDataRow feature in layer.GetFeatures(region))
        //        {
        //            if (layer.CoordinateTransformation != null)
        //                feature.Geometry = GeometryTransform.TransformGeometry(feature.Geometry, layer.CoordinateTransformation.MathTransform);

        //            LabelStyle style = null;

        //            if (this.Theme != null) //If thematics is enabled, lets override the style
        //                style = this.Theme.GetStyle(feature) as LabelStyle;
        //            else
        //                style = this.Style;

        //            Single rotation = 0;

        //            if (layer.RotationColumn != null && layer.RotationColumn != "")
        //                Single.TryParse(feature[layer.RotationColumn].ToString(), NumberStyles.Any, Map.NumberFormat_EnUS, out rotation);

        //            String text = layer.GetLabelText(feature);

        //            if (!String.IsNullOrEmpty(text))
        //            {
        //                if (feature.Geometry is GeometryCollection)
        //                {
        //                    if (layer.MultipartGeometryBehaviour == MultipartGeometryBehaviourEnum.All)
        //                    {
        //                        foreach (Geometry geom in (feature.Geometry as Geometries.GeometryCollection))
        //                        {
        //                            Label lbl = CreateLabel(geom, text, rotation, style);

        //                            if (lbl != null)
        //                                labels.Add(lbl);
        //                        }
        //                    }
        //                    else if (layer.MultipartGeometryBehaviour == MultipartGeometryBehaviourEnum.CommonCenter)
        //                    {
        //                        Label lbl = CreateLabel(feature.Geometry, text, rotation, style);

        //                        if (lbl != null)
        //                            labels.Add(lbl);
        //                    }
        //                    else if (layer.MultipartGeometryBehaviour == MultipartGeometryBehaviourEnum.First)
        //                    {
        //                        if ((feature.Geometry as GeometryCollection).Collection.Count > 0)
        //                        {
        //                            Label lbl = CreateLabel((feature.Geometry as GeometryCollection).Collection[0], text, rotation, style);

        //                            if (lbl != null)
        //                                labels.Add(lbl);
        //                        }
        //                    }
        //                    else if (layer.MultipartGeometryBehaviour == MultipartGeometryBehaviourEnum.Largest)
        //                    {
        //                        GeometryCollection coll = (feature.Geometry as GeometryCollection);
        //                        if (coll.NumGeometries > 0)
        //                        {
        //                            Double largestVal = 0;
        //                            Int32 idxOfLargest = 0;

        //                            for (Int32 j = 0; j < coll.NumGeometries; j++)
        //                            {
        //                                Geometry geom = coll.Geometry(j);

        //                                if (geom is LineString && ((LineString)geom).Length > largestVal)
        //                                {
        //                                    largestVal = ((LineString)geom).Length;
        //                                    idxOfLargest = j;
        //                                }
        //                                if (geom is MultiLineString && ((MultiLineString)geom).Length > largestVal)
        //                                {
        //                                    largestVal = ((LineString)geom).Length;
        //                                    idxOfLargest = j;
        //                                }
        //                                if (geom is Polygon && ((Polygon)geom).Area > largestVal)
        //                                {
        //                                    largestVal = ((Polygon)geom).Area;
        //                                    idxOfLargest = j;
        //                                }
        //                                if (geom is MultiPolygon && ((MultiPolygon)geom).Area > largestVal)
        //                                {
        //                                    largestVal = ((MultiPolygon)geom).Area;
        //                                    idxOfLargest = j;
        //                                }
        //                            }

        //                            Label lbl = CreateLabel(coll.Geometry(idxOfLargest), text, rotation, style);

        //                            if (lbl != null)
        //                                labels.Add(lbl);
        //                        }
        //                    }
        //                }
        //                else
        //                {
        //                    Label lbl = CreateLabel(feature.Geometry, text, rotation, style);
        //                    if (lbl != null)
        //                        labels.Add(lbl);
        //                }
        //            }
        //        }

        //        if (labels.Count > 0) //We have labels to render...
        //        {
        //            if (this.Style.CollisionDetection && layer.LabelFilter != null)
        //                layer.LabelFilter(labels);

        //            foreach (Label label in labels)
        //                DrawLabel(label);
        //        }

        //        labels = null;
        //    }

        //    //base.Render(view, region);
        //}

        ///// <summary>
        ///// Renders a label to the <paramref name="view"/>.
        ///// </summary>
        ///// <param name="text">Text to render.</param>
        ///// <param name="location">Location of the upper left corner of the label.</param>
        ///// <param name="offset">Offset of label in view coordinates from the <paramref name="location"/>.</param>
        ///// <param name="font">Font used for rendering.</param>
        ///// <param name="foreColor">Font color.</param>
        ///// <param name="backColor">Background color.</param>
        ///// <param name="halo">Halo to be drawn around the label text.</param>
        ///// <param name="rotation">Text rotation in degrees.</param>
        //public void DrawLabel(String text, Point2D location, Point2D offset, StyleFont font,
        //    StyleColor foreColor, StyleBrush backColor, StylePen halo, Single rotation)
        //{
        //    DrawLabel(text, location, offset, font, foreColor, backColor, halo, rotation);
        //}

        //private Label CreateLabel(Geometry feature, String text, Single rotation, LabelStyle style)
        //{
        //    LabelLayer layer = null;
        //    ViewSize2D size = MeasureString(text, style.Font);

        //    Point2D position = ViewTransformer.WorldToView(feature.GetBoundingBox().GetCentroid());
        //    Double x = position.X - size.Width * (Int16)style.HorizontalAlignment * 0.5f;
        //    Double y = position.Y - size.Height * (Int16)style.VerticalAlignment * 0.5f;

        //    position = new Point2D(x, y);

        //    if (position.X - size.Width > ViewTransformer.ViewSize.Width || position.X + size.Width < 0 ||
        //        position.Y - size.Height > ViewTransformer.ViewSize.Height || position.Y + size.Height < 0)
        //    {
        //        return null;
        //    }

        //    Label label;

        //    if (!style.CollisionDetection)
        //    {
        //        label = new Label(text, position, rotation, layer.Priority, null, style);
        //    }
        //    else
        //    {
        //        //Collision detection is enabled so we need to measure the size of the String
        //        label = new Label(text, position, rotation, layer.Priority,
        //            new Rectangle2D(position.X - size.Width * 0.5f - style.CollisionBuffer.Width, position.Y + size.Height * 0.5f + style.CollisionBuffer.Height,
        //            size.Width + 2f * style.CollisionBuffer.Width, size.Height + style.CollisionBuffer.Height * 2f), style);
        //    }

        //    if (feature is LineString)
        //    {
        //        LineString line = feature as LineString;

        //        if (line.Length / ViewTransformer.PixelSize > size.Width) //Only label feature if it is Int64 enough
        //            CalculateLabelOnLineString(line, ref label);
        //        else
        //            return null;
        //    }

        //    return label;
        //}

        private void calculateLabelOnLineString(LineString line, ref Label2D label)
        {
            Double dx, dy;
            Double tmpx, tmpy;
            Double angle = 0.0;

            // first find the middle segment of the line
            Int32 midPoint = (line.Vertices.Count - 1)/2;

            if (line.Vertices.Count > 2)
            {
                dx = line.Vertices[midPoint + 1].X - line.Vertices[midPoint].X;
                dy = line.Vertices[midPoint + 1].Y - line.Vertices[midPoint].Y;
            }
            else
            {
                midPoint = 0;
                dx = line.Vertices[1].X - line.Vertices[0].X;
                dy = line.Vertices[1].Y - line.Vertices[0].Y;
            }

            if (dy == 0)
            {
                label.Rotation = 0;
            }
            else if (dx == 0)
            {
                label.Rotation = 90;
            }
            else
            {
                // calculate angle of line					
                angle = -Math.Atan(dy/dx) + Math.PI*0.5;
                angle *= (180d/Math.PI); // convert radians to degrees
                label.Rotation = (Single) angle - 90; // -90 text orientation
            }

            tmpx = line.Vertices[midPoint].X + (dx*0.5);
            tmpy = line.Vertices[midPoint].Y + (dy*0.5);

#warning figure out label positioning
            throw new NotImplementedException();
            //label.LabelPoint = map.WorldToImage(new SharpMap.Geometries.Point(tmpx, tmpy));
        }

        #region ILabelRenderer<Point2D,Size2D,Rectangle2D,TRenderObject> Members

        IEnumerable<TRenderObject> ILabelRenderer<Point2D, Size2D, Rectangle2D, TRenderObject>.RenderLabel(ILabel<Point2D, Size2D, Rectangle2D, Path<Point2D, Rectangle2D>> label)
        {
            if(!(label is Label2D))
            {
                throw new ArgumentException("Parameter must be an instance of type Label2D.", "label");
            }

            return RenderLabel(label as Label2D);
        }

        #endregion

		public override void CleanUp()
		{
			foreach(LabelCollisionDetection2D detector in collisionDetectors.Values)
			{
				detector.CleanUp();
			}
		}
    }
}