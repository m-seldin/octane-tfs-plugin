﻿/*!
* (c) 2016-2018 EntIT Software LLC, a Micro Focus company
*
* Licensed under the Apache License, Version 2.0 (the "License");
* you may not use this file except in compliance with the License.
* You may obtain a copy of the License at
*
* http://www.apache.org/licenses/LICENSE-2.0
*
* Unless required by applicable law or agreed to in writing, software
* distributed under the License is distributed on an "AS IS" BASIS,
* WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
* See the License for the specific language governing permissions and
* limitations under the License.
*/
using log4net;
using MicroFocus.Adm.Octane.CiPlugins.Tfs.Core.Tools.Connectivity;
using System;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace MicroFocus.Adm.Octane.CiPlugins.Tfs.Core.Configuration
{
	public static class ConfigurationManager
	{
		private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
		private static string _configurationFile = "octane.conf.json";
		private static FileSystemWatcher _watcher;
		private static Task configurationFileChangedTask;

		public static event EventHandler ConfigurationChanged;

		static ConfigurationManager()
		{
			Init();
		}

		public static void Init()
		{
			Paths.CreateDirIfMissing(Paths.MainFolder);
			WatchForConfigFileChanges();
		}

		public static ConnectionDetails Read(bool printLogs)
		{
			var fullConfigFilePath = GetConfigFilePath();
			if (printLogs)
			{
				Log.Debug($"Loading configuration from {fullConfigFilePath}");
			}

			if (!File.Exists(fullConfigFilePath))
			{
				throw new FileNotFoundException($"Configuration file {fullConfigFilePath} was not found!");
			}

			string configJson = null;
			using (FileStream fileStream = new FileStream(fullConfigFilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
			{
				using (StreamReader streamReader = new StreamReader(fileStream))
				{
					configJson = streamReader.ReadToEnd();
				}
			}

			var res = JsonHelper.DeserializeObject<ConnectionDetails>(configJson);
			res.Decrypt();

			if (printLogs)
			{
				Log.Info($"Loaded configuration : {JsonHelper.SerializeObject(res.GetInstanceWithoutSensitiveInfo(), true)}");
			}

			return res;
		}

		public static bool ConfigurationExists()
		{
			return File.Exists(GetConfigFilePath());
		}

		private static string GetConfigFilePath()
		{
			return Path.Combine(Paths.MainFolder, _configurationFile);
		}

		public static void WriteConfig(ConnectionDetails config)
		{
			var configFile = GetConfigFilePath();
			var configText = JsonHelper.SerializeObject(config.GetSecuredInstance(), true);
			File.WriteAllText(configFile, configText);
			Log.Info($"Writing configuration done");
		}

		private static void WatchForConfigFileChanges()
		{
			if (_watcher == null)
			{
				// Create a new FileSystemWatcher and set its properties.
				_watcher = new FileSystemWatcher();
				_watcher.Path = Paths.MainFolder;
				_watcher.Filter = _configurationFile;
				_watcher.NotifyFilter = NotifyFilters.FileName | NotifyFilters.Size;

				// Add event handlers.
				_watcher.Changed += ConfigurationFileChanged;
				_watcher.Created += ConfigurationFileChanged;
				_watcher.Deleted += ConfigurationFileChanged;
				_watcher.Renamed += ConfigurationFileChanged;
				_watcher.EnableRaisingEvents = true;
			}
		}

		/*!
		 * Used for connDetails that was received from UI with replaced sensitive info by SENSITIVE_VALUE_REPLACER.
		 * Here it will be replaced back to real values.
		 * 
		 */
		public static void ResetSensitiveInfo(ConnectionDetails connDetails)
		{
			ConnectionDetails connFromFs = Read(false);
			connFromFs.ResetSensitiveInfoTo(connDetails);
		}

		private static void ConfigurationFileChanged(object sender, FileSystemEventArgs e)
		{
			//We want to delay invoking of event in 5 secs, why?
			//1.FileSystemWatcher may call this method twice
			//2.User click on save configuration more than once
			if (configurationFileChangedTask == null)
			{
				configurationFileChangedTask = Task.Factory.StartNew(() =>
				{
					Thread.Sleep(5000);
					configurationFileChangedTask = null;
					Log.Info($"Configuration changed on FileSystem");
					ConfigurationChanged?.Invoke(sender, e);
				});
			}
		}
	}
}
