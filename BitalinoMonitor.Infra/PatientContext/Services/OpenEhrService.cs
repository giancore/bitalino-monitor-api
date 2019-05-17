using BitalinoMonitor.Domain.PatientContext.Entities;
using BitalinoMonitor.Domain.PatientContext.Services;
using OpenEhr.RM.Common.Archetyped.Impl;
using OpenEhr.RM.Common.Generic;
using OpenEhr.RM.Composition;
using OpenEhr.RM.Composition.Content;
using OpenEhr.RM.Composition.Content.Entry;
using OpenEhr.RM.DataStructures.History;
using OpenEhr.RM.DataStructures.ItemStructure;
using OpenEhr.RM.DataStructures.ItemStructure.Representation;
using OpenEhr.RM.DataTypes.Quantity;
using OpenEhr.RM.DataTypes.Quantity.DateTime;
using OpenEhr.RM.DataTypes.Text;
using OpenEhr.RM.Support.Identification;
using OpenEhr.Serialisation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace BitalinoMonitor.Infra.PatientContext.Services
{
    public class OpenEHRService : IOpenEHRService
    {
        public string CreateCompositionAsXml(Patient patient, Exam exam)
        {
            var composition = CreateComposition(patient);

            var settings = new XmlWriterSettings
            {
                OmitXmlDeclaration = true,
                Indent = true,
                NewLineOnAttributes = true
            };

            using (var stringWriter = new StringWriter())
            {
                using (var writer = XmlWriter.Create(stringWriter, settings))
                {
                    var serializer = new XmlSerializer(typeof(Composition), new XmlAttributeOverrides(), new Type[] { },
                                        new XmlRootAttribute("composition"), RmXmlSerializer.OpenEhrNamespace);

                    serializer.Serialize(writer, composition);

                    return stringWriter.ToString();
                }
            }
        }

        Cluster CreateClusterPersonName(Patient patient)
        {
            string archetypeNodeId = "openEHR-EHR-CLUSTER.person_name.v1";

            var name = new DvText("Bitalino Monitor Report");
            var at0006 = new Element(new DvText("Name type"), "at0006", null, null, null, null, new DvText("Registered name"), null);
            var at0020 = new Element(new DvText("Registered name"), "at0020", null, null, null, null, new DvText(patient.Name), null);

            var cluster = new Cluster(name, "at0088", null, null, GetArchetypeDetails(archetypeNodeId), null, new Item[2] { at0006, at0020 });

            return cluster;
        }

        Cluster CreateClusterMedicalDevice()
        {
            string archetypeNodeId = "openEHR-EHR-CLUSTER.device.v1";

            var at0001 = new Element(new DvText("Name"), "at0001", null, null, null, null, new DvText("Bitalino"), null);
            var at0002 = new Element(new DvText("Description"), "at0002", null, null, null, null, new DvText("O BITalino é um kit de ferramentas de hardware e software especificamente projetado para lidar com os requisitos dos sinais corporais."), null);

            var cluster = new Cluster(new DvText("Device details"), "at0076", null, null, GetArchetypeDetails(archetypeNodeId), null, new Item[2] { at0001, at0002 });

            return cluster;
        }

        Composition CreateComposition(Patient patient)
        {
            string archetypeNodeId = "openEHR-EHR-COMPOSITION.report-result.v1";
            var language = new CodePhrase("en", "ISO_639-1");
            var party = new PartyIdentified("Gian Mella");
            var context = new EventContext(new DvDateTime(), null, new DvCodedText("232", "secondary medical care", "openehr"), null, null, null, null);

            var content = CreateObservationEcg(patient);
            var composition = new Composition(new DvText("Result Report"), archetypeNodeId, null, null, GetArchetypeDetails(archetypeNodeId), null, language, language, new DvCodedText("Teste"), context, new ContentItem[1] { content }, party);

            return composition;
        }

        Observation CreateObservationEcg(Patient patient)
        {
            string archetypeNodeId = "openEHR-EHR-OBSERVATION.ecg_result.v1";

            var name = new DvText("ECG result");
            var language = new CodePhrase("en", "ISO_639-1");
            var encoding = new CodePhrase("UTF-8", "IANA_character-sets");
            var party = new PartyIdentified("Gian Mella");

            var medicalDevice = CreateClusterMedicalDevice();
            var personName = CreateClusterPersonName(patient);
            var at0102 = new Element(new DvText("Technical quality"), "at0102", null, null, null, null, new DvText("Teste"), null);
            var at0097 = new Element(new DvText("ECG lead placement"), "at0097", null, null, null, null, new DvText("Teste"), null);
            var at0025 = new Element(new DvText("QTc algorithm"), "at0025", null, null, null, null, new DvText("Teste"), null);
            var at0095 = new Element(new DvText("Device interpretation comment"), "at0095", null, null, null, null, new DvText("Teste"), null);

            var protocol = new ItemTree(new DvText("Tree"), "at0003", null, null, null, null, new Item[6] { medicalDevice, personName, at0102, at0097, at0025, at0095 });

            var data = GetHistoryData();

            return new Observation(name, archetypeNodeId, null, null, GetArchetypeDetails(archetypeNodeId), null, language, encoding, party, null, null, null, protocol, null, data, null);
        }

        History<ItemStructure> GetHistoryData()
        {
            var treeItensData = new List<Item>();

            var elementECGType = new Element(new DvText("ECG type"), "at0100", null, null, null, null, new DvText("Raw ECG"), null);
            treeItensData.Add(elementECGType);

            var elementPPRate = new Element(new DvText("PP rate"), "at0094", null, null, null, null, new DvQuantity(00.0, "1/min", 0), null);
            treeItensData.Add(elementPPRate);

            var elementRRRate = new Element(new DvText("RR rate"), "at0013", null, null, null, null, new DvQuantity(00.0, "1/min", 0), null);
            treeItensData.Add(elementRRRate);

            var elementGlobalPRInterval = new Element(new DvText("Global PR interval"), "at0012", null, null, null, null, new DvQuantity(00.0, "ms", 0), null);
            treeItensData.Add(elementGlobalPRInterval);

            var elementGlobalQRSDuration = new Element(new DvText("Global QRS duration"), "at0014", null, null, null, null, new DvQuantity(00.0, "ms", 0), null);
            treeItensData.Add(elementGlobalQRSDuration);

            var elementGlobalQTInterval = new Element(new DvText("Global QT interval"), "at0007", null, null, null, null, new DvQuantity(00.0, "ms", 0), null);
            treeItensData.Add(elementGlobalQTInterval);

            var elementGlobalQTcInterval = new Element(new DvText("Global QTc interval"), "at0008", null, null, null, null, new DvQuantity(00.0, "ms", 0), null);
            treeItensData.Add(elementGlobalQTcInterval);

            var elementPAxis = new Element(new DvText("P axis"), "at0020", null, null, null, null, new DvText(), null);
            treeItensData.Add(elementPAxis);

            var elementQRSAxis = new Element(new DvText("QRS axis"), "at0021", null, null, null, null, new DvText(), null);
            treeItensData.Add(elementQRSAxis);

            var elementTAxis = new Element(new DvText("T axis"), "at0022", null, null, null, null, new DvText(), null);
            treeItensData.Add(elementTAxis);

            var elementDescription = new Element(new DvText("Description"), "at0098", null, null, null, null, new DvText(), null);
            treeItensData.Add(elementDescription);

            var elementClinicalInformationProvided = new Element(new DvText("Clinical information provided"), "at0096", null, null, null, null, new DvText(), null);
            treeItensData.Add(elementClinicalInformationProvided);

            var elementDeviceInterpretation = new Element(new DvText("Device interpretation"), "at0009", null, null, null, null, new DvText(), null);
            treeItensData.Add(elementDeviceInterpretation);

            var elementFinding = new Element(new DvText("Finding"), "at0101", null, null, null, null, new DvText(), null);
            treeItensData.Add(elementFinding);

            var elementECGDiagnosis = new Element(new DvText("ECG diagnosis"), "at0081", null, null, null, null, new DvText(), null);
            treeItensData.Add(elementECGDiagnosis);

            var elementConclusion = new Element(new DvText("Conclusion"), "at0089", null, null, null, null, new DvText(), null);
            treeItensData.Add(elementConclusion);

            var elementComment = new Element(new DvText("Comment"), "at0090", null, null, null, null, new DvText(), null);
            treeItensData.Add(elementComment);

            var itemTreeData = new ItemTree(new DvText("Tree"), "at0005", null, null, null, null, treeItensData.ToArray());

            var pontEvent = new PointEvent<ItemStructure>(new DvText("Any event"), "at0002", null, null, null, null, new DvDateTime(new DateTime()), itemTreeData, null);

            var history = new History<ItemStructure>(new DvText("Event Series"), "at0001", null, null, null, null, new DvDateTime(), null, null, new Event<ItemStructure>[1] { pontEvent }, null);

            return history;
        }

        Archetyped GetArchetypeDetails(string archetypeId)
        {
            //var aId = new ArchetypeId("openEHR-EHR-COMPOSITION.report-result.v1");
            var aId = new ArchetypeId(archetypeId);
            var tId = new TemplateId("bitalino_monitor.pt.v1");
            string rmVersion = "1.0.1";

            Archetyped details = new Archetyped(aId, rmVersion, tId);
            return details;
        }
    }
}
