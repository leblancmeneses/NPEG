using System.Windows.Controls;

namespace LanguageWorkbench
{
	public partial class WorkBenchView : UserControl
	{
		public WorkBenchView()
		{
			InitializeComponent();
		}

		public WorkBenchViewModel ViewModel
		{
			get { return DataContext as WorkBenchViewModel; }
			set
			{
				DataContext = value;

				value.PropertyChanged += ViewModelPropertyChanged;
			}
		}

		void ViewModelPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			if (e.PropertyName == "AbstractSyntaxTree")
			{
				UIHelper.SetTimeout(500, () => astDiagram.ExpandAllHierarchyItems());
			}

			if (e.PropertyName == "ParseTree")
			{
				UIHelper.SetTimeout(1000, () => parseTree.ExpandAll());
			}
		}
	}
}