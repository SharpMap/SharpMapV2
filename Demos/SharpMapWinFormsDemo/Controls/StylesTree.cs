using System;
using System.Collections.Generic;
using System.Text;
using SharpMap.Styles;

namespace MapViewer.Controls
{
    public class StylesTree : CustomTreeView
    {
        public void AddStyleNode(int index, string name, GeometryStyle style)
        {
            Nodes.Insert(index, new GeometryStyleNode(style, name));
        }


    }
}
