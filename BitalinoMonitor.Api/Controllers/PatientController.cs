using BitalinoMonitor.Domain.PatientContext.Commands.PatientCommands.Inputs;
using BitalinoMonitor.Domain.PatientContext.CustomerCommands.Outputs;
using BitalinoMonitor.Domain.PatientContext.Entities;
using BitalinoMonitor.Domain.PatientContext.Handlers;
using BitalinoMonitor.Domain.PatientContext.Queries;
using BitalinoMonitor.Domain.PatientContext.Repositories;
using BitalinoMonitor.Shared.Commands;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;

namespace BitalinoMonitor.Api.Controllers
{
    public class PatientController : Controller
    {
        readonly IPatientRepository _repository;
        readonly PatientHandler _handler;

        public PatientController(IPatientRepository repository, PatientHandler handler)
        {
            _repository = repository;
            _handler = handler;
        }

        [HttpGet]
        [Route("v1/Patients")]
        [ResponseCache(Duration = 15)]
        public IEnumerable<ListPatientQueryResult> Get()
        {
            return _repository.GetPatients();
        }

        [HttpGet]
        [Route("v1/Patients/{id}")]
        public Patient GetById(Guid id)
        {
            return _repository.GetPatient(id);
        }

        [HttpDelete]
        [Route("v1/Patients/{id}")]
        public CommandResult DeleteById(Guid id)
        {
            var command = new DeletePatientCommand
            {
                IdPatient = id
            };

            var result = (CommandResult)_handler.Handle(command);
            return result;
        }

        [HttpGet]
        [Route("v1/Patients/{idPatient}/Exams")]
        public IEnumerable<ListPatientExamsQueryResult> GetExams(Guid idPatient)
        {
            return _repository.ListExams(idPatient);
        }

        [HttpGet]
        [Route("v1/Patients/Exam/{idExam}")]
        public CommandResult GetExam(Guid idExam)
        {
            //return _repository.GetExamAsQueryResult(idExam);
            var command = new GetExamCommand
            {
                IdExam = idExam
            };

            var result = (CommandResult)_handler.Handle(command);
            return result;
        }

        [HttpGet]
        [Route("v1/Patients/Exam/{idExam}/Frames")]
        public IEnumerable<ListPatientExamsBitalinoFrameQueryResult> GetFrames(Guid idExam)
        {
            return _repository.ListFrames(idExam);
        }

        [HttpPost]
        [Route("v1/Patients")]
        public ICommandResult Post([FromBody]CreatePatientCommand command)
        {
            var result = (CommandResult)_handler.Handle(command);
            return result;
        }

        [HttpPost]
        [Route("v1/Patients/Exam")]
        public ICommandResult AddExam([FromBody]AddExamCommand command)
        {
            var result = (CommandResult)_handler.Handle(command);
            return result;
        }

        [HttpGet]
        [Route("v1/Patients/Exam/{idExam}/Create-Medical-Records")]
        public ICommandResult GetMedicalRecords(Guid idExam)
        {
            var command = new CreateMedicalRecordsXMLCommand
            {
                IdExam = idExam
            };

            var result = (CommandResult)_handler.Handle(command);
            return result;
        }

        [HttpGet]
        [Route("v1/Patients/Exam/{idExam}/Transform-Medical-Records")]
        public ICommandResult TransformMedicalRecordsToHTML(Guid idExam)
        {
            var command = new TransformMedicalRecordsToHTMLCommand
            {
                IdExam = idExam
            };

            var result = (CommandResult)_handler.Handle(command);
            return result;
        }

        [HttpGet]
        [Route("v1/Patients/Exam/{idExam}/Filter")]
        public ICommandResult GetFilterExamResult(Guid idExam)
        {
            var command = new FilterExamResultCommand
            {
                IdExam = idExam
            };

            var result = (CommandResult)_handler.Handle(command);
            return result;
        }
    }
}