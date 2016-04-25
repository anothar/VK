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

namespace VkAPI.Controls
{
	public partial class ctrPollAnswer : UserControl
	{
		public ctrPollAnswer()
		{
			InitializeComponent();
		}
		public ctrPollAnswer(Media.Answer ans, int ansId)
		{
			InitializeComponent();
			Text = ans.Text;
			Rate = ans.Rate;
			Answers = ans.Votes;
			if(ansId == ans.Id)
				Answered = true;
		}
		public string Text
		{
			get
			{
				return text.Text;
			}
			set
			{
				text.Text = value;
			}
		}
		public double Rate
		{
			get
			{
				return rate.Width / 3;
			}
			set
			{
				rate.Width = value * 3;
				rate_pers.Text = value.ToString() + '%';
			}
		}
		public uint Answers
		{
			get
			{
				return Convert.ToUInt32(mans.Text);
			}
			set
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
			set
			{
				if(value)
					text.FontWeight = FontWeights.Bold;
				else
					text.FontWeight = FontWeights.Normal;
			}
		}

		bool answered = false;
	}
}
