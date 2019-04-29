using BitalinoMonitor.Shared.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BitalinoMonitor.Domain.PatientContext.Entities
{
    public class Patient : Entity
    {
        readonly IList<Exam> _exams;

        public string Name { get; private set; }
        public string Phone { get; private set; }
        public string PhotoPath { get; private set; }
        public DateTime DateOfBirth { get; private set; }
        public IReadOnlyCollection<Exam> Exams => _exams.ToArray();

        public Patient(string name, string phone, DateTime birthdayDate)
        {
            Name = name;
            Phone = phone;
            DateOfBirth = birthdayDate;
        }

        public void AddPhoto(string photoPath)
        {
            PhotoPath = photoPath;
        }

        public void AddExam(Exam exam)
        {
            _exams.Add(exam);
        }

        public override string ToString()
        {
            return $"{Name} - Telefone: {Phone.ToString()} - Data de Nascimento: {DateOfBirth.ToString("dd/MM/yyyy")}";
        }
    }
}
