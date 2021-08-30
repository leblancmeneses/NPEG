using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Controls;
using LanguageWorkbench.Algorithms.LanguageWriters;
using Microsoft.Practices.Composite.Presentation.Commands;
using NPEG;
using NPEG.GrammarInterpreter;
using RobustHaven.Windows.Controls.Notifications;
using Telerik.Windows.Zip;

namespace LanguageWorkbench
{
	public class WorkBenchViewModel : INotifyPropertyChanged
	{
		public WorkBenchViewModel()
		{
			GrammarText = @"
				WhiteSpace: [\s\n\t ]+;
				Comment: '/*' (!'*/' .)* '*/';
				(?<S>): (WhiteSpace / Comment)*;
				(?<Digit>): [0-9]+('.'[0-9]+)?;
				(?<Trig>): 'sin(' S Expr S ')' / 'cos(' S Expr S ')';
				Value:  Digit / Trig / '(' S Expr S ')';
				(?<Product\rsc>): Value S ((?<Symbol> '*' / '/') S Value)*;
				(?<Sum\rsc>): Product S ((?<Symbol>'+' / '-') S Product)*;
				(?<Expr\rsc>): S Sum S;
            ";

			InputText = @"
/* ((((12/3)+5-2*(81/9))+1)) */
cos(25)+cos(15+sin(25.333))+3";

			ExportCommand = new DelegateCommand<object>((x) =>
			{
				try
				{
					var dialog = new SaveFileDialog {Filter = "Zip File | *.zip"};
					bool? dialogResult = dialog.ShowDialog();
					if (dialogResult == true)
					{
						using (ZipPackage zipPackage = ZipPackage.Create(dialog.OpenFile()))
						{
							var grammarName = Regex.Replace(dialog.SafeFileName, @"\.zip", string.Empty);
							grammarName = Regex.Replace(grammarName, @"[^a-zA-Z0-9]", string.Empty);
							var writer = GetWriter(grammarName, GrammarText, zipPackage);

							var parseTree = PEGrammar.Load(GrammarText);
							parseTree.Accept(writer);

							writer.Write();
						}
					}
				}
				catch(Exception exception)
				{
					Errors.Add(new ErrorMessage() { Header = "Error Writing", Description = exception.Message });
				}

			}, (x) => IsValidGrammar && SelectedSupportedLanguageItem != null);



			RunCommand = new DelegateCommand<object>((x) =>
			{
				try
				{
					IsValidGrammar = false;

					var parseTree = PEGrammar.Load(GrammarText);
					ParseTree = new List<AExpression>(){parseTree};

					
					InputIterator = new ByteInputIterator(Encoding.UTF8.GetBytes(InputText));
					var visitor = new NpegParserVisitor(InputIterator);
					parseTree.Accept(visitor);

					if (visitor.IsMatch)
					{
						IsValidGrammar = true;

						AbstractSyntaxTree = new List<AstNode>() {visitor.AST};

						ExportCommand.RaiseCanExecuteChanged();
					}
					else
					{
						Errors.Add(new ErrorMessage() { Header = "Mismatch", Description = "rule(s) or input mismatch" });
					}
				}
				catch(Exception exception)
				{
					Errors.Add(new ErrorMessage() { Header = "Rule Problem", Description = "rule syntax is invalid: " + exception.Message });
				}
			});

			SupportedLanguages = new SupportedLanguagesCollection
			{
			    new SupportedLanguageItem() {Text = "C"},
			    new SupportedLanguageItem() {Text = "C++"},
			    new SupportedLanguageItem() {Text = "C#"},
			    new SupportedLanguageItem() {Text = "Java"},
			    new SupportedLanguageItem() {Text = "JavaScript"}
			};
		}



