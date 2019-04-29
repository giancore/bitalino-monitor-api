using BitalinoMonitor.Domain.PatientContext.Entities;
using BitalinoMonitor.Domain.PatientContext.Queries;
using BitalinoMonitor.Domain.PatientContext.Repositories;
using BitalinoMonitor.Infra.DataContexts;
using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BitalinoMonitor.Infra.PatientContext.Repositories
{
    public class PatientRepository : IPatientRepository
    {
        readonly BitalinoMonitorDataContext _context;

        public PatientRepository(BitalinoMonitorDataContext context)
        {
            _context = context;
        }

        public GetPatientQueryResult Get(Guid id)
        {
            return _context.Connection
                .Query<GetPatientQueryResult>("SELECT [Id], [Name], [PhotoPath], [Phone], [DateOfBirth], [CreateDate] FROM [Patient] WHERE [Id] = @id", new { id }).FirstOrDefault();
        }

        public GetPatientExamQueryResult GetExam(Guid idExam)
        {
            var exam = _context.Connection
                .Query<GetPatientExamQueryResult>("SELECT [Id], [Date], [Type], [Channel] FROM [Exam] WHERE [IdPatient] = @idExam", new { idExam }).FirstOrDefault();

            if(exam != null)
            {
                var frames = ListFrames(exam.Id);
                exam.Frames.AddRange(frames);
            }

            return exam;
        }

        public IEnumerable<ListPatientQueryResult> Get()
        {
            return _context.Connection
                .Query<ListPatientQueryResult>("SELECT [Id], [Name], [PhotoPath], [Phone] FROM [Patient]");
        }

        public IEnumerable<ListPatientExamsQueryResult> ListExams(Guid idPatient)
        {
            return _context.Connection
                .Query<ListPatientExamsQueryResult>("SELECT [Id], [Date], [Type] FROM [Exam] WHERE [IdPatient] = @idPatient", new { idPatient });
        }

        public IEnumerable<ListPatientExamsBitalinoFrameQueryResult> ListFrames(Guid idExam)
        {
            return _context.Connection
               .Query<ListPatientExamsBitalinoFrameQueryResult>("SELECT [Id], [Identifier], [Seq], [A0], [A1], [A2], [A3], [A4], [A5], [D0], [D1], [D2], [D3] FROM [BitalinoFrame] WHERE [IdExam] = @idExam", new { idExam });
        }

        public void Save(Patient patient)
        {
            _context.Connection.Execute(@"INSERT INTO [Patient] ([Id], [Name], [Phone], [PhotoPath], [DateOfBirth])
                                            VALUES (@Id, @Name, @Phone, @PhotoPath, @DateOfBirth)",
                new
                {
                    patient.Id,
                    patient.Name,
                    patient.Phone,
                    patient.PhotoPath,
                    patient.DateOfBirth
                });
        }

        public void SaveExam(Exam exam, Guid idPatient)
        {
            _context.Connection.Execute(@"INSERT INTO [Exam] ([Id], [IdPatient], [Type], [Date], [Channel])
                                            VALUES (@Id, @IdPatient, @Type, @Date, @Channel)",
                new
                {
                    exam.Id,
                    IdPatient = idPatient,
                    exam.Type,
                    exam.Date,
                    exam.Channel
                });

            foreach (var frame in exam.Frames)
            {
                _context.Connection.Execute(@"INSERT INTO [BitalinoFrame] ([Id], [IdExam], [Identifier], [Seq], [A0], [A1], [A2], [A3] ,[A4], [A5], [D0], [D1], [D2], [D3])
                                                VALUES (@Id, @IdExam, @Identifier, @Seq, @A0, @A1, @A2, @A3, @A4, @A5, @D0, @D1, @D2, @D3)",
                    new
                    {
                        IdExam = exam.Id,
                        frame.Id,
                        frame.Identifier,
                        frame.Seq,
                        A0 = frame.GetAnalog(0),
                        A1 = frame.GetAnalog(1),
                        A2 = frame.GetAnalog(2),
                        A3 = frame.GetAnalog(3),
                        A4 = frame.GetAnalog(4),
                        A5 = frame.GetAnalog(5),
                        D0 = frame.GetDigital(0),
                        D1 = frame.GetDigital(1),
                        D2 = frame.GetDigital(2),
                        D3 = frame.GetDigital(3)
                    });
            }
        }
    }
}
