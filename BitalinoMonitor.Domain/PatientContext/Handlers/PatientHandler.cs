using BitalinoMonitor.Domain.PatientContext.Commands.PatientCommands.Inputs;
using BitalinoMonitor.Domain.PatientContext.CustomerCommands.Outputs;
using BitalinoMonitor.Domain.PatientContext.Entities;
using BitalinoMonitor.Domain.PatientContext.Enums;
using BitalinoMonitor.Domain.PatientContext.Repositories;
using BitalinoMonitor.Shared.Commands;
using FluentValidator;
using System.Linq;

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

            _repository.Save(patient);

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

            var exam = new Exam(command.Channel, command.Date, (EExamType)command.Channel, frames);

            _repository.SaveExam(exam, patient.Id);

            return new CommandResult(true, "Exame inserido com sucesso!", new
            {
                exam.Id,
            });
        }
    }
}
