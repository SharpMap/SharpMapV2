using System.Windows.Forms;
using SharpMap.Data;

namespace MapViewer.Controls
{
    public partial class QueryResults : UserControl
    {
        public QueryResults()
        {
            InitializeComponent();
        }

        public FeatureDataView DataSource
        {
            get { return dataGridView1.DataSource as FeatureDataView; }
            set { dataGridView1.DataSource = value; }
        }
    }
}