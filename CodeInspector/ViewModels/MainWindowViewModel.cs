using CodeInspector.Models;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using ReactiveUI;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace CodeInspector.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {

        #region Fields
        ReactiveCommand<Unit, Unit> _run_code;
        string _code_text;
        string _result_text;
        GuiState _state;
        #endregion

        #region Properties
        public string ResultText
        {
            get => _result_text;
            set => this.RaiseAndSetIfChanged(ref _result_text, value);
        }
        public string CodeText
        {
            get => _code_text;
            set => this.RaiseAndSetIfChanged(ref _code_text, value);
        }
        public GuiState GuiStatus
        {
            get => _state;
            set => this.RaiseAndSetIfChanged(ref _state, value);
        }
        public ObservableCollection<LogEntry> Logs { get; } = new ObservableCollection<LogEntry>();
        #endregion

        #region Functions
        public Task<SyntaxTree> ParseTextAsync(string text)
        {
            return Task.Run<SyntaxTree>(() =>
            {
                return CSharpSyntaxTree.ParseText(CodeText);
            });
        }
        public string GetNode(SyntaxNodeOrToken node)
        {
            return $"{node.Kind()}, token: {node.IsToken}, raw kind: {node.RawKind}";
        }
        #endregion

        #region Commands
        public ReactiveCommand<Unit, Unit> RunCodeCommand
        {
            get => _run_code ??= ReactiveCommand.CreateFromTask(async () =>
            {
                GuiStatus = GuiState.Busy;
                ResultText = "";
                var tree = await ParseTextAsync(CodeText);
                var root = tree.GetRoot();
                //var des = root.DescendantNodes().ToList();
                string s = "";
                foreach (var item in root.DescendantNodesAndSelf())
                {
                    s += $"{GetNode(item)}\n";
                }
                CSharpCompilation compilation = CSharpCompilation.Create($"Assembly{DateTime.Now:s)}", new[] { tree});
                Logs.Add(new LogEntry($"Compile compleated: {compilation.LanguageVersion}"));
                ResultText = s;
                GuiStatus = GuiState.Idle;
            });
        }
        #endregion

        #region Constructors
        public MainWindowViewModel()
        {
            CodeText = "class Foo\n{\n\tprivate int _data;\n}\n";
            //Observable.Interval(TimeSpan.FromSeconds(1)).Select(x => new LogEntry($"Text {x}", DateTime.Now, LogLevel.Info))
                                        //.Take(100)
                                        //.Subscribe(x => Logs.Add(x));
        }
        #endregion
    }
}
