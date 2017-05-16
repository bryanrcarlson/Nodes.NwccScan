using Nsar.Common.UnitOfMeasure;
using Nsar.Nodes.NwccScan.ReportFormat;
using System;
using System.Collections.Generic;
using System.IO;
using Xunit;

namespace ReportFormat.Tests
{
    public class FormatterTests
    {
        [Fact]
        public void ParseData_WithComments_IgnoreCommentedLines()
        {
            // ARRANGE
            Formatter sut = new Formatter();
            string path = Path.Combine(AppContext.BaseDirectory, @"Assets\ResultsWithComments.txt");
            string data = File.ReadAllText(path);

            string pathToExpected = Path.Combine(AppContext.BaseDirectory, @"Assets\ResultsNoComments.txt");
            string dataExpected = File.ReadAllText(pathToExpected);
            List<ITemporalMeasurement> expected = sut.ParseData(dataExpected);

            // ACT
            List<ITemporalMeasurement> results = sut.ParseData(data);

            // ASSERT
            Assert.Equal(expected[0].NumericalValue, results[0].NumericalValue);
            Assert.Equal(expected[4].Phenomenon.Metadata, results[4].Phenomenon.Metadata);
            Assert.Equal(expected[6].Unit, results[6].Unit);
        }
    }
}
