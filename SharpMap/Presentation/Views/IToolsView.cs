using System;
using System.Collections.Generic;
using System.Text;

namespace SharpMap.Presentation
{
    public interface IToolsView
    {
        IList<ToolSet> Tools { get; set; }
        event EventHandler ToolSelectionChanged;
        ToolSet SelectedTool { get; set; }
    }
}
