using System.Windows.Forms;
using SharpMap.Data;

namespace MapViewer.Controls
{
    public class QueryResultsTab : TabPage
    {
        protected QueryResults results = new QueryResults();
        public QueryResultsTab(string title, FeatureDataView features)
            : base(title)
        {
            Controls.Add(results);
            results.Dock = DockStyle.Fill;
            results.DataSource = features;
        }


    }
}