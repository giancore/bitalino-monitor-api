using BitalinoMonitor.Domain.PatientContext.Entities;
using BitalinoMonitor.Domain.PatientContext.Queries;
using BitalinoMonitor.Domain.PatientContext.Repositories;
using BitalinoMonitor.Infra.DataContexts;
using Dapper;
using DapperLike;
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

        public Patient GetPatient(Guid idPatient)
        {
            string query = "SELECT [Id], [Name], [Phone], [PhotoPath], [DateOfBirth] FROM [Patient] WHERE [Id] = @idPatient";

            return _context.Connection.Query(query, new { idPatient })
                       .Select(row => new Patient(row.Id, row.Name, row.Phone, row.PhotoPath, row.DateOfBirth)).FirstOrDefault();
        }

        public Exam GetExam(Guid idExam)
        {
            string query = "SELECT [Id], [Channel], [Frequency], [Duration], [Date] FROM [Exam] WHERE [Id] = @idExam";

            var frames = GetFrames(idExam);

            return _context.Connection.Query(query, new { idExam })
                       .Select(row => new Exam(row.Id, row.Channel, row.Frequency, row.Duration, row.Date, frames)).FirstOrDefault();
        }

        public IEnumerable<BitalinoFrame> GetFrames(Guid idExam)
        {
            string query = "SELECT [Id], [Identifier], [Seq], [A0], [A1], [A2], [A3], [A4], [A5], [D0], [D1], [D2], [D3], [CreateDate] FROM [BitalinoFrame] WHERE [IdExam] = @idExam ORDER BY [CreateDate]";

            return _context.Connection.Query(query, new { idExam })
                       .Select(row => new BitalinoFrame(row.Id, row.Identifier, row.Seq, row.A0, row.A1, row.A2, row.A3, row.A4, row.A5, row.D0, row.D1, row.D2, row.D3, row.CreateDate));
        }

        public Patient GetPatientByIdExam(Guid idExam)
        {
            string query = @"SELECT [Id], [Name], [PhotoPath], [Phone], [DateOfBirth], [CreateDate] 
                                    FROM [Patient] WHERE [Id] = (SELECT [IdPatient] FROM [Exam] WHERE [Id] = @idExam)";

            return _context.Connection.Query(query, new { idExam })
                        .Select(row => new Patient(row.Id, row.Name, row.Phone, row.PhotoPath, row.DateOfBirth)).FirstOrDefault();
        }

        public GetPatientExamQueryResult GetExamAsQueryResult(Guid idExam)
        {
            var exam = _context.Connection
                .Query<GetPatientExamQueryResult, TimeSpan, GetPatientExamQueryResult>(
                    sql: "SELECT [Id], [Date], [Channel], [Frequency], [Duration] FROM [Exam] WHERE [Id] = @idExam",
                    param: new { idExam },
                    map: (examMap, duration) =>
                    {
                        examMap.Duration = (long)duration.TotalMilliseconds;
                        return examMap;
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

        public IEnumerable<ListPatientQueryResult> GetPatients()
        {
            return _context.Connection
                .Query<ListPatientQueryResult>("SELECT [Id], [Name], [PhotoPath], [Phone] FROM [Patient] ORDER BY [Name]");
        }

        public IEnumerable<ListPatientExamsQueryResult> ListExams(Guid idPatient)
        {
            return _context.Connection
                .Query<ListPatientExamsQueryResult>("SELECT [Id], [Date], [Channel], [Frequency] FROM [Exam] WHERE [IdPatient] = @idPatient ORDER BY [Date] DESC", new { idPatient });
        }

        public IEnumerable<ListPatientExamsBitalinoFrameQueryResult> ListFrames(Guid idExam)
        {
            return _context.Connection
               .Query<ListPatientExamsBitalinoFrameQueryResult>("SELECT [Id], [Identifier], [Seq], [A0], [A1], [A2], [A3], [A4], [A5], [D0], [D1], [D2], [D3] FROM [BitalinoFrame] WHERE [IdExam] = @idExam ORDER BY [Id]", new { idExam });
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
                                            VALUES (@Id, @IdPatient, @Date, @Channel, @Frequency, @DurationAsTimeSpan)",
            new
            {
                exam.Id,
                IdPatient = idPatient,
                exam.Date,
                exam.Channel,
                exam.Frequency,
                exam.DurationAsTimeSpan
            });

            var dto = exam.Frames
                .Select(frame => new
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

            using (var conexao = _context.Connection)
            {
                conexao.BulkInsert(dto, tableName: "BitalinoFrame");

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
