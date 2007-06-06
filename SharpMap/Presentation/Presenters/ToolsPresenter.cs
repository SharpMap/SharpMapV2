using System;
using System.Collections.Generic;
using System.Text;

namespace SharpMap.Presentation
{
    public class ToolsPresenter
    {
        private readonly List<IToolsView> _views;
        private SharpMap.Map.Map _map;

        protected ToolsPresenter(SharpMap.Map.Map map, IEnumerable<IToolsView> toolsViews)
        {
            _map = map;
            _views = new List<IToolsView>(toolsViews);

            foreach (IToolsView toolView in Views)
                toolView.ToolSelectionChanged += new EventHandler(ToolsView_ToolSelectionChanged);
        }

        public SharpMap.Map.Map Map
        {
            get { return _map; }
        }

        public IList<IToolsView> Views
        {
            get { return _views; }
        }

        private void ToolsView_ToolSelectionChanged(object sender, EventArgs e)
        {
            IToolsView senderView = sender as IToolsView;

            Map.SelectedTool = senderView.SelectedTool;

            foreach (IToolsView toolView in Views)
            {
                // if toolView isn't the view on which the event was raised, change the selected tool
                if (!Object.ReferenceEquals(senderView, toolView))
                    toolView.SelectedTool = Map.SelectedTool;
            }
        }
    }
}
