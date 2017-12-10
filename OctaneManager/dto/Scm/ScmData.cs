﻿using MicroFocus.Ci.Tfs.Octane.dto;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace MicroFocus.Ci.Tfs.Octane.Dto.Scm
{
	public class ScmData : IDtoBase
    {
		[JsonProperty("repository")]
		public ScmRepository Repository { get; set; }
		[JsonProperty("builtRevId")]
		public string BuiltRevId { get; set; }
		[JsonProperty("commits")]
		public List<ScmCommit> Commits { get; set; }
    }
}
