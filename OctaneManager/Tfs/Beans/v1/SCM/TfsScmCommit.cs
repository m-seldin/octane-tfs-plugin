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
using Newtonsoft.Json;
using System.Collections.Generic;

namespace MicroFocus.Adm.Octane.CiPlugins.Tfs.Core.Tfs.Beans.v1.SCM
{
	public class TfsScmCommit
	{
		public string TreeId { get; set; }
		public string CommitId { get; set; }
		public string Comment { get; set; }
		public List<string> Parents { get; set; }
		public string RemoteUrl { get; set; }
		[JsonProperty("_links")]
		public TfsScmCommitLinks Links { get; set; }
		public List<TfsScmCommitChange> Changes { get; set; }
		public TfsScmCommitAuthor Author { get; set; }
		public TfsScmCommitAuthor Committer { get; set; }
		
		public override string ToString()
		{
			return CommitId + ":" + Comment;
		}
	}

	public class TfsScmCommitLinks
	{
		public TfsLink Repository { get; set; }
		public TfsLink Changes { get; set; }
		public TfsLink Web { get; set; }

	}
}
