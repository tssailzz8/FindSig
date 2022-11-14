using System.Diagnostics;
using System.IO;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Xml.Linq;

namespace 找sig
{
	internal class Program
	{
		public static Dictionary<string, Infor> sig = new Dictionary<string, Infor>();
		static void Main(string[] args)
		{
			string 目录位置 = System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase;
			var directory = new DirectoryInfo(目录位置).GetFiles();
			bool 是否有ff=false;
			string path = null;
			foreach (var item in directory)
			{
				if (item.Name== "ffxiv_dx11.exe")
				{
					path= 目录位置;
					是否有ff = true;
				}
			}
			if (!是否有ff)
			{
				Console.WriteLine("输入exe文件夹位置");
			   path = Console.ReadLine();
			}
			
			Console.WriteLine("输入插件文件夹位置");
			var plugins = Console.ReadLine();
			Console.WriteLine("是否只输出未找到:请输入y或者n");
			var 是否只输出没找到 = Console.ReadKey().Key.ToString();
			Console.WriteLine("");
			bool flag = false; ;
			switch (是否只输出没找到)
			{
				case "Y":
					flag = true;
					break;
				case "N":
					flag = false;
					break;
			}
			Stopwatch sw = new Stopwatch();
			sw.Start();
			GetAllSigs(plugins);

			//var path = @"F:\FINALFANTASYXIV\SquareEnix\ff14\game";
			var offest = new Offsets(Path.Combine(path, "ffxiv_dx11.exe"));
			foreach (var item in sig)
			{
				var a = offest.ScanText(item.Key);
				if (a>0&&flag)
				{
					var b = a + 0x140000000;
					Console.WriteLine($"找到{item.Key}的偏移为{b:x4},文件在{item.Value.path}的{item.Value.line}行");
				}
				else
				{
					
					
						Console.WriteLine($"没找到{item.Key}的偏移,文件在{item.Value.path}的{item.Value.line}行");
					
					
				}
			}
			offest.UnInitialize();
			sw.Stop();
			TimeSpan ts = sw.Elapsed;
			Console.WriteLine(ts);
			Console.ReadKey();
		}
		public static void GetAllSigs(string 目录位置)
		{
			//var 目录位置 = @"G:\国服\zoom";
			
			foreach (var item in GetAllFiles(目录位置))
			{
				if (item.Extension == ".cs")
				{
					using (StreamReader sr = new StreamReader(item.FullName))
					{
						int count = 0;
						while (true)
						{
							var text = sr.ReadLine();
							count++;
							if (text == null)
							{
								break;
							}
							else
							{
								var pattern = "\\\"(([0-9a-fA-F?]{2}|\\?) )*([0-9a-fA-F?]{2}|\\?)\\\"";
								if (System.Text.RegularExpressions.Regex.IsMatch(text, pattern))
								{
									var result = System.Text.RegularExpressions.Regex.Match(text, pattern).Groups[1].Value;
									if (!sig.ContainsKey(result))
									{
										var infor = new Infor() { path=item.FullName,line=count};
										sig.Add(result, infor);
									}

								}
							}
						}


					}
				}
			}
		}
		public static FileInfo[] GetAllFiles(string path)
		{
			try
			{
				var files = new List<FileInfo>();
				files.AddRange(new DirectoryInfo(path).GetFiles());//获取文件夹下所有文件
				var tmpdics = new DirectoryInfo(path).GetDirectories();//获取文件夹下所有子文件夹
				foreach (var dic in tmpdics)
				{
					files.AddRange(GetAllFiles(dic.FullName));//递归获取文件
				}
				return files.ToArray();
			}
			catch (Exception)
			{
				return null;
			}
		}
	}
	class Infor
	{
		public string path { get; set; }
		public int line { get; set; }
	}
}