		private WriterBase GetWriter(string grammarName, string grammarText, ZipPackage zipPackage)
		{
			if (SelectedSupportedLanguageItem.Text.Equals("C", StringComparison.InvariantCultureIgnoreCase))
			{
				var c = new CWriter(grammarName, grammarText, zipPackage);
				return c;
			}

			if (SelectedSupportedLanguageItem.Text.Equals("C++", StringComparison.InvariantCultureIgnoreCase))
			{
				var cpp = new CPlusPlusWriter(grammarName, grammarText, zipPackage);
				return cpp;
			}

			if (SelectedSupportedLanguageItem.Text.Equals("C#", StringComparison.InvariantCultureIgnoreCase))
			{
				var csharp = new CSharpWriter(grammarName, grammarText, zipPackage);
				return csharp;
			}

			if (SelectedSupportedLanguageItem.Text.Equals("Java", StringComparison.InvariantCultureIgnoreCase))
			{
				var java = new JavaWriter(grammarName, grammarText, zipPackage);
				return java;
			}

			if (SelectedSupportedLanguageItem.Text.Equals("JavaScript", StringComparison.InvariantCultureIgnoreCase))
			{
				var javascript = new JavaScriptWriter(grammarName, grammarText, zipPackage);
				return javascript;
			}

			throw new NotImplementedException("Writer not supported.");
		}






		private string _grammarText;
		public string GrammarText
		{
			get { return _grammarText; }
			set
			{
				_grammarText = value;

				RaisePropertyChanged("GrammarText");
			}
		}

		private string _inputText;
		public string InputText
		{
			get { return _inputText; }
			set
			{
				_inputText = value;

				RaisePropertyChanged("InputText");
			}
		}

		private bool _isValidGrammar;
		public bool IsValidGrammar
		{
			get { return _isValidGrammar; }
			set
			{
				_isValidGrammar = value;

				RaisePropertyChanged("IsValidGrammar");
			}	

		}

		private SupportedLanguagesCollection _supportedLanguages;
		public SupportedLanguagesCollection SupportedLanguages
		{
			get { return _supportedLanguages; }
			set
			{
				_supportedLanguages = value;

				RaisePropertyChanged("SupportedLanguages");
			}
		}

		private SupportedLanguageItem _selectedSupportedLanguageItem;
		public SupportedLanguageItem SelectedSupportedLanguageItem
		{
			get { return _selectedSupportedLanguageItem; }
			set
			{
				_selectedSupportedLanguageItem = value;

				RaisePropertyChanged("SelectedSupportedLanguageItem");
				ExportCommand.RaiseCanExecuteChanged();
			}
		}

		private DelegateCommand<object> _exportCommand;
		public DelegateCommand<object> ExportCommand
		{
			get { return _exportCommand; }
			set
			{
				_exportCommand = value;
				RaisePropertyChanged("ExportCommand");
			}
		}

		private DelegateCommand<object> _runCommand;
		public DelegateCommand<object> RunCommand
		{
			get { return _runCommand; }
			set
			{
				_runCommand = value;
				RaisePropertyChanged("RunCommand");
			}
		}

		private List<AstNode> _astSyntaxTree;
		public List<AstNode> AbstractSyntaxTree
		{
			get { return _astSyntaxTree; }
			set
			{
				_astSyntaxTree = value;
				RaisePropertyChanged("AbstractSyntaxTree");
			}
		}

		private List<AExpression> _parseTree;
		public List<AExpression> ParseTree
		{
			get { return _parseTree; }
			set
			{
				_parseTree = value;
				RaisePropertyChanged("ParseTree");
			}
		}

		private IInputIterator _inputIterator;
		public IInputIterator InputIterator
		{
			get { return _inputIterator; }
			set
			{
				_inputIterator = value;
				RaisePropertyChanged("InputIterator");
			}
		}
		private ObservableCollection<object> _errors = new ObservableCollection<object>();
		public ObservableCollection<object> Errors
		{
			get { return _errors; }
			set
			{
				_errors = value;
				RaisePropertyChanged("Errors");
			}
		}

		public event PropertyChangedEventHandler PropertyChanged;
		public void RaisePropertyChanged(string propertyName)
		{
			if (PropertyChanged != null)
			{
				PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}
	}
}