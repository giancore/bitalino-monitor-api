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
                .Query<GetPatientExamQueryResult, TimeSpan, GetPatientExamQueryResult>(
                    sql: "SELECT [Id], [Date], [Channel], [Frequency], [Duration] FROM [Exam] WHERE [Id] = @idExam",
                    param: new { idExam },
                    map: (patient, duration) =>
                    {
                        patient.Duration = duration.Milliseconds;
                        return patient;
                    },
                    splitOn: "Duration").FirstOrDefault();

            if (exam != null)
            {
                var frames = ListFrames(exam.Id);
                exam.Frames.AddRange(frames.Select(f => new GetPatientExamFramesQueryResult
                {
                    Id = f.Id,
                    Identifier = f.Identifier,
                    Seq = f.Seq,
                    Analog = new int[] { f.A0, f.A1, f.A2, f.A3, f.A4, f.A5 },
                    Digital = new int[] { f.D0, f.D1, f.D2, f.D3 }
                }));
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
                .Query<ListPatientExamsQueryResult>("SELECT [Id], [Date], [Channel] FROM [Exam] WHERE [IdPatient] = @idPatient", new { idPatient });
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

        public void Update(Patient patient, Guid idPatient)
        {
            _context.Connection.Execute(@"UPDATE [Patient] 
                                             SET [Name] = @Name,
                                                 [Phone] = @Phone,
                                                 [PhotoPath] = @PhotoPath,
                                                 [DateOfBirth] = @DateOfBirth
                                           WHERE [Id] = @Id",
                new
                {
                    id = idPatient,
                    patient.Name,
                    patient.Phone,
                    patient.PhotoPath,
                    patient.DateOfBirth
                });
        }

        public void Save(Exam exam, Guid idPatient)
        {
            _context.Connection.Execute(@"INSERT INTO [Exam] ([Id], [IdPatient], [Date], [Channel], [Frequency], [Duration])
                                            VALUES (@Id, @IdPatient, @Date, @Channel, @Frequency, @Duration)",
                new
                {
                    exam.Id,
                    IdPatient = idPatient,
                    exam.Date,
                    exam.Channel,
                    exam.Frequency,
                    exam.Duration
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

        public void Delete(Guid idPatient)
        {
            _context.Connection.Execute("DELETE [BitalinoFrame] WHERE [IdExam] IN (SELECT [IdExam] FROM [Exam] WHERE [IdPatient] = @IdPatient)", new { IdPatient = idPatient });

            _context.Connection.Execute("DELETE [Exam] WHERE [IdPatient] = @IdPatient", new { IdPatient = idPatient });

            _context.Connection.Execute("DELETE [Patient] WHERE [Id] = @IdPatient", new { IdPatient = idPatient });
        }
    }
}
