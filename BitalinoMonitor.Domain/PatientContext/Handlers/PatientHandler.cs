using BitalinoMonitor.Domain.PatientContext.Commands.PatientCommands.Inputs;
using BitalinoMonitor.Domain.PatientContext.CustomerCommands.Outputs;
using BitalinoMonitor.Domain.PatientContext.Entities;
using BitalinoMonitor.Domain.PatientContext.Repositories;
using BitalinoMonitor.Domain.PatientContext.Services;
using BitalinoMonitor.Domain.PatientContext.Services.Filter;
using BitalinoMonitor.Shared.Commands;
using FluentValidator;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml;
using System.Xml.Xsl;

namespace BitalinoMonitor.Domain.PatientContext.Handlers
{
    public class PatientHandler : Notifiable, ICommandHandler<CreatePatientCommand>, ICommandHandler<AddExamCommand>
    {
        readonly IPatientRepository _repository;
        readonly IOpenEHRService _openEhrService;

        public PatientHandler(IPatientRepository repository, IOpenEHRService openEhrService)
        {
            _repository = repository;
            _openEhrService = openEhrService;
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
            var patient = _repository.GetPatient(command.IdPatient);

            if (patient == null)
            {
                return new CommandResult(false, "Paciente não encontrado", Notifications);
            }

            var frames = command.Frames.Select(f => new BitalinoFrame(f.Identifier, f.Seq, f.Analog, f.Digital));

            var exam = new Exam(command.Channel, command.Frequency, command.Duration, command.Date, frames);

            _repository.Save(exam, patient.Id);

            return new CommandResult(true, "Exame inserido com sucesso!", new
            {
                exam.Id,
            });
        }

        public ICommandResult Handle(DeletePatientCommand command)
        {
            var patient = _repository.GetPatient(command.IdPatient);

            if (patient == null)
            {
                return new CommandResult(false, "Paciente não encontrado", Notifications);
            }

            _repository.Delete(patient.Id);

            return new CommandResult(true, "Paciente excluído com sucesso!", true);
        }

        public ICommandResult Handle(GetExamCommand command)
        {
            var exam = _repository.GetExam(command.IdExam);

            if (exam == null)
            {
                return new CommandResult(false, "Exame não encontrado", Notifications);
            }

            var frames = exam.Frames;
            exam.ClearFrames();

            foreach (var frame in frames)
            {
                var framesAdc = frame.Analog[exam.Channel];
                var realValue = SensorService.GetTransferFunction(Convert.ToDouble(framesAdc), exam.Type);
                frame.Analog[exam.Channel] = realValue;

                exam.AddFrame(frame);
            }

            return new CommandResult(true, "Exame obtido com sucesso", exam);
        }

        public ICommandResult Handle(CreateMedicalRecordsXMLCommand command)
        {
            var exam = _repository.GetExam(command.IdExam);

            if (exam == null)
            {
                return new CommandResult(false, "Exame não encontrado", Notifications);
            }

            if (exam.Type != Enums.EExamType.Electrocardiography)
            {
                return new CommandResult(false, "O exame deve ser do tipo Eletrocardiograma (ECG)", Notifications);
            }

            var patient = _repository.GetPatientByIdExam(command.IdExam);

            string xml = _openEhrService.CreateCompositionAsXml(patient, exam);

            return new CommandResult(true, "Prontuário gerado com sucesso", xml);
        }

        public ICommandResult Handle(TransformMedicalRecordsToHTMLCommand command)
        {
            var exam = _repository.GetExam(command.IdExam);

            if (exam == null)
            {
                return new CommandResult(false, "Exame não encontrado", Notifications);
            }

            if (exam.Type != Enums.EExamType.Electrocardiography)
            {
                return new CommandResult(false, "O exame deve ser do tipo Eletrocardiograma (ECG)", Notifications);
            }

            var patient = _repository.GetPatientByIdExam(command.IdExam);

            string xml = _openEhrService.CreateCompositionAsXml(patient, exam);
            string buildDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string xslFile = $"{buildDir}\\PatientContext\\XSLTransform\\openEHR_RMtoHTML.xsl";

            var results = new StringWriter();
            var settings = new XmlReaderSettings
            {
                DtdProcessing = DtdProcessing.Parse
            };

            using (var reader = XmlReader.Create(xslFile, settings))
            {
                var transform = new XslCompiledTransform();
                transform.Load(reader);

                using (var readerTransform = XmlReader.Create(new StringReader(xml)))
                {
                    transform.Transform(readerTransform, null, results);
                }
            }

            string result = results.ToString();

            return new CommandResult(true, "Transfomação gerada com sucesso", new { result });
        }

        public ICommandResult Handle(FilterExamResultCommand command)
        {
            var exam = _repository.GetExam(command.IdExam);

            if (exam == null)
            {
                return new CommandResult(false, "Exame não encontrado", Notifications);
            }

            var frames = exam.Frames.Select(f => f.Analog[exam.Channel]);
            var filteredFrames = new List<double>();

            var butterworthFilter = new ButterworthFilterService(exam.Frequency);

            foreach (var frame in frames)
            {
                var realValue = SensorService.GetTransferFunction(frame, exam.Type);
                double filteredFrame = butterworthFilter.Filter(realValue);
                filteredFrames.Add(filteredFrame);
            }

            return new CommandResult(true, "Filtragem realizada com sucesso", filteredFrames);
        }
    }
}
