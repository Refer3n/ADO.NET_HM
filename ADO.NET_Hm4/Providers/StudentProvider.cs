using Pract4.DAL.Entities;
using Pract4.DAL.Repositories;
using System.Collections.Generic;

namespace Pract4
{
    public class StudentsProvider
    {
        private readonly IRepository<Student> _repository;

        public StudentsProvider(IRepository<Student> repository)
        {
            _repository = repository;
        }

        public void AddStudents(List<Student> students)
        {
            students.ForEach(student => AddStudent(student));
        }

        public void AddStudent(Student student)
        {
            _repository.Add(student);
        }

        public void RemoveStudents(List<Student> students)
        {
            students.ForEach(student => RemoveStudent(student));
        }

        public void RemoveStudent(Student student)
        {
            _repository.Remove(student);
        }

        public void RemoveStudent(int Id)
        {
            var student = GetStudent(Id);
            _repository.Remove(student);
        }

        public Student GetStudent(int Id)
        {
            return _repository.Get(Id);
        }

        public IEnumerable<Student> GetStudents()
        {
            return _repository.GetAll();
        }
    }
}
