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
	public partial class ctrPoll : UserControl, IPoll
	{
		public ctrPoll()
		{
			InitializeComponent();
		}
		public ctrPoll(IPoll poll)
		{
			//------Инициализация-членов-интерфейса------\\
			Id			= poll.Id;
			Owner_id	= poll.Owner_id;
			Votes		= poll.Votes;
			Answer_id	= poll.Answer_id;
			Question	= poll.Question;
			Answers		= poll.Answers;
			//-------------------------------------------\\

			foreach(Answer ans in Answers)
				AnserPanel.Add(new ctrPollAnswer(ans, Answer_id));

			InitializeComponent();
		}

		public string Question
		{
			get { return question.Text; }
			set { question.Text = value; }
		}
		public int Id				{ get; private set; }
		public int Owner_id			{ get; private set; }
		public int Votes			{ get { return vot; } private set { votes.Text = String.Format("Проголосовало {0} человек", value); vot = value; } }
		public int Answer_id		{ get; private set; }
		public List<Answer> Answers { get; private set; }
		public UIElementCollection AnserPanel { get { return answers.Children; } }
		private int vot = 0;
	}
}
