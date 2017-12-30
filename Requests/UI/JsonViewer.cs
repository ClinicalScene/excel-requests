using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Windows.Forms;

namespace Requests.UI
{
    public partial class JsonViewer : Form
    {
        public JsonViewer(string title, JToken jtoken)
        {
            InitializeComponent();
            this.Text = title;
            this.richTextBox1.Text = jtoken.ToString(Formatting.Indented);
        }

    }
}
