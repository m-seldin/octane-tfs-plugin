﻿
using Newtonsoft.Json;

namespace MicroFocus.Adm.Octane.CiPlugins.Tfs.Core.Tfs.Beans.v1.SCM
{
	public class TfsScmCommitChange
	{
		public string ChangeType { get; set; }
		public TfsScmCommitChangeItem Item { get; set; }

	}

	public class TfsScmCommitChangeItem
	{
		public string Path { get; set; }
		public bool IsFolder { get; set; }
	}
}
