using Newtonsoft.Json;
using Pract4;
using Pract4.DAL;
using Pract4.DAL.Entities;
using Pract4.DAL.Repositories;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADO.NET_Hm4
{
    public class StudentsDatabase
    {
        private StudentsProvider studentsProvider;
        private CardsProvider cardsProvider;

        private static string JsonFile = "students.json";

        public StudentsDatabase()
        {
            var context = new StudentsContext();
            var repository = new Repository<Student>(context);
            studentsProvider = new StudentsProvider(repository);

            var repositoryCards = new Repository<StudentCard>(context);
            cardsProvider = new CardsProvider(repositoryCards);
        }

        public string GetAllInfo()
        {
            var cards = cardsProvider.GetCards().ToList();

            var students = studentsProvider.GetStudents().ToList();

            string result = "";

            foreach (var student in students)
            {
                result += $"Student Name: {student.FirstName} {student.SecondName}," +
                    $" Date Of Birth: {student.DateOfBirth.ToShortDateString()}, Address: {student.Address}\n";

                result += $"Student Card: {student.StudentCard.IdNumber}, Date of Issue: {student.StudentCard.DateOfIssue}," +
                    $" Status: {(student.StudentCard.Status ? "Active\n" : "Inactive\n")}";

            }

            return result;
        }

        public string AddTestStudent()
        {
            var randomStudent = GetRandomStudent();

            if(randomStudent == null)
            {
                return "No students found in the JSON file.";
            }

            studentsProvider.AddStudent(randomStudent);

            return "Test student added successfully.";
        }

        public string UpdateLastStudent()
        {
            var lastStudent = studentsProvider.GetStudents().LastOrDefault();

            if (lastStudent == null)
            {
                return "No students found to update.";
            }
            var randomStudent = GetRandomStudent();

            if (randomStudent == null)
            {
                return "No students found in the JSON file.";
            }

            lastStudent.FirstName = randomStudent.FirstName;
            lastStudent.SecondName = randomStudent.SecondName;
            lastStudent.Phone = randomStudent.Phone;
            lastStudent.Email = randomStudent.Email;
            lastStudent.DateOfBirth = randomStudent.DateOfBirth;
            lastStudent.Address = randomStudent.Address;
            lastStudent.StudentCard = randomStudent.StudentCard;

            studentsProvider.UpdateStudent(lastStudent);

            return "Last student updated successfully.";
        }

        public string RemoveLastStudent()
        {
            var lastStudent = studentsProvider.GetStudents().LastOrDefault();

            if (lastStudent == null)
            {
                return "No students found to delete.";
            }

            studentsProvider.RemoveStudent(lastStudent);

            return "Last student deleted successfully.";
        }

        private Student GetRandomStudent()
        {
            var studentsFromFile = ReadStudentsFromJsonFile();

            if (studentsFromFile == null || studentsFromFile.Count == 0)
            {
                return null;
            }

            var random = new Random();
            var randomStudent = studentsFromFile[random.Next(studentsFromFile.Count)];

            return randomStudent;
        }

        private List<Student> ReadStudentsFromJsonFile()
        {
            try
            {
                var json = File.ReadAllText(JsonFile);
                var data = JsonConvert.DeserializeObject<ListStudent>(json);
                return data.Students;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }

    class ListStudent
    {
        public List<Student> Students { get; set; }
    }
}
