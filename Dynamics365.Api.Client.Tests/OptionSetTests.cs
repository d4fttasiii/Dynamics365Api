using Dynamics365.Api.Client.Services;
using Dynamics365.Api.Client.Tests.Entities;
using FluentAssertions;
using NUnit.Framework;

namespace Dynamics365.Api.Client.Tests
{
    [TestFixture]
    public class OptionSetTests
    {
        [Test]
        public void OptionSetMapper_Can_Parse_Response_JSON()
        {
            OptionSetMapper.AddOptionSetMapping(_jsonResponseForTest);

            OptionSetMapper.OptionMapping.OptionSetDict.ContainsKey("test_sync_states").Should().BeTrue();
            OptionSetMapper.OptionMapping.OptionSetDict["test_sync_states"].Count.Should().Be(3);
            OptionSetMapper.ToOptionSet("Synchronized", "test_sync_states").Should().Be(1);
        }

        [Test]
        public void OptionSetMapper_Should_Be_Able_To_Map_OptionSetValues()
        {
            OptionSetMapper.AddOptionSetMapping(_jsonResponseForTest);

            var account = new AccountEntity
            {
                SyncState = "Out of Sync"
            };
            account.CommitChanges();
            account.Attributes["test_sync_state"].Should().Be(2);
        }

        [Test]
        public void OptionSetMapper_Should_Be_Able_To_Map_OptionSetValues_Enums()
        {
            OptionSetMapper.AddOptionSetMapping(_jsonResponseForTest);

            var lead = new LeadEntity
            {
                SyncState = SyncStates.Error
            };
            lead.CommitChanges();
            lead.Attributes["test_sync_state"].Should().Be(3);
        }

