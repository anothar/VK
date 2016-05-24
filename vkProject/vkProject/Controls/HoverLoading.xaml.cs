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
			if(!_rotating)
			{
				_source = GetSource();
				loading.Source = _source;
				ImageAnimator.Animate(_bitmap, OnFrameChanged);
				_rotating = true;
			}
		}
		public void LaodWheelRotateStop()
		{
			if(_rotating)
				ImageAnimator.StopAnimate(_bitmap, OnFrameChanged);
		}


		private BitmapSource GetSource()
		{
			if(_bitmap == null)
			{
				_bitmap = new Bitmap("712.gif");
			}
			IntPtr handle = IntPtr.Zero;
			handle = _bitmap.GetHbitmap();
			return Imaging.CreateBitmapSourceFromHBitmap(handle, IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
		}
		private void FrameUpdatedCallback()
		{
			ImageAnimator.UpdateFrames();
			if(_source != null)
				_source.Freeze();
			_source = GetSource();
			loading.Source = _source;
			InvalidateVisual();
		}
		private void OnFrameChanged(object sender, EventArgs e)
		{
			Dispatcher.BeginInvoke(DispatcherPriority.Send,
									new Action(FrameUpdatedCallback));
		}
		private BitmapSource _source;
		private Bitmap _bitmap;
		private bool _rotating = false;

	}
}
