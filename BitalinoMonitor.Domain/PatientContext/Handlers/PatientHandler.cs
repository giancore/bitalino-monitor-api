using BitalinoMonitor.Domain.PatientContext.Commands.PatientCommands.Inputs;
using BitalinoMonitor.Domain.PatientContext.CustomerCommands.Outputs;
using BitalinoMonitor.Domain.PatientContext.Entities;
using BitalinoMonitor.Domain.PatientContext.Repositories;
using BitalinoMonitor.Shared.Commands;
using FluentValidator;
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
using System.Linq;
using System.Xml;
using System.Xml.Serialization;

namespace BitalinoMonitor.Domain.PatientContext.Handlers
{
    public class PatientHandler : Notifiable, ICommandHandler<CreatePatientCommand>, ICommandHandler<AddExamCommand>
    {
        readonly IPatientRepository _repository;

        public PatientHandler(IPatientRepository repository)
        {
            _repository = repository;
        }

        public ICommandResult Handle(CreatePatientCommand command)
        {
            var patient = new Patient(command.Name, command.Phone, command.DateOfBirth);

            if (!string.IsNullOrEmpty(command.PhotoPath))
            {
                patient.AddPhoto(command.PhotoPath);
            }

            if (command.Id.HasValue)
            {
                _repository.Update(patient, command.Id.Value);
            }
            else
            {
                _repository.Save(patient);
            }

            return new CommandResult(true, "Paciente inserido com sucesso!", new
            {
                patient.Id,
            });
        }

        public ICommandResult Handle(AddExamCommand command)
        {
            var patient = _repository.Get(command.IdPatient);

            if (patient == null)
            {
                return new CommandResult(false, "Paciente não encontrado", Notifications);
            }

            var frames = command.Frames.Select(f => new BitalinoFrame(f.Identifier, f.Seq, f.Analog, f.Digital));

            var duration = TimeSpan.FromMilliseconds(command.Duration);
            var exam = new Exam(command.Channel, command.Frequency, duration, command.Date, frames);

            _repository.Save(exam, patient.Id);

            return new CommandResult(true, "Exame inserido com sucesso!", new
            {
                exam.Id,
            });
        }

        public ICommandResult Handle(DeletePatientCommand command)
        {
            var patient = _repository.Get(command.IdPatient);

            if (patient == null)
            {
                return new CommandResult(false, "Paciente não encontrado", Notifications);
            }

            _repository.Delete(patient.Id);

            return new CommandResult(true, "Paciente excluído com sucesso!", true);
        }

        public ICommandResult Handle(CreateEhrCommand command)
        {
            //var exam = _repository.GetExam(command.IdExam);

            //if (exam == null)
            //{
            //    return new CommandResult(false, "Exame não encontrado", Notifications);
            //}

            var composition = CreateComposition();

            var settings = new XmlWriterSettings
            {
                OmitXmlDeclaration = true,
                Indent = true,
                NewLineOnAttributes = true
            };

            using (var writer = XmlWriter.Create(@"c:\BitalinoMonitorReport.xml", settings))
            {
                var serializer = new XmlSerializer(typeof(Composition), new XmlAttributeOverrides(), new Type[] { },
                                    new XmlRootAttribute("composition"), RmXmlSerializer.OpenEhrNamespace);

                serializer.Serialize(writer, composition);
            }

            return new CommandResult(true, "Exame processado com sucesso!", true);
        }

        Cluster CreateClusterPersonName()
        {
            var name = new DvText("Bitalino Monitor Report");
            string archetypeNodeId = "openEHR-EHR-CLUSTER.person_name.v1";

            var at0006 = new Element(new DvText("Name type"), "at0006", null, null, null, null, new DvText("Registered name"), null);
            var at0020 = new Element(new DvText("Registered name"), "at0020", null, null, null, null, new DvText("Gian"), null);

            var cluster = new Cluster(name, "at0088", null, null, null, null, new Item[2] { at0006, at0020 });

            return cluster;
        }

        Cluster CreateClusterMedicalDevice()
        {
            string archetypeNodeId = "openEHR-EHR-CLUSTER.device.v1";

            var at0001 = new Element(new DvText("Name"), "at0001", null, null, null, null, new DvText("Bitalino"), null);
            var at0002 = new Element(new DvText("Description"), "at0002", null, null, null, null, new DvText("O BITalino é um kit de ferramentas de hardware e software especificamente projetado para lidar com os requisitos dos sinais corporais."), null);

            var cluster = new Cluster(new DvText("Device details"), "at0076", null, null, GetArchetypeDetails(), null, new Item[2] { at0001, at0002 });

            return cluster;
        }

        Composition CreateComposition()
        {
            string archetypeNodeId = "openEHR-EHR-COMPOSITION.report-result.v1";
            var language = new CodePhrase("en", "ISO_639-1");
            var context = new EventContext(new DvDateTime(), null, new DvCodedText("232", "secondary medical care", "openehr"), null, null, null, null);

            var content = CreateObservationEcg();

            var composition = new Composition(new DvText("Result Report"), archetypeNodeId, null, null, GetArchetypeDetails(), null, language, null, new DvCodedText(), context, new ContentItem[1] { content }, new PartyIdentified("Composer"));

            return composition;
        }

