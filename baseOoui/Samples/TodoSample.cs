using Ooui;
using System.Collections.Generic;

namespace Samples
{
    public class TodoSample : ISample
    {
        public string Title => "Todo List";

        private class Item : ListItem
        {
            private Element label = new Div();

            private bool _isDone;

            public bool IsDone
            {
                get => _isDone;
                set
                {
                    _isDone = value;
                    label.Style.TextDecoration =
                        _isDone ? "line-through" : "none";
                    label.Style.FontWeight =
                        _isDone ? "normal" : "bold";
                    label.Style.Color =
                        _isDone ? "#999" : "#000";
                }
            }

            public Item(string text)
            {
                ClassName = "list-group-item";
                Style.Cursor = "pointer";
                label.Text = text;
                label.Style.FontWeight = "bold";
                AppendChild(label);
            }
        }

        private Element MakeTodo()
        {
            List items = new List()
            {
                ClassName = "list-group",
            };
            items.Style.MarginTop = "1em";

            Heading heading = new Heading("Todo List");
            Paragraph subtitle = new Paragraph("This is the shared todo list of the world.");
            Paragraph count = new Paragraph("0 chars");
            Form inputForm = new Form
            {
                ClassName = "form-inline"
            };
            Input input = new Input
            {
                ClassName = "form-control"
            };
            Button addbtn = new Button("Add")
            {
                Type = ButtonType.Submit,
                ClassName = "btn btn-primary",
            };
            addbtn.Style.MarginLeft = "1em";
            Button clearbtn = new Button("Clear Completed")
            {
                Type = ButtonType.Submit,
                ClassName = "btn btn-danger",
            };
            void UpdateCount()
            {
                count.Text = $"{input.Value.Length} chars";
            }
            void AddItem()
            {
                if (string.IsNullOrWhiteSpace(input.Value))
                    return;
                Item item = new Item(input.Value);
                item.Click += (s, e) =>
                {
                    item.IsDone = !item.IsDone;
                };
                items.InsertBefore(item, items.FirstChild);
                input.Value = "";
                //UpdateCount();
            }
            addbtn.Click += (s, e) =>
            {
                AddItem();
            };
            inputForm.Submit += (s, e) =>
            {
                AddItem();
            };
            input.Change += (s, e) =>
            {
                UpdateCount();
            };
            clearbtn.Click += (s, e) =>
            {
                List<Node> toremove = new List<Node>();
                foreach (Node node in items.Children)
                {
                    var i = (Item)node;
                    if (i.IsDone)
                        toremove.Add(i);
                }
                foreach (Node i in toremove)
                {
                    items.RemoveChild(i);
                }
            };
            Div app = new Div();
            app.AppendChild(heading);
            app.AppendChild(subtitle);
            inputForm.AppendChild(input);
            inputForm.AppendChild(addbtn);
            inputForm.AppendChild(count);
            app.AppendChild(inputForm);
            app.AppendChild(items);
            app.AppendChild(clearbtn);
            return app;
        }

        public void Publish()
        {
            Element b = MakeTodo();

            UI.Publish("/todo", MakeTodo);
        }

        public Element CreateElement()
        {
            return MakeTodo();
        }
    }
}
