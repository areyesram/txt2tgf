using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace aryes
{
    internal partial class MainForm : Form
    {
        internal MainForm()
        {
            InitializeComponent();
        }

        public static readonly BE.Delimiter[] Options =
        {
            new BE.Delimiter {Name = "<Tab>", Delims = new[] {'\t'}},
            new BE.Delimiter {Name = "<Space>", Delims = new[] {' '}},
            new BE.Delimiter {Name = ",", Delims = new[] { ','}},
            new BE.Delimiter {Name = ";", Delims = new[] { ';'}},
            new BE.Delimiter {Name = ">", Delims = new[] {'>'}},
            new BE.Delimiter {Name = "|", Delims = new[] {'|'}}
        };

        private BE.Graph graph;

        private void MainForm_Load(object sender, System.EventArgs e)
        {
            BL.Graph.OnError += message => lblStatus.Text = message;

            var comboBox = cboDelim.ComboBox;
            if (comboBox == null) return;
            comboBox.DataSource = Options;
            comboBox.DisplayMember = "Name";
            comboBox.ValueMember = "ID";
        }

        private void btnSave_Click(object sender, System.EventArgs e)
        {
            if (saveDialog.ShowDialog() != DialogResult.OK) return;
            var comboBox = cboRoot.ComboBox;
            if (comboBox == null) return;
            var g = comboBox.SelectedIndex == 0 ? graph : BL.Graph.Isolate(graph, (string) comboBox.SelectedItem);
            var filename = saveDialog.FileName;
            File.WriteAllText(filename, BL.Graph.GraphToTgf(g), Encoding.Default);
            Process.Start(filename);
        }

        private void textBox1_TextChanged(object sender, System.EventArgs e)
        {
            CalculateGraph();
        }

        private void cboDelim_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            var comboBox = cboDelim.ComboBox;
            if (comboBox == null) return;
            BL.Graph.Delimiters = ((BE.Delimiter)comboBox.SelectedItem).Delims;
            CalculateGraph();
        }

        private void CalculateGraph()
        {
            lblStatus.Text = string.Empty;
            graph = BL.Graph.CsvToGraph(textBox1.Text);
            if (graph == null) return;
            var comboBox = cboRoot.ComboBox;
            var roots = graph.Nodes
                .Where(node => !graph.Edges.Any(edge => edge.Target.Equals(node)))
                .OrderBy(o => o).ToList();
            roots.Insert(0, "<whole graph>");
            if (comboBox != null) comboBox.DataSource = roots;
        }

        private void btnImport_ButtonClick(object sender, System.EventArgs e)
        {

        }
    }
}
