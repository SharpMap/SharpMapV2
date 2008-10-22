using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using SharpMap.Styles;

namespace MapViewer.Controls
{
    public partial class StylesControl : UserControl
    {
        private IBindingList styles;

        public StylesControl()
        {
            InitializeComponent();
        }

        public IBindingList Styles
        {
            get { return styles; }
            set
            {
                if (styles != null)
                    Unwire(styles);
                styles = value;
                if (styles != null)
                    Wire(styles);
            }
        }

        private void Wire(IBindingList styles)
        {
            styles.ListChanged += styles_ListChanged;
        }

        private void styles_ListChanged(object sender, ListChangedEventArgs e)
        {
            if (e.ListChangedType == ListChangedType.ItemAdded)
            {
                string name = "";
                GeometryStyle style = null;

                object o = styles[e.NewIndex];
                if (o is KeyValuePair<string, GeometryStyle>)
                {
                    KeyValuePair<string, GeometryStyle> kvp = (KeyValuePair<string, GeometryStyle>)o;
                    name = kvp.Key;
                    style = kvp.Value;
                }


                stylesTree1.AddStyleNode(e.NewIndex, name, style);
            }
            else if (e.ListChangedType == ListChangedType.ItemDeleted)
            {
                stylesTree1.Nodes.RemoveAt(e.OldIndex);
            }
        }

        private void Unwire(IBindingList styles)
        {
            styles.ListChanged -= styles_ListChanged;
        }
    }
}