﻿using log4net;
using MicroFocus.Adm.Octane.Api.Core.Connector.Exceptions;
using MicroFocus.Ci.Tfs.Octane;
using System;
using System.Net;
using System.Security.Authentication;

namespace MicroFocus.Adm.Octane.CiPlugins.Tfs.Core.Tools
{
	public static class ExceptionHelper
	{
		/// <summary>
		/// Check if exception related to communication issues and restart plugin to obtaine new connections
		/// <returns>True if restart was done</returns>
		public static bool HandleExceptionAndRestartIfRequired(Exception e, ILog log, string methodName)
		{

			Exception myEx = e;
			if (e is AggregateException && e.InnerException != null)
			{
				myEx = e.InnerException;
			}
			if (myEx is ServerUnavailableException || myEx is InvalidCredentialException || myEx is UnauthorizedAccessException)
			{
				log.Error($"{methodName} failed with {myEx.GetType().Name} : {myEx.Message}");
				PluginManager.GetInstance().RestartPlugin();
				return true;
			}
			else if (myEx is GeneralHttpException)
			{
				HttpStatusCode status = ((GeneralHttpException)myEx).StatusCode;
				log.Error($"{methodName} failed with {myEx.GetType().Name} : {(int)status} {status} - {myEx.Message}");
				return false;
			}
			else
			{
				log.Error($"{methodName} failed with {myEx.GetType().ToString()} : {myEx.Message}", myEx);
				return false;
			}

		}

		public static Exception GetMostInnerException(Exception e)
		{
			Exception myEx = e;
			while (myEx.InnerException != null)
			{
				myEx = myEx.InnerException;
			}
			return myEx;
		}
	}
}