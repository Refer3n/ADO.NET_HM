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
                result += $"Student Name: {student.FirstName} {student.SecondName}\n";

                result += $"Student Card: {student.StudentCard.IdNumber}, Date of Issue: {student.StudentCard.DateOfIssue}," +
                    $" Status: {(student.StudentCard.Status ? "Active\n" : "Inactive\n")}";

            }

            return result;
        }

        public string AddTestStudent()
        {
            var studentsFromFile = ReadStudentsFromJsonFile();

            if (studentsFromFile == null || studentsFromFile.Count == 0)
            {
                return "No students found in the JSON file.";
            }

            var random = new Random();
            var randomStudent = studentsFromFile[random.Next(studentsFromFile.Count)];

            studentsProvider.AddStudent(randomStudent);

            return "Test student added successfully.";
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
