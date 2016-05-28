using System;
using System.Drawing;
using System.Windows.Interop;
using System.Windows.Threading;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace vkProject.Controls
{
	public partial class HoverLoading : UserControl
	{
		public HoverLoading()
		{
			InitializeComponent();
		}
		public string Text
		{
			get { return text.Text; }
			set { text.Text = value; }
		}
		public void LoadWheelRotateBegin()
		{
			loading.Play();
		}
		public void LaodWheelRotateStop()
		{
			loading.Pause();
		}
	}
}
