﻿using CsvHelper;
using Nsar.Common.UnitOfMeasure;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Nsar.Nodes.NwccScan.ReportFormat
{
    // TODO: Class should inherit interface!
    public class Formatter
    {
        // https://joshclose.github.io/CsvHelper/
        private ICsvParser parser;

        public Formatter()
        {
            //this.parser = parser;
        }

        public List<ITemporalMeasurement> ParseData(string data)
        {
            List<ITemporalMeasurement> measurements = new List<ITemporalMeasurement>();

            // TODO: Figure out how to use dependency injection with this
            using (TextReader reader = new StringReader(data))
            {
                // TODO: Read metadata again
                //string fieldInfo = reader.ReadLine();
                //fieldInfo += reader.ReadLine();
                //string temperalInfo = reader.ReadLine();

                bool isComment = false;
                do
                {
                    int character = reader.Peek();

                    if (character > 0 && Convert.ToChar(character) == '#')
                    {
                        isComment = true;
                        reader.ReadLine();
                    }
                    else { isComment = false; }

                } while (isComment == true);

                // TODO: Figure out how to use dependency injection with this
                parser = new CsvParser(reader);

                // Determine Phenomenon from headers and used to set units
                List<IPhenomenon> headers = parseHeaders(parser.Read());

                while (true)
                {
                    var row = parser.Read();
                    if (row == null)
                    {
                        break;
                    }

                    for (int i = 1; i < row.Count(); i++)
                    {
                        // Regex from: http://stackoverflow.com/a/378447/1621156
                        string units = Regex.Match(
                            headers[i].Metadata,
                            @"\(([^)]*)\)").Groups[1].Value;

                        //if(row[i].Contains('-'))
                        //    row[i].Replace('-', '-');

                        // Sets empty strings to 0
                        double value;
                        if (!double.TryParse(row[i], out value))
                            value = 0.0;

                        // Each column for a row is a measurment with a given datetime
                        measurements.Add(
                            new TemporalMeasurement(
                                //Double.Parse(row[i], System.Globalization.NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture), 
                                value,
                                units,
                                headers[i],
                                DateTime.ParseExact(
                                    row[0],
                                    "yyyy-MM-dd HH:mm",
                                    CultureInfo.InvariantCulture)));

                    }
                }
            }

            var result = convertUnits(measurements);

            return result;
        }

        private List<IPhenomenon> parseHeaders(string[] headerStrings)
        {
            List<IPhenomenon> headers = new List<IPhenomenon>();

            foreach (string header in headerStrings)
            {
                //TODO: Incomplete -- also assumes metric
                switch (header)
                {
                    case "Air Temperature Average (degC)":
                        headers.Add(new AirTemperature(header));
                        break;
                    case "Wind Speed Average (km/hr)":
                        headers.Add(new WindSpeed(header));
                        break;
                    case "Wind Direction Average (degree)":
                        headers.Add(new WindDirection(header));
                        break;
                    case "Relative Humidity (pct)":
                        headers.Add(new RelativeHumididty(header));
                        break;
                    case "Precipitation Increment (mm)":
                        headers.Add(new Precipitation(header));
                        break;
                    default:
                        headers.Add(new Phenomenon("Unknown", "", header));
                        break;
                }
            }

            return headers;
        }

        private List<ITemporalMeasurement> convertUnits(List<ITemporalMeasurement> measurements)
        {
            // TODO: Ojo, very hardcoded function

            // Windspeeds are in units of km/hr, need to conver to m/s
            var windSpeeds = measurements.Where(ws => ws.Unit == "km/hr").ToList();
            var result = measurements.Where(m => m.Unit != "km/hr").ToList();

            foreach (var ws in windSpeeds)
            {
                string newUnit = "m/s";
                double newValue = ws.NumericalValue * (1D / 60D) * (1D / 60D) * (1000D / 1D);
                DateTime dt = ws.DateTime;

                result.Add(new TemporalMeasurement(
                    newValue, newUnit, ws.Phenomenon, ws.DateTime));
            }

            return result;
        }
    }
}
