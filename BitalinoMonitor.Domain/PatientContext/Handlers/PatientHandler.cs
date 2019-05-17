using BitalinoMonitor.Domain.PatientContext.Commands.PatientCommands.Inputs;
using BitalinoMonitor.Domain.PatientContext.CustomerCommands.Outputs;
using BitalinoMonitor.Domain.PatientContext.Entities;
using BitalinoMonitor.Domain.PatientContext.Repositories;
using BitalinoMonitor.Domain.PatientContext.Services;
using BitalinoMonitor.Shared.Commands;
using FluentValidator;
using System;
using System.Linq;

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
            var patient = _repository.GetPatient(command.IdPatient);

            if (patient == null)
            {
                return new CommandResult(false, "Paciente não encontrado", Notifications);
            }

            _repository.Delete(patient.Id);

            return new CommandResult(true, "Paciente excluído com sucesso!", true);
        }

        public ICommandResult Handle(CreateEhrCompositionCommand command)
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

            string xml =_openEhrService.CreateCompositionAsXml(patient, exam);

            return new CommandResult(true, "Composição gerada com sucesso", xml);
        }
    }
}
