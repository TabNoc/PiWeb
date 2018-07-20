using Ooui;
using System.IO;
using TabNoc.Ooui.Interfaces.AbstractObjects;
using TabNoc.Ooui.Interfaces.Enums;
using TabNoc.Ooui.Pages.WateringWeb.Channels;
using TabNoc.Ooui.Storage.WateringWeb.Channels;

namespace TabNoc.Ooui.PagePublisher.WateringWeb
{
	internal class ChannelsPagePublisher : WateringPublisher
	{
		public ChannelsPagePublisher(string publishPath) : base(publishPath)
		{
			Initialize();
		}

		protected override void Initialize()
		{
			PageStorage<ChannelsData>.Instance.Initialize(LoadDataCallback, SaveDataCallback);
		}

		protected override Element CreatePage()
		{
			ChannelsPage channelsPage = new ChannelsPage(PageStorage<ChannelsData>.Instance);
			channelsPage.AddStyling(StylingOption.MarginRight, 5);
			channelsPage.AddStyling(StylingOption.MarginLeft, 1);
			channelsPage.ClassName += " col-xl-10";
			return channelsPage;
		}

		private void SaveDataCallback(string data)
		{
			FileInfo fileInfo = new FileInfo("demo_Channels.json");
			using (StreamWriter streamWriter = fileInfo.CreateText())
			{
				streamWriter.Write(data);
				streamWriter.Flush();
				streamWriter.Close();
				streamWriter.Dispose();
			}
		}

		private string LoadDataCallback()
		{
			if (File.Exists("demo_Channels.json"))
			{
				FileInfo fileInfo = new FileInfo("demo_Channels.json");
				using (StreamReader streamReader = fileInfo.OpenText())
				{
					return streamReader.ReadToEnd();
				}
			}
			else
			{
				return "";
			}
		}
	}
}
