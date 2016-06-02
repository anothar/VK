using System;
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
using VkAPI.Media;

namespace VkAPI.Controls
{
	public partial class ctrPollAnswer : UserControl, IAnswer
	{
		public ctrPollAnswer()
		{
			InitializeComponent();
		}
		public ctrPollAnswer(IAnswer ans, int ansId)
		{
			InitializeComponent();

			Id		= ans.Id;
			Text	= ans.Text;
			Rate	= ans.Rate;
			Votes	= ans.Votes;

			if(ansId == Id)
				Answered = true;
		}
		public string Text
		{
			get
			{
				return text.Text;
			}
			private set
			{
				text.Text = value;
			}
		}
		public double Rate
		{
			get
			{
				return rate.Value;
			}
			private set
			{
				rate.Value = value;
				rate_pers.Text = rate.Value.ToString() + "%";
			}
		}
		public uint Votes
		{
			get
			{
				return Convert.ToUInt32(mans.Text);
			}
			private set
			{
				mans.Text = value.ToString();
			}
		}
		public bool Answered
		{
			get
			{
				return answered;
			}
			private set
			{
				if(value)
					text.FontWeight = FontWeights.Bold;
				else
					text.FontWeight = FontWeights.Normal;
			}
		}
		public int Id { get; private set; }
		private bool answered = false;
	}
}
