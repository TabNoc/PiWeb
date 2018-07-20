using Ooui;
using System;
using System.Collections.Generic;
using ButtonXaml;
using Xamarin.Forms;
using Element = Ooui.Element;

namespace Samples
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            Xamarin.Forms.Forms.Init();

            for (var i = 0; i < args.Length; i++)
            {
                var a = args[i];
                switch (args[i])
                {
                    case "-p" when i + 1 < args.Length:
                    case "--port" when i + 1 < args.Length:
                        {
                            int p;
                            if (int.TryParse(args[i + 1], out p))
                            {
                                UI.Port = p;
                            }
                            i++;
                        }
                        break;
                }
            }

            new EntryListViewSample().Publish();
            new ButtonSample().Publish();
            new TodoSample().Publish();
            new DrawSample().Publish();
            new FilesSample().Publish();
            new DisplayAlertSample().Publish();
            new DotMatrixClockSample().Publish();
            new EditorSample().Publish();
            new MonkeysSample().Publish();
            new RefreshListViewSample().Publish();
            new SearchBarSample().Publish();
            new SliderSample().Publish();
            new SwitchListViewSample().Publish();
            new TimePickerSample().Publish();
            new TipCalcSample().Publish();
            new WeatherAppSample().Publish();
            new XuzzleSample().Publish();
            new WebViewSample().Publish();
            new PickerSample().Publish();

            new SampleList().Publish();

            new Test("/test").Publish();

            UI.Present("/");

            Console.ReadLine();
        }

        private class SampleList
        {
            private TabbedPage tabbedPage = new TabbedPage();
            private List _list = new List();
            private Div appElement = new Div();

            public void Publish()
            {
                tabbedPage.Children.Add(new Page());
                tabbedPage.Children.Add(new Page());
                tabbedPage.Publish("/1");
                UI.Publish("/", CreateAppElement);
            }

            private Element CreateAppElement()
            {
                appElement.AppendChild(_list);

                List<string> pathList = new List<string>()
                {
                    "display-alert",
                    "entry-listview",
                    "shared-button",
                    "button",
                    "todo",
                    "draw",
                    "files",
                    "dotmatrixclock",
                    "editor",
                    "monkeys",
                    "refreshlistview",
                    "searchbar",
                    "slider",
                    "switch-listview",
                    "timepicker",
                    "tipcalc",
                    "weatherapp",
                    "xuzzle",
                    "webview",
                    "picker"
                };
                foreach (string path in pathList)
                {
                    Div pathElement = new Div
                    {
                        ClassName = "list-group-item",
                        Style =
                        {
                            Cursor = "pointer",
                            FontWeight = "bold"
                        },
                        Text = path
                    };
                    pathElement.SetAttribute("href", pathElement.Document.Window.Location + path);
                    pathElement.Click += (sender, args) =>
                    {
                        Console.WriteLine(pathElement.Document.Window.Location);
                        pathElement.Document.Window.Location = pathElement.GetAttribute("href", "");
                    };
                    appElement.AppendChild(pathElement);
                }
                return appElement;
            }
        }

        public class Test : Publishable
        {
            protected override Element PopulateAppElement()
            {
                return new TestXamlPage().GetOouiElement();
            }

            public Test(string publishPath) : base(publishPath)
            {
            }
        }

        public class TabNav
        {
        }

        public abstract class Publishable
        {
            protected readonly Div AppElement;
            private string _publishPath;

            protected Publishable(string publishPath)
            {
                _publishPath = publishPath;
                this.AppElement = new Div();
            }

            protected abstract Element PopulateAppElement();

            public void Publish()
            {
                UI.Publish(_publishPath, PopulateAppElement);
            }
        }
    }
}
