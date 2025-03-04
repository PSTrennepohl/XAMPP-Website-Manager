using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using XWM.Application;

namespace XWM.File;

internal class HostFile
{
	public void Write(List<Vhost> vHosts)
	{
		try
		{
			string hostFile = Config.Instance.GetHostFile();
			string text = "\t\t# " + System.Windows.Forms.Application.ProductName;
			string[] array = System.IO.File.ReadAllLines(hostFile);
			int num = 0;
			string[] array2 = array;
			for (int i = 0; i < array2.Length; i++)
			{
				if (array2[i].IndexOf(text) > -1)
				{
					array[num] = null;
				}
				num++;
			}
			System.IO.File.WriteAllLines(hostFile, array.Where((string s) => !string.IsNullOrEmpty(s)).ToArray());
			using StreamWriter streamWriter = new StreamWriter(hostFile);
			foreach (Vhost vHost in vHosts)
			{
				streamWriter.WriteLine("127.0.0.1        " + vHost.ServerName.Trim() + text);
			}
		}
		catch (Exception ex)
		{
			if (ex is FileNotFoundException || ex is IOException)
			{
				MessageBox.Show(Main.Instance(), "Unable to write to host file - please check that you have run this program with administrator privileges.", Config.Instance.GetTranslation("Error"), MessageBoxButtons.OK, MessageBoxIcon.Hand);
			}
			else
			{
				MessageBox.Show(Main.Instance(), ex.Message, Config.Instance.GetTranslation("Error"), MessageBoxButtons.OK, MessageBoxIcon.Hand);
			}
		}
	}
}
