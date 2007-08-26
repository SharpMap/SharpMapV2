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

using SharpMap.Geometries;
using SharpMap.Styles;

namespace SharpMap.Rendering.Rendering2D
{
    /// <summary>
    /// The base class for 2D feature renderers which produce labels.
    /// </summary>
    /// <typeparam name="TRenderObject">Type of render object to generate.</typeparam>
    public abstract class LabelRenderer2D<TRenderObject> 
		: FeatureRenderer2D<LabelStyle, TRenderObject>, ILabelRenderer<Point2D, Size2D, Rectangle2D, TRenderObject>
    {
        private StyleTextRenderingHint _textRenderingHint;

        protected LabelRenderer2D(VectorRenderer2D<TRenderObject> vectorRenderer, 
			StyleTextRenderingHint renderingHint) : base(vectorRenderer)
        {
            _textRenderingHint = renderingHint;
        }

        ~LabelRenderer2D()
        {
            Dispose(false);
        }

        #region ILabelRenderer<ViewPoint2D,ViewSize2D,ViewRectangle2D,TRenderObject> Members

        public StyleTextRenderingHint TextRenderingHint
        {
            get 
            { 
                return _textRenderingHint; 
            }
            set
            {
                if (_textRenderingHint != value)
                {
                    StyleTextRenderingHint oldValue = _textRenderingHint;
                    _textRenderingHint = value;
                    OnTextRenderingHintChanged();
                }
            }
        }

        public abstract Size2D MeasureString(string text, StyleFont font);
        public abstract TRenderObject RenderLabel(Label2D label);
        public abstract TRenderObject RenderLabel(string text, Point2D location, StyleFont font, StyleColor foreColor);
        public abstract TRenderObject RenderLabel(string text, Point2D location, Point2D offset, StyleFont font, StyleColor foreColor, StyleBrush backColor, StylePen halo, float rotation);
        #endregion

        protected override IEnumerable<PositionedRenderObject2D<TRenderObject>> DoRenderFeature(
			FeatureDataRow feature, LabelStyle style)
        {
            throw new NotImplementedException();
        }

        public event EventHandler TextRenderingHintChanged;

        protected virtual void OnTextRenderingHintChanged()
        {
            EventHandler @event = TextRenderingHintChanged;

            if (@event != null)
            {
                @event(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Renders the layer to the <paramref name="view"/>.
        /// </summary>
        /// <param name="view"><see cref="IMapView"/> to render layer on.</param>
        /// <param name="region">Area of the map to render.</param>
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

        //            float rotation = 0;

        //            if (layer.RotationColumn != null && layer.RotationColumn != "")
        //                float.TryParse(feature[layer.RotationColumn].ToString(), NumberStyles.Any, Map.NumberFormat_EnUS, out rotation);

        //            string text = layer.GetLabelText(feature);

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
        //                            double largestVal = 0;
        //                            int idxOfLargest = 0;

        //                            for (int j = 0; j < coll.NumGeometries; j++)
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

        /// <summary>
        /// Renders a label to the <paramref name="view"/>.
        /// </summary>
        /// <param name="text">Text to render.</param>
        /// <param name="location">Location of the upper left corner of the label.</param>
        /// <param name="offset">Offset of label in view coordinates from the <paramref name="location"/>.</param>
        /// <param name="font">Font used for rendering.</param>
        /// <param name="foreColor">Font color.</param>
        /// <param name="backColor">Background color.</param>
        /// <param name="halo">Halo to be drawn around the label text.</param>
        /// <param name="rotation">Text rotation in degrees.</param>
        //public void DrawLabel(string text, ViewPoint2D location, ViewPoint2D offset, StyleFont font,
        //    StyleColor foreColor, StyleBrush backColor, StylePen halo, float rotation)
        //{
        //    DrawLabel(text, location, offset, font, foreColor, backColor, halo, rotation);
        //}

        //private Label CreateLabel(Geometry feature, string text, float rotation, LabelStyle style)
        //{
        //    LabelLayer layer = null;
        //    ViewSize2D size = MeasureString(text, style.Font);

        //    ViewPoint2D position = ViewTransformer.WorldToView(feature.GetBoundingBox().GetCentroid());
        //    double x = position.X - size.Width * (short)style.HorizontalAlignment * 0.5f;
        //    double y = position.Y - size.Height * (short)style.VerticalAlignment * 0.5f;

        //    position = new ViewPoint2D(x, y);

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
        //        //Collision detection is enabled so we need to measure the size of the string
        //        label = new Label(text, position, rotation, layer.Priority,
        //            new ViewRectangle2D(position.X - size.Width * 0.5f - style.CollisionBuffer.Width, position.Y + size.Height * 0.5f + style.CollisionBuffer.Height,
        //            size.Width + 2f * style.CollisionBuffer.Width, size.Height + style.CollisionBuffer.Height * 2f), style);
        //    }

        //    if (feature is LineString)
        //    {
        //        LineString line = feature as LineString;

        //        if (line.Length / ViewTransformer.PixelSize > size.Width) //Only label feature if it is long enough
        //            CalculateLabelOnLineString(line, ref label);
        //        else
        //            return null;
        //    }

        //    return label;
        //}

        private void CalculateLabelOnLineString(LineString line, ref Label2D label)
        {
            double dx, dy;
            double tmpx, tmpy;
            double angle = 0.0;

            // first find the middle segment of the line
            int midPoint = (line.Vertices.Count - 1) / 2;

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
                angle = -Math.Atan(dy / dx) + Math.PI * 0.5;
                angle *= (180d / Math.PI); // convert radians to degrees
                label.Rotation = (float)angle - 90; // -90 text orientation
            }

            tmpx = line.Vertices[midPoint].X + (dx * 0.5);
            tmpy = line.Vertices[midPoint].Y + (dy * 0.5);

#warning figure out label positioning
            throw new NotImplementedException();
            //label.LabelPoint = map.WorldToImage(new SharpMap.Geometries.Point(tmpx, tmpy));
        }

        #region Explicit Interface Implementation

        #region ILabelRenderer<ViewPoint2D,ViewSize2D,ViewRectangle2D,TRenderObject> Members

        TRenderObject ILabelRenderer<Point2D,Size2D,Rectangle2D,TRenderObject>.RenderLabel(ILabel<Point2D, Rectangle2D, GraphicsPath<Point2D, Rectangle2D>> label)
        {
            return RenderLabel(label as Label2D);
        }

        #endregion
        #endregion
    }
}
