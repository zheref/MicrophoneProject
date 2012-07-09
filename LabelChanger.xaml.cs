using System;
using System.Data;
using System.Windows;
using System.Windows.Controls;

namespace MicrophoneProject
{
	/// <summary>
	/// Lógica de interacción para LabelChanger.xaml
	/// </summary>
	public partial class LabelChanger : UserControl
	{
		public event EventHandler<LabelChangingArgs> ActionAtBack;

		public Microfono Mic { get; set; }

		DataSet dataset = new DataSet();
		DataRow dr;

		public LabelChanger()
		{
			this.InitializeComponent();
		}

        public void Set()
        {
            txtNombre.Text = LeerTextoTag();
        }

		string LeerTextoTag()
		{
			dataset.ReadXml("nombres.xml");
			dr = dataset.Tables["nombre"].Rows[Mic.Number - 1];
			string text = dr["nombre_Text"].ToString();
			return text;
		}

		private void btnAction_Click(object sender, RoutedEventArgs e)
		{
			dr["nombre_Text"] = txtNombre.Text;
			dataset.WriteXml("nombres.xml");
			ActionAtBack(this, new LabelChangingArgs() { Mic = this.Mic });
		}
	}

	public class LabelChangingArgs : EventArgs
	{
		public Microfono Mic { get; set; }
	}
}