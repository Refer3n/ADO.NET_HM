using Pract4.DAL.Entities;
using Pract4.DAL.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pract4
{
    public class CardsProvider
    {
        private readonly IRepository<StudentCard> _repository;

        public CardsProvider(IRepository<StudentCard> repository)
        {
            _repository = repository;
        }

        public void AddCards(List<StudentCard> students)
        {
            students.ForEach(student => AddCard(student));
        }

        public void AddCard(StudentCard card)
        {
            _repository.Add(card);
        }

        public void RemoveCards(List<StudentCard> cards)
        {
            cards.ForEach(card => RemoveStudent(card));
        }

        public void RemoveStudent(StudentCard card)
        {
            _repository.Remove(card);
        }

        public void RemoveCard(int Id)
        {
            var card = GetCard(Id);
            _repository.Remove(card);
        }

        public StudentCard GetCard(int Id)
        {
            return _repository.Get(Id);
        }

        public IEnumerable<StudentCard> GetCards()
        {
            return _repository.GetAll();
        }
    }
}
