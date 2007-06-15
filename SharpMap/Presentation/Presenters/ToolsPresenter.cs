using System;
using System.Collections.Generic;
using System.Text;

using SharpMap.Tools;

namespace SharpMap.Presentation
{
    /// <summary>
    /// The presenter for managing a <see cref="IToolsView">view</see> of <see cref="MapTool"/> instances.
    /// </summary>
    public class ToolsPresenter : BasePresenter<IToolsView>
    {
        /// <summary>
        /// Creates a new instance of a <see cref="ToolsPresenter"/> with the given model and view.
        /// </summary>
        /// <param name="map">The map model to present.</param>
        /// <param name="toolsView">The view to accept input from and keep synchronized with the model.</param>
        public ToolsPresenter(SharpMap.Map map, IToolsView toolsView)
            : base(map, toolsView)
        {
            Map.SelectedToolChanged += new EventHandler(handleSelectedToolChanged);
            View.ToolChangeRequested += new EventHandler<ToolChangeRequestedEventArgs>(handleToolChangeRequested);
            
            // TODO: tool configuration should come from a config file and / or reflection
            List<MapTool> mapTools = new List<MapTool>(new MapTool[] { MapTool.Pan, MapTool.Query, MapTool.ZoomIn, MapTool.ZoomOut });
            View.Tools = mapTools;
        }

        private void handleToolChangeRequested(object sender, ToolChangeRequestedEventArgs e)
        {
            Map.SelectedTool = e.RequestedTool;
        }

        private void handleSelectedToolChanged(object sender, EventArgs e)
        {
            View.SelectedTool = Map.SelectedTool;
        }
    }
}