        private readonly string _jsonResponseForTest = @"{
  ""@odata.context"": ""https://test.crm4.dynamics.com/api/data/v9.1/$metadata#GlobalOptionSetDefinitions"",
  ""value"": [
      {
          ""@odata.type"": ""#Microsoft.Dynamics.CRM.OptionSetMetadata"",
          ""ParentOptionSetName"": null,
          ""IsCustomOptionSet"": true,
          ""IsGlobal"": true,
          ""IsManaged"": false,
          ""Name"": ""test_sync_states"",
          ""ExternalTypeName"": """",
          ""OptionSetType"": ""Picklist"",
          ""IntroducedVersion"": ""1.0.0.0"",
          ""MetadataId"": ""5f1ec791-c2ec-e811-a95e-000d3a39a711"",
          ""HasChanged"": null,
          ""Options"": [
              {
                  ""Value"": 1,
                  ""Color"": ""#0000ff"",
                  ""IsManaged"": false,
                  ""ExternalValue"": """",
                  ""ParentValues"": [],
                  ""MetadataId"": null,
                  ""HasChanged"": null,
                  ""Label"": {
                      ""LocalizedLabels"": [
                          {
                              ""Label"": ""Synchronized"",
                              ""LanguageCode"": 1033,
                              ""IsManaged"": false,
                              ""MetadataId"": ""621ec791-c2ec-e811-a95e-000d3a39a711"",
                              ""HasChanged"": null
                          }
                      ],
                      ""UserLocalizedLabel"": {
                          ""Label"": ""Synchronized"",
                          ""LanguageCode"": 1033,
                          ""IsManaged"": false,
                          ""MetadataId"": ""621ec791-c2ec-e811-a95e-000d3a39a711"",
                          ""HasChanged"": null
                      }
                  },
                  ""Description"": {
                      ""LocalizedLabels"": [
                          {
                              ""Label"": """",
                              ""LanguageCode"": 1033,
                              ""IsManaged"": false,
                              ""MetadataId"": ""641ec791-c2ec-e811-a95e-000d3a39a711"",
                              ""HasChanged"": null
                          }
                      ],
                      ""UserLocalizedLabel"": {
                          ""Label"": """",
                          ""LanguageCode"": 1033,
                          ""IsManaged"": false,
                          ""MetadataId"": ""641ec791-c2ec-e811-a95e-000d3a39a711"",
                          ""HasChanged"": null
                      }
                  }
              },
              {
                  ""Value"": 2,
                  ""Color"": ""#0000ff"",
                  ""IsManaged"": false,
                  ""ExternalValue"": """",
                  ""ParentValues"": [],
                  ""MetadataId"": null,
                  ""HasChanged"": null,
                  ""Label"": {
                      ""LocalizedLabels"": [
                          {
                              ""Label"": ""Out of Sync"",
                              ""LanguageCode"": 1033,
                              ""IsManaged"": false,
                              ""MetadataId"": ""651ec791-c2ec-e811-a95e-000d3a39a711"",
                              ""HasChanged"": null
                          }
                      ],
                      ""UserLocalizedLabel"": {
                          ""Label"": ""Out of Sync"",
                          ""LanguageCode"": 1033,
                          ""IsManaged"": false,
                          ""MetadataId"": ""651ec791-c2ec-e811-a95e-000d3a39a711"",
                          ""HasChanged"": null
                      }
                  },
                  ""Description"": {
                      ""LocalizedLabels"": [
                          {
                              ""Label"": """",
                              ""LanguageCode"": 1033,
                              ""IsManaged"": false,
                              ""MetadataId"": ""671ec791-c2ec-e811-a95e-000d3a39a711"",
                              ""HasChanged"": null
                          }
                      ],
                      ""UserLocalizedLabel"": {
                          ""Label"": """",
                          ""LanguageCode"": 1033,
                          ""IsManaged"": false,
                          ""MetadataId"": ""671ec791-c2ec-e811-a95e-000d3a39a711"",
                          ""HasChanged"": null
                      }
                  }
              },
              {
                  ""Value"": 3,
                  ""Color"": ""#0000ff"",
                  ""IsManaged"": false,
                  ""ExternalValue"": """",
                  ""ParentValues"": [],
                  ""MetadataId"": null,
                  ""HasChanged"": null,
                  ""Label"": {
                      ""LocalizedLabels"": [
                          {
                              ""Label"": ""Error"",
                              ""LanguageCode"": 1033,
                              ""IsManaged"": false,
                              ""MetadataId"": ""681ec791-c2ec-e811-a95e-000d3a39a711"",
                              ""HasChanged"": null
                          }
                      ],
                      ""UserLocalizedLabel"": {
                          ""Label"": ""Error"",
                          ""LanguageCode"": 1033,
                          ""IsManaged"": false,
                          ""MetadataId"": ""681ec791-c2ec-e811-a95e-000d3a39a711"",
                          ""HasChanged"": null
                      }
                  },
                  ""Description"": {
                      ""LocalizedLabels"": [
                          {
                              ""Label"": """",
                              ""LanguageCode"": 1033,
                              ""IsManaged"": false,
                              ""MetadataId"": ""6a1ec791-c2ec-e811-a95e-000d3a39a711"",
                              ""HasChanged"": null
                          }
                      ],
                      ""UserLocalizedLabel"": {
                          ""Label"": """",
                          ""LanguageCode"": 1033,
                          ""IsManaged"": false,
                          ""MetadataId"": ""6a1ec791-c2ec-e811-a95e-000d3a39a711"",
                          ""HasChanged"": null
                      }
                  }
              }
          ],
          ""Description"": {
              ""LocalizedLabels"": [
                  {
                      ""Label"": """",
                      ""LanguageCode"": 1033,
                      ""IsManaged"": false,
                      ""MetadataId"": ""6c1ec791-c2ec-e811-a95e-000d3a39a711"",
                      ""HasChanged"": null
                  }
              ],
              ""UserLocalizedLabel"": {
                  ""Label"": """",
                  ""LanguageCode"": 1033,
                  ""IsManaged"": false,
                  ""MetadataId"": ""6c1ec791-c2ec-e811-a95e-000d3a39a711"",
                  ""HasChanged"": null
              }
          },
          ""DisplayName"": {
              ""LocalizedLabels"": [
                  {
                      ""Label"": ""System: Sync States"",
                      ""LanguageCode"": 1033,
                      ""IsManaged"": false,
                      ""MetadataId"": ""6b1ec791-c2ec-e811-a95e-000d3a39a711"",
                      ""HasChanged"": null
                  }
              ],
              ""UserLocalizedLabel"": {
                  ""Label"": ""System: Sync States"",
                  ""LanguageCode"": 1033,
                  ""IsManaged"": false,
                  ""MetadataId"": ""6b1ec791-c2ec-e811-a95e-000d3a39a711"",
                  ""HasChanged"": null
              }
          },
          ""IsCustomizable"": {
              ""Value"": true,
              ""CanBeChanged"": true,
              ""ManagedPropertyLogicalName"": ""iscustomizable""
          }
      }
  ]
}";
    }
}
