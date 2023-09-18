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
            var students = studentsProvider.GetStudents();

            var result = new StringBuilder();
            foreach (var student in students)
            {
                result.AppendLine($"Student Name: {student.FirstName} {student.SecondName}," +
                    $" Date Of Birth: {student.DateOfBirth.ToShortDateString()}, Address: {student.Address}");
                result.AppendLine($"Student Card: {student.StudentCard.IdNumber}, Date of Issue: {student.StudentCard.DateOfIssue}," +
                    $" Status: {(student.StudentCard.Status ? "Active" : "Inactive")}\n");
            }

            return result.ToString();
        }

        public string GetStudentsByStatus(bool isActive)
        {
            var cards = cardsProvider.GetCards().ToList();

            var filteredStudents = studentsProvider.GetStudents()
                .Where(student => student.StudentCard.Status == isActive)
                .OrderBy(student => student.FirstName)
                .ThenBy(student => student.SecondName)
                .ToList();

            if (filteredStudents.Count == 0)
            {
                return isActive ? "No active students found." : "No inactive students found.";
            }

            string statusText = isActive ? "Active" : "Inactive";
            StringBuilder result = new($"{statusText} Students (sorted by name):\n");
            foreach (var student in filteredStudents)
            {
                result.AppendLine($"Student Name: {student.FirstName} {student.SecondName}," +
                    $" Date Of Birth: {student.DateOfBirth.ToShortDateString()}, Address: {student.Address}");

                result.AppendLine($"Student Card: {student.StudentCard.IdNumber}, Date of Issue: {student.StudentCard.DateOfIssue}\n");
            }

            return result.ToString();
        }

        public string GetStudentsSortedByBirthdate(bool ascending)
        {
            var cards = cardsProvider.GetCards().ToList();

            var sortedStudents = ascending
                ? studentsProvider.GetStudents().OrderBy(student => student.DateOfBirth).ToList()
                : studentsProvider.GetStudents().OrderByDescending(student => student.DateOfBirth).ToList();

            if (sortedStudents.Count == 0)
            {
                return "No students found.";
            }

            string sortOrder = ascending ? "ascending" : "descending";
            StringBuilder result = new($"All Students (sorted by birthdate {sortOrder}):\n");
            foreach (var student in sortedStudents)
            {
                result.AppendLine($"Student Name: {student.FirstName} {student.SecondName}," +
                    $" Date Of Birth: {student.DateOfBirth.ToShortDateString()}, Address: {student.Address}");

                result.AppendLine($"Student Card: {student.StudentCard.IdNumber}, Date of Issue: {student.StudentCard.DateOfIssue}\n");
            }

            return result.ToString();
        }

        public string GetStudentsSortedByName()
        {
            var cards = cardsProvider.GetCards().ToList();

            var sortedStudents = studentsProvider.GetStudents()
                .OrderBy(student => student.FirstName)
                .ThenBy(student => student.SecondName)
                .ToList();

            if (sortedStudents.Count == 0)
            {
                return "No students found.";
            }

            StringBuilder result = new("All Students (sorted by name):\n");
            foreach (var student in sortedStudents)
            {
                result.AppendLine($"Student Name: {student.FirstName} {student.SecondName}," +
                    $" Date Of Birth: {student.DateOfBirth.ToShortDateString()}, Address: {student.Address}");

                result.AppendLine($"Student Card: {student.StudentCard.IdNumber}, Date of Issue: {student.StudentCard.DateOfIssue}\n");
            }

            return result.ToString();
        }

        public string GetStudentsByFirstNameStartingWith(string startsWith)
        {
            var cards = cardsProvider.GetCards().ToList();

            var filteredStudents = studentsProvider.GetStudents()
                .Where(student => student.FirstName.StartsWith(startsWith, StringComparison.OrdinalIgnoreCase))
                .ToList();

            if (filteredStudents.Count == 0)
            {
                return $"No students found with first name starting with '{startsWith}'.";
            }

            StringBuilder result = new($"Students with first name starting with '{startsWith}':\n");
            foreach (var student in filteredStudents)
            {
                result.AppendLine($"Student Name: {student.FirstName} {student.SecondName}," +
                    $" Date Of Birth: {student.DateOfBirth.ToShortDateString()}, Address: {student.Address}");

                result.AppendLine($"Student Card: {student.StudentCard.IdNumber}, Date of Issue: {student.StudentCard.DateOfIssue}\n");
            }

            return result.ToString();
        }

        public string GetStudentsByLastNameStartingWith(string startsWith)
        {
            var cards = cardsProvider.GetCards().ToList();

            var filteredStudents = studentsProvider.GetStudents()
                .Where(student => student.SecondName.StartsWith(startsWith, StringComparison.OrdinalIgnoreCase))
                .ToList();

            if (filteredStudents.Count == 0)
            {
                return $"No students found with last name starting with '{startsWith}'.";
            }

            StringBuilder result = new($"Students with last name starting with '{startsWith}':\n");
            foreach (var student in filteredStudents)
            {
                result.AppendLine($"Student Name: {student.FirstName} {student.SecondName}," +
                    $" Date Of Birth: {student.DateOfBirth.ToShortDateString()}, Address: {student.Address}");

                result.AppendLine($"Student Card: {student.StudentCard.IdNumber}, Date of Issue: {student.StudentCard.DateOfIssue}\n");
            }

            return result.ToString();
        }

        public string GetStudentsWithEmailDomain(string emailDomain)
        {
            var cards = cardsProvider.GetCards().ToList();

            var filteredStudents = studentsProvider.GetStudents()
                .Where(student => student.Email.EndsWith(emailDomain)).ToList();

            if (filteredStudents.Count == 0)
            {
                return $"No students found with email addresses ending in '{emailDomain}'.";
            }

            StringBuilder result = new($"Students with email addresses ending in '{emailDomain}':\n");
            foreach (var student in filteredStudents)
            {
                result.AppendLine($"Student Name: {student.FirstName} {student.SecondName}," +
                    $" Date Of Birth: {student.DateOfBirth.ToShortDateString()}, Address: {student.Address}");

                result.AppendLine($"Student Card: {student.StudentCard.IdNumber}, Date of Issue: {student.StudentCard.DateOfIssue}\n");
            }

            return result.ToString();
        }

        public string GetStudentsByAgeRange(int minAge, int maxAge)
        {
            var cards = cardsProvider.GetCards().ToList();

            var currentDate = DateTime.Now;
            var filteredStudents = studentsProvider.GetStudents()
                .Where(student => currentDate.Year - student.DateOfBirth.Year >= minAge &&
                                  currentDate.Year - student.DateOfBirth.Year <= maxAge)
                .ToList();

            if (filteredStudents.Count == 0)
            {
                return $"No students found within the age range {minAge}-{maxAge}.";
            }

            StringBuilder result = new($"Students within the age range {minAge}-{maxAge}:\n");
            foreach (var student in filteredStudents)
            {
                result.AppendLine($"Student Name: {student.FirstName} {student.SecondName}," +
                    $" Date Of Birth: {student.DateOfBirth.ToShortDateString()}, Address: {student.Address}");

                result.AppendLine($"Student Card: {student.StudentCard.IdNumber}, Date of Issue: {student.StudentCard.DateOfIssue}\n");
            }

            return result.ToString();
        }

        public string GetStudentsBornBeforeYear(int year)
        {
            var cards = cardsProvider.GetCards().ToList();

            var filteredStudents = studentsProvider.GetStudents()
                .Where(student => student.DateOfBirth.Year < year)
                .ToList();

            if (filteredStudents.Count == 0)
            {
                return $"No students found born before the year {year}.";
            }

            StringBuilder result = new($"Students born before the year {year}:\n");
            foreach (var student in filteredStudents)
            {
                result.AppendLine($"Student Name: {student.FirstName} {student.SecondName}," +
                    $" Date Of Birth: {student.DateOfBirth.ToShortDateString()}, Address: {student.Address}");
                result.AppendLine($"Student Card: {student.StudentCard.IdNumber}, Date of Issue: {student.StudentCard.DateOfIssue}," +
                    $" Status: {(student.StudentCard.Status ? "Active" : "Inactive")}\n");
            }

            return result.ToString();
        }

        public string GetStudentsWithLongestName()
        {
            var cards = cardsProvider.GetCards().ToList();

            var students = studentsProvider.GetStudents();

            var longestName = students
                .OrderByDescending(student => (student.FirstName + student.SecondName).Length)
                .FirstOrDefault();

            if (longestName == null)
            {
                return "No students found.";
            }

            StringBuilder result = new("Student with the longest name:\n");
            result.AppendLine($"Student Name: {longestName.FirstName} {longestName.SecondName}," +
                $" Date Of Birth: {longestName.DateOfBirth.ToShortDateString()}, Address: {longestName.Address}");
            result.AppendLine($"Student Card: {longestName.StudentCard.IdNumber}, Date of Issue: {longestName.StudentCard.DateOfIssue}," +
                $" Status: {(longestName.StudentCard.Status ? "Active" : "Inactive")}\n");

            return result.ToString();
        }

        public string GetStudentsBornInMonth(int month)
        {
            var cards = cardsProvider.GetCards().ToList();

            var filteredStudents = studentsProvider.GetStudents()
                .Where(student => student.DateOfBirth.Month == month)
                .ToList();

            if (filteredStudents.Count == 0)
            {
                return $"No students found born in month {month}.";
            }

            StringBuilder result = new($"Students born in month {month}:\n");
            foreach (var student in filteredStudents)
            {
                result.AppendLine($"Student Name: {student.FirstName} {student.SecondName}," +
                    $" Date Of Birth: {student.DateOfBirth.ToShortDateString()}, Address: {student.Address}");
                result.AppendLine($"Student Card: {student.StudentCard.IdNumber}, Date of Issue: {student.StudentCard.DateOfIssue}," +
                    $" Status: {(student.StudentCard.Status ? "Active" : "Inactive")}\n");
            }

            return result.ToString();
        }

        public string GetStudentsWithMostCommonFirstName()
        {
            var cards = cardsProvider.GetCards().ToList();

            var students = studentsProvider.GetStudents();

            var mostCommonFirstName = students
                .GroupBy(student => student.FirstName, StringComparer.OrdinalIgnoreCase)
                .OrderByDescending(group => group.Count())
                .Select(group => group.Key)
                .FirstOrDefault();

            if (mostCommonFirstName == null)
            {
                return "No students found.";
            }

            StringBuilder result = new($"Students with the most common first name '{mostCommonFirstName}':\n");
            var filteredStudents = studentsProvider.GetStudents()
                .Where(student => student.FirstName.Equals(mostCommonFirstName, StringComparison.OrdinalIgnoreCase))
                .ToList();

            foreach (var student in filteredStudents)
            {
                result.AppendLine($"Student Name: {student.FirstName} {student.SecondName}," +
                    $" Date Of Birth: {student.DateOfBirth.ToShortDateString()}, Address: {student.Address}");
                result.AppendLine($"Student Card: {student.StudentCard.IdNumber}, Date of Issue: {student.StudentCard.DateOfIssue}," +
                    $" Status: {(student.StudentCard.Status ? "Active" : "Inactive")}\n");
            }

            return result.ToString();
        }

        public string GetStudentsWithSameFirstNameAndLastName()
        {
            var cards = cardsProvider.GetCards().ToList();

            var filteredStudents = studentsProvider.GetStudents()
                .Where(student => student.FirstName.Equals(student.SecondName, StringComparison.OrdinalIgnoreCase))
                .ToList();

            if (filteredStudents.Count == 0)
            {
                return "No students found with the same first and last name.";
            }

            StringBuilder result = new("Students with the same first and last name:\n");
            foreach (var student in filteredStudents)
            {
                result.AppendLine($"Student Name: {student.FirstName} {student.SecondName}," +
                    $" Date Of Birth: {student.DateOfBirth.ToShortDateString()}, Address: {student.Address}");
                result.AppendLine($"Student Card: {student.StudentCard.IdNumber}, Date of Issue: {student.StudentCard.DateOfIssue}," +
                    $" Status: {(student.StudentCard.Status ? "Active" : "Inactive")}\n");
            }

            return result.ToString();
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

        private List<Student> GetStudents()
        {
            var cards = cardsProvider.GetCards();
            return studentsProvider.GetStudents().ToList();
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
