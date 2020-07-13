using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace pubmed
{
    class Program
    {
        static void Main(string[] args)
        {
#if DEBUG
            var path = "data/data.txt";
            var savePath = "data/data-result.txt";
#else
            var path = "../data/data.txt";
            var savePath = "../data/data-result.txt";
#endif
            if (args.Length > 0)
            {
                path = args[0];
            }

            if (args.Length > 1)
            {
                savePath = args[1];
            }

            if (!File.Exists(path))
            {
                Console.WriteLine($"Could not find data set file at {path}");
                return;
            }

            var rawData = File.ReadAllText(path);

            var data = DataItem.Parse(rawData);
            File.WriteAllText(savePath, data.ToString());

            Console.WriteLine($"----------BEGIN QUERY---------");
            Console.WriteLine(data);
            Console.WriteLine($"----------END QUERY---------");
            Console.WriteLine($"Saved to {savePath}");
            Console.Read();
        }

        public class DataItem
        {
            public static DataItem Parse(string rawData)
            {
                if (rawData.StartsWith("{"))
                {
                    return JsonSerializer.Deserialize<DataItem>(rawData);
                }

                var result = new DataItem();
                var sets = rawData.Split("---data-set---", StringSplitOptions.RemoveEmptyEntries);
                foreach (var set in sets)
                {
                    var lines = set.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);
                    if (!lines.Any())
                    {
                        continue;
                    }

                    var setItem = new DataItem();
                    string setOperator = "OR";
                    for (int i = 0; i < lines.Length; i++)
                    {
                        if (i == 0)
                        {
                            var options = lines[i].Split(" ", StringSplitOptions.RemoveEmptyEntries)
                                .ToDictionary(s => s.Split("=")[0], s => s.Split("=")[1].ToUpperInvariant());
                            if (options.ContainsKey("set-operator"))
                            {
                                setOperator = options["set-operator"];
                            }

                            if (options.ContainsKey("prev-operator"))
                            {
                                setItem.Operator = options["prev-operator"];
                            }

                            continue;
                        }

                        var dataItem = new DataItem();
                        var line = lines[i].Trim();
                        var colvalue = line.Split("[", StringSplitOptions.RemoveEmptyEntries);
                        if (colvalue.Length > 1)
                        {
                            dataItem.Column = colvalue[1].Trim('[', ']');
                        }

                        dataItem.Operator = setOperator;

                        dataItem.Value = colvalue[0];
                        setItem.Items.Add(dataItem);
                    }

                    result.Items.Add(setItem);
                }

                return result;
            }

            public string Operator { get; set; }


            public List<DataItem> Items { get; set; } = new List<DataItem>();


            public string Value { get; set; }
            public string Column { get; set; } = "Title/Abstract";

            public override string ToString()
            {
                if ((Items?.Count ?? 0) == 0)
                {
                    return $"(\"{Value}\"[{Column}])";
                }

                var result = "";
                for (int i = 0; i < Items.Count; i++)
                {
                    var item = Items[i];
                    if (i > 0)
                    {
                        result += $" {item.Operator} ";
                    }

                    result += $"{item}";
                }

                if (result.Length > 1)
                {
                    result = $"({result})";
                }

                return result;
            }
        }
    }
}