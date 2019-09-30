using Dynamics365.Api.Client.Models;
using Dynamics365.Api.Client.Types;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace Dynamics365.Api.Client.Services
{
    public static class OptionSetMapper
    {
        public static OptionSetMap OptionMapping = new OptionSetMap();

        public static void AddOptionSetMapping(string json)
        {
            OptionMapping.OptionSetDict = new ConcurrentDictionary<string, List<OptionSet>>();

            var jsonObject = JObject.Parse(json);
            foreach (JToken optionSet in jsonObject["value"])
            {
                if (optionSet["Options"] == null)
                {
                    continue;
                }

                if (optionSet["IsCustomOptionSet"] == null || !optionSet["IsCustomOptionSet"].ToObject<bool>())
                {
                    continue;
                }

                var optionSetName = optionSet["Name"].ToString();
                var displayName = optionSet["DisplayName"]["LocalizedLabels"].First["Label"].ToString();

                foreach (var option in optionSet["Options"])
                {
                    var optionSetValue = option["Value"].ToObject<int>();
                    var label = option["Label"]["LocalizedLabels"].First["Label"].ToString();
                    var optionSetMapping = new OptionSet
                    {
                        Name = displayName,
                        Mapping = new Mapping
                        {
                            Crm = optionSetValue,
                            Value = label
                        }
                    };

                    if (OptionMapping.OptionSetDict.ContainsKey(optionSetName))
                    {
                        OptionMapping.OptionSetDict[optionSetName].Add(optionSetMapping);
                    }
                    else
                    {
                        OptionMapping.OptionSetDict[optionSetName] = new List<OptionSet> { optionSetMapping };
                    }
                }
            }
        }

        public static int ToOptionSet(object value, string optionSetName)
        {
            var optionSet = OptionMapping.OptionSetDict.ContainsKey(optionSetName) ?
               OptionMapping.OptionSetDict[optionSetName]
               : throw new ArgumentException($"Mapping: {optionSetName} not found!");

            var option = optionSet.FirstOrDefault(opt => CompareWithMapping(value, opt.Mapping.Value))
                ?? throw new ArgumentException($"{value} with type {value.GetType()} was not found in option set: {optionSetName}");

            return option.Mapping.Crm;
        }

        public static object FromOptionSet(int optionSetValue, string optionSetName)
        {
            var optionSet = OptionMapping.OptionSetDict.ContainsKey(optionSetName) ?
               OptionMapping.OptionSetDict[optionSetName]
               : throw new ArgumentException($"Mapping: {optionSetName} not found!");

            var option = optionSet.FirstOrDefault(opt => CompareWithMapping(optionSetValue, opt.Mapping.Crm))
                ?? throw new ArgumentException($"{optionSetValue} was not found in option set: {optionSetName}");

            return option.Mapping.Value;
        }

        private static bool CompareWithMapping(object propValue, object mappedValue)
        {
            switch (mappedValue)
            {
                case long l:
                    return Convert.ToInt64(propValue) == l;
                case string str:
                    return propValue.ToString().Equals(str, StringComparison.InvariantCultureIgnoreCase);
                default:
                    return propValue.Equals(mappedValue) || mappedValue.Equals(propValue);
            }
        }
    }


}
