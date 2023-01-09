/***********************************************************************************
* File:         MyCommand.cs                                                       *
* Contents:     Class MyCommand                                                    *
* Author:       Stanislav Koncebovski (stanislav@pikkatech.eu)                     *
* Date:         2023-01-05 16:12                                                   *
* Version:      1.0                                                                *
* Copyright:    pikkatech.eu (www.pikkatech.eu)                                    *
***********************************************************************************/

using Community.VisualStudio.Toolkit;
using FlowerPot;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Text;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using Task = System.Threading.Tasks.Task;

namespace Saskia
{
	[Command(PackageIds.MyCommand)]
	internal sealed class MyCommand : BaseCommand<MyCommand>
	{
		public static string RegistryKey { get; set; } = "";

		static MyCommand()
		{
			RegistryKey = $"Software\\{UserData.GlobalCompanyName}\\{UserData.GlobalProductName}";
		}

		protected override async Task ExecuteAsync(OleMenuCmdEventArgs e)
		{
			UserData userData = null;

			try
			{
				userData = UserData.FromRegistry(RegistryKey);
			}
			catch (Exception)
			{
				userData = new UserData();
			}

			DocumentView docView = await VS.Documents.GetActiveDocumentViewAsync();

			if (docView == null)
			{
				await VS.MessageBox.ShowWarningAsync("FlowerPot", $"docView == null");
			}

			string flowerPot = this.CreateFlowerPot(docView, userData);

			docView.TextBuffer.Insert(0, flowerPot);
		}

		private string CreateFlowerPot(DocumentView docView, UserData userData)
		{
			var selection = docView.TextView.Selection;
			var currentSnapshot = docView.TextBuffer.CurrentSnapshot;
			var currentLines = currentSnapshot.Lines;

			List<string> lines = new List<string>();

			foreach (var line in currentLines)
			{
				lines.Add(line.GetText());
			}

			string entity = "Module";
			string version = "0.0";
			string fileName = Path.GetFileName(docView.TextBuffer.GetFileName());
			string entityName = Path.GetFileNameWithoutExtension(docView.TextBuffer.GetFileName());

			foreach (string line in lines)
			{
				if (line.Contains("class "))
				{
					entity = "Class";
					Regex rxClass = new Regex("^.*class (?'Class'\\w*)");
					entityName = rxClass.Match(line).Groups["Class"].Value;
					break;
				}
				else
				{
					entity = "Module";
				}
			}

			string[] fileHeader = new string[8];

			fileHeader[0] = $"'''";
			fileHeader[1] = $"File:         {fileName}";
			fileHeader[2] = $"Contents:     {entity} {entityName}";
			fileHeader[3] = $"Author:       {userData.AuthorName} ({userData.AuthorEmail})";
			fileHeader[4] = $"Date:         {DateTime.Now.ToString("yyyy-MM-dd HH:mm")}";
			fileHeader[5] = $"Version:      {version}";
			fileHeader[6] = $"Copyright:    {userData.CompanyName} ({userData.CompanyWebsite})";
			fileHeader[7] = $"'''";

			string result = string.Join("\n", fileHeader) + "\n\n";

			return result;
		}
	}
}