        Observation CreateObservationEcg()
        {
            string archetypeNodeId = "openEHR-EHR-OBSERVATION.ecg_result.v0";

            var name = new DvText("ECG result");

            var archetypeDetails = GetArchetypeDetails();
            var language = new CodePhrase("en", "ISO_639-1");
            var category = new DvCodedText("event", "433", "openehr");
            var composer = new PartyIdentified("EhrGateUnit");
            var encoding = new CodePhrase("UTF-8", "IANA_character-sets");

            var medicalDevice = CreateClusterMedicalDevice();
            var personName = CreateClusterPersonName();
            var at0102 = new Element(new DvText("Technical quality"), "at0102", null, null, null, null, new DvText(), null);
            var at0097 = new Element(new DvText("ECG lead placement"), "at0097", null, null, null, null, new DvText(), null);
            var at0025 = new Element(new DvText("QTc algorithm"), "at0025", null, null, null, null, new DvText(), null);
            var at0095 = new Element(new DvText("Device interpretation comment"), "at0095", null, null, null, null, new DvText(), null);

            var protocol = new ItemTree(new DvText("Tree"), "at0003", null, null, GetArchetypeDetails(), null, new Item[6] { medicalDevice, personName, at0102, at0097, at0025, at0095 });

            var data = GetHistoryData();

            return new Observation(name, archetypeNodeId, null, null, archetypeDetails, null, language, null, null, null, null, null, protocol, null, data, null);
        }

        History<ItemStructure> GetHistoryData()
        {
            var treeItensData = new List<Item>();

            var elementECGType = new Element(new DvText("ECG type"), "at0100", null, null, GetArchetypeDetails(), null, new DvText("Não sei"), null);
            treeItensData.Add(elementECGType);

            var elementPPRate = new Element(new DvText("PP rate"), "at0094", null, null, GetArchetypeDetails(), null, new DvQuantity(00.0, "1/min", 0), null);
            treeItensData.Add(elementPPRate);

            var elementRRRate = new Element(new DvText("RR rate"), "at0013", null, null, GetArchetypeDetails(), null, new DvQuantity(00.0, "1/min", 0), null);
            treeItensData.Add(elementRRRate);

            var elementGlobalPRInterval = new Element(new DvText("Global PR interval"), "at0012", null, null, GetArchetypeDetails(), null, new DvQuantity(00.0, "ms", 0), null);
            treeItensData.Add(elementGlobalPRInterval);

            var elementGlobalQRSDuration = new Element(new DvText("Global QRS duration"), "at0014", null, null, GetArchetypeDetails(), null, new DvQuantity(00.0, "ms", 0), null);
            treeItensData.Add(elementGlobalQRSDuration);

            var elementGlobalQTInterval = new Element(new DvText("Global QT interval"), "at0007", null, null, GetArchetypeDetails(), null, new DvQuantity(00.0, "ms", 0), null);
            treeItensData.Add(elementGlobalQTInterval);

            var elementGlobalQTcInterval = new Element(new DvText("Global QTc interval"), "at0008", null, null, GetArchetypeDetails(), null, new DvQuantity(00.0, "ms", 0), null);
            treeItensData.Add(elementGlobalQTcInterval);

            var elementPAxis = new Element(new DvText("P axis"), "at0020", null, null, GetArchetypeDetails(), null, new DvText(), null);
            treeItensData.Add(elementPAxis);

            var elementQRSAxis = new Element(new DvText("QRS axis"), "at0021", null, null, GetArchetypeDetails(), null, new DvText(), null);
            treeItensData.Add(elementQRSAxis);

            var elementTAxis = new Element(new DvText("T axis"), "at0022", null, null, GetArchetypeDetails(), null, new DvText(), null);
            treeItensData.Add(elementTAxis);

            var elementDescription = new Element(new DvText("Description"), "at0098", null, null, GetArchetypeDetails(), null, new DvText(), null);
            treeItensData.Add(elementDescription);

            var elementClinicalInformationProvided = new Element(new DvText("Clinical information provided"), "at0096", null, null, GetArchetypeDetails(), null, new DvText(), null);
            treeItensData.Add(elementClinicalInformationProvided);

            var elementDeviceInterpretation = new Element(new DvText("Device interpretation"), "at0009", null, null, GetArchetypeDetails(), null, new DvText(), null);
            treeItensData.Add(elementDeviceInterpretation);

            var elementFinding = new Element(new DvText("Finding"), "at0101", null, null, GetArchetypeDetails(), null, new DvText(), null);
            treeItensData.Add(elementFinding);

            var elementECGDiagnosis = new Element(new DvText("ECG diagnosis"), "at0081", null, null, GetArchetypeDetails(), null, new DvText(), null);
            treeItensData.Add(elementECGDiagnosis);

            var elementConclusion = new Element(new DvText("Conclusion"), "at0089", null, null, GetArchetypeDetails(), null, new DvText(), null);
            treeItensData.Add(elementConclusion);

            var elementComment = new Element(new DvText("Comment"), "at0090", null, null, GetArchetypeDetails(), null, new DvText(), null);
            treeItensData.Add(elementComment);

            var itemTreeData = new ItemTree(new DvText("Tree"), "at0005", null, null, GetArchetypeDetails(), null, treeItensData.ToArray());

            var pontEvent = new PointEvent<ItemStructure>(new DvText("Any event"), "at0002", null, null, GetArchetypeDetails(), null, new DvDateTime(new DateTime()), itemTreeData, null);

            var history = new History<ItemStructure>(new DvText("Event Series"), "at0001", null, null, GetArchetypeDetails(), null, new DvDateTime(), null, null, new Event<ItemStructure>[1] { pontEvent }, null);

            return history;
        }

        Archetyped GetArchetypeDetails()
        {
            ArchetypeId aId = new ArchetypeId("openEHR-EHR-COMPOSITION.report-result.v1");
            TemplateId tId = new TemplateId("BitalinoMonitor");
            string rmVersion = "1.0.1";

            Archetyped details = new Archetyped(aId, rmVersion, tId);
            return details;
        }
    }
}
