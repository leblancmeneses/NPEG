using System.ComponentModel;

namespace LanguageWorkbench
{
	public class SupportedLanguageItem : INotifyPropertyChanged
	{
		private string _text = string.Empty;
		public string Text
		{
			get { return _text; }
			set
			{
				if (_text != value)
				{
					_text = value;
					RaisePropertyChanged("Text");
				}
			}
		}



		public event PropertyChangedEventHandler PropertyChanged;


		protected void RaisePropertyChanged(string propertyName)
		{
			if (PropertyChanged != null)
			{
				PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}
	}
}