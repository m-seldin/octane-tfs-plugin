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
using System;
using MicroFocus.Adm.Octane.CiPlugins.Tfs.Core.Configuration.Credentials;

namespace MicroFocus.Adm.Octane.CiPlugins.Tfs.Core.Configuration
{
	public class ConnectionDetails : ICloneable
	{
		[JsonIgnore]
		public string Host
		{
			get
			{
				Uri uri = new Uri(ALMOctaneUrl);
				string host = $"{uri.Scheme}://{uri.Host}:{uri.Port}";
				return host;
			}
		}

		[JsonIgnore]
		public int SharedSpace
		{
			get
			{
				try
				{
					var index = ALMOctaneUrl.IndexOf("=", StringComparison.Ordinal);
					var sharedSpaceStr = ALMOctaneUrl.Substring(index + 1);
					var endIndex = sharedSpaceStr.IndexOf("/", StringComparison.Ordinal);
					if (endIndex > -1)
					{
						sharedSpaceStr = sharedSpaceStr.Substring(0, endIndex);
					}
					var res = int.Parse(sharedSpaceStr);
					return res;
				}
				catch (Exception)
				{
					throw new Exception($"Sharedspace id is expected, but not received in {ALMOctaneUrl}");
				}
			}
		}
		public string ALMOctaneUrl { get; set; }
		public string ClientId { get; set; }
	    
        public string ClientSecret
	    {
	        get;
	        set;
	    }
	    
        public string Pat
	    {
	        get; 
	        set; 
        }

	    public string TfsLocation { get; set; }
		public ConnectionDetails()
		{

		}

		public ConnectionDetails(string octaneUrl, string clientId, string clientSecret, string tfsLocation, string instanceId)
		{
			ALMOctaneUrl = octaneUrl;
			ClientId = clientId;
			ClientSecret = clientSecret;
			InstanceId = (instanceId == null ? Guid.NewGuid().ToString() : instanceId);
			TfsLocation = tfsLocation;
		}

		public string InstanceId { get; set; } = Guid.NewGuid().ToString();

		public ConnectionDetails RemoveSensitiveInfo()
		{
			ClientSecret = "***";
			Pat = "***";
			return this;
		}

        

	    public object Clone()
	    {
	        var clone  =new ConnectionDetails();

	        clone.ClientSecret = ClientSecret;
	        clone.Pat = Pat;
	        clone.ALMOctaneUrl = ALMOctaneUrl;
	        clone.ClientId = ClientId;
	        clone.TfsLocation = TfsLocation;
	        clone.InstanceId = InstanceId;
	        return clone;
	    }

	    public void Encrypt()
	    {
	        Pat = Encryption.Encrypt(Pat);
	        ClientSecret = Encryption.Encrypt(ClientSecret);
        }


	    public void Decrypt()
	    {
	        Pat = Encryption.Decrypt(Pat);
	        ClientSecret = Encryption.Decrypt(ClientSecret);
        }

	    public ConnectionDetails GetSecureObject()
	    {
	        var clone = Clone() as ConnectionDetails;
            clone.Encrypt();
	        return clone;
	    }
	}
}
