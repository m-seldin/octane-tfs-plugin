﻿using MicroFocus.Ci.Tfs.Octane.Dto.TestResults;
using MicroFocus.Ci.Tfs.Octane.Tfs.ApiItems;
using MicroFocus.Ci.Tfs.Octane.Tfs.Beans;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml.Serialization;

namespace MicroFocus.Ci.Tfs.Octane
{
	public static class TestResultUtils
	{
		public static String SerializeToXml(OctaneTestResult octaneTestResult)
		{
			XmlSerializer xmlSerializer = new XmlSerializer(typeof(OctaneTestResult));
			String output;
			using (StringWriter textWriter = new Utf8StringWriter())
			{
				XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
				ns.Add("", "");
				xmlSerializer.Serialize(textWriter, octaneTestResult, ns);
				return output = textWriter.ToString();
			}
		}

		public static OctaneTestResult ConvertToOctaneTestResult(string serverId, string projectCiId, string buildCiId, TfsTestResults testResults)
		{
			if (testResults.Count > 0)
			{
				//Serialization prepare
				OctaneTestResult octaneTestResult = new OctaneTestResult();
				TfsItem build = testResults.Results[0].Build;
				TfsItem project = testResults.Results[0].Project;
				octaneTestResult.Build = OctaneTestResultBuild.Create(serverId, buildCiId, projectCiId);
				octaneTestResult.TestFields = new List<OctaneTestResultTestField>(new[] {
					OctaneTestResultTestField.Create(OctaneTestResultTestField.TEST_LEVEL_TYPE, "UnitTest")
				});

				octaneTestResult.TestRuns = new List<OctaneTestResultTestRun>();
				foreach (TfsTestResult testResult in testResults.Results)
				{
					OctaneTestResultTestRun run = new OctaneTestResultTestRun();
					String[] testNameParts = testResult.AutomatedTestName.Split('.');
					run.Name = testNameParts[testNameParts.Length - 1];
					run.Class = testNameParts[testNameParts.Length - 2];
					run.Package = String.Join(".", new ArraySegment<String>(testNameParts, 0, testNameParts.Length - 2));

					run.Module = Path.GetFileNameWithoutExtension(testResult.AutomatedTestStorage);

					run.Duration = (long)testResult.DurationInMs;
					run.Status = testResult.Outcome;
					if (run.Status.Equals("Failed"))
					{
						run.Error = OctaneTestResultError.Create(testResult.FailureType, testResult.ErrorMessage, testResult.StackTrace);
					}

					run.Started = ConvertToOctaneTime(testResult.StartedDate);

					//Run WebAccessUrl        http://berkovir:8080/tfs/DefaultCollection/Test2/_TestManagement/Runs#runId=8&_a=runCharts
					//Run Result WebAccessUrl http://berkovir:8080/tfs/DefaultCollection/Test2/_TestManagement/Runs#runId=8&_a=resultSummary&resultId=100000
					run.ExternalReportUrl = testResults.Run.WebAccessUrl.Replace("_a=runCharts", ($"_a=resultSummary&resultId={testResult.Id}"));

					octaneTestResult.TestRuns.Add(run);
				}
				return octaneTestResult;
			}
			return null;
		}

		public static long ConvertToOctaneTime(DateTime data)
		{
			TimeSpan span = data.Subtract(new DateTime(1970, 1, 1, 0, 0, 0, data.Kind));
			return (long)span.TotalMilliseconds;
		}
	}

	public class Utf8StringWriter : StringWriter
	{
		public override Encoding Encoding => Encoding.UTF8;
	}
}