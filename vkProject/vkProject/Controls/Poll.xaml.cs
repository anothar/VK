﻿using System;
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
	public partial class ctrPoll : UserControl
	{
		public ctrPoll()
		{
			InitializeComponent();
		}
		public ctrPoll(Poll poll)
		{
			InitializeComponent();
			AnsId = poll.Answer_id;
			foreach(var d in poll.Answers)
			{
				AddAnswer(d);
			}
			Question = poll.Question;
		}

		public string Question
		{
			get { return question.Text; }
			set { question.Text = value; }
		}
		public int AnsId { get; set; }

		public void AddAnswer(Answer ans)
		{
			Lanswers.Add(ans);
			answers.Children.Add(new ctrPollAnswer(ans, AnsId));
		}

		List<Answer> Lanswers = new List<Answer>();

	}
}
