using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Reflection.Metadata.BlobBuilder;

namespace Bot
{
    internal class BotMessages
    {
        public static bool AddNewUser(int _id)
        {
            Console.WriteLine("Добавляю нового чела");
            using (ApplicationContext db = new ApplicationContext())
            {
                var users = db.Users.ToList();

                foreach (User u in users)
                {
                    if (u.UserId == _id)
                    {
                        return false;
                    }
                }

                User _newUser = new User { UserId = _id, Preferences = new List<Evaluation>() };
                db.Users.Add(_newUser);
                db.SaveChanges();
            }
            return true;
        }

        public static MessageForBot GetBook(int _id)
        {
            Console.WriteLine("Получаю книгу");
            Book _currentBook;
            List<Book> recommendedBooks = new List<Book>();

            using (ApplicationContext db = new ApplicationContext())
            {
                var allBooks = db.Books.Include(g=>g.Genres).ToList();
                var allEvaluations = db.Evaluation.Include(e => e.User).Include(b => b.Book).ToList();

                var userEvaluations = allEvaluations.Where(e => e.User.UserId == _id).ToList();

                //recommendedBooks = Recommender.RecommendBooks(_id, allBooks, allEvaluations);
                recommendedBooks = Recommender.Recommend(_id, allBooks, allEvaluations);
            }

            Console.WriteLine("Кол-во рекомендованных книжек: " + recommendedBooks.Count);

            Random recRd = new Random();
            int chance = recRd.Next(0, 3);

            if (recommendedBooks.Count > 0 && (chance == 0 || chance == 1))
            {
                Random rd = new Random();
                int _rep = rd.Next(0, recommendedBooks.Count);
                _currentBook = recommendedBooks[0];
                Console.WriteLine("Это была рекомендация " + _currentBook.Title);

            }
            else
            {
                using (ApplicationContext db = new ApplicationContext())
                {
                    var books = db.Books.Include(g => g.Genres).ToList();
                    Random rd = new Random();
                    int _rep = rd.Next(0, books.Count);
                    _currentBook = books[_rep];
                }
                Console.WriteLine("Это была случайная книга " + _currentBook.Title);
            }

            string genres = "";
            
            List<Genre> genreList = _currentBook.Genres;

            foreach(Genre g in genreList)
            {
                genres += g.Name + ", ";
            }
            
            BotActions._lastBook = _currentBook;
            string _replyMessage = "Название: " + _currentBook.Title + "\n" + "Автор: " + _currentBook.Author + "\n\n" + "Описание: " + _currentBook.Description + "\n\n" + "Жанры: " + genres + "\n" +"Страниц: " + _currentBook.CountPages.ToString();
            return new MessageForBot() {Book = _currentBook, Message = _replyMessage, Image = _currentBook.Picture };
        }

        public static void SetEvaluation(int _id, int rate, Book _book)
        {
            Console.WriteLine("Ставлю оценку книге");

            try
            {
                using (ApplicationContext db = new ApplicationContext())
                {
                    _book = db.Books.FirstOrDefault(b => b.Id == _book.Id);
                    if (_book != null)
                    {
                        Evaluation _evaluation = new Evaluation() { Book = _book, Rate = rate, User = db.Users.FirstOrDefault(u => u.UserId == _id) };
                        List<Evaluation> _evaluationList = db.Evaluation.Include(e => e.Book).Include(u => u.User).ToList();

                        foreach (Evaluation e in _evaluationList)
                        {
                            if (e.User.UserId == _evaluation.User.UserId && e.Book.Id == _evaluation.Book.Id)
                            {
                                e.Rate = rate;
                                db.Evaluation.Update(e);
                                db.SaveChanges();
                                Console.WriteLine("Успешное обновление оценки " + " " +_id.ToString() + " " +rate.ToString() + " " + _book.Title);
                                return;
                            }
                        }
                        /*
                        List<User> _users = db.Users.Include(e => e.Preferences).ToList();
                        User? _currentUser = null;
                        foreach (User _user in _users)
                        {
                            if (_user.UserId == _id)
                            {
                                _currentUser = _user;
                                break;
                            }
                        }
                        */
                        User _currentUser = db.Users.Include(e => e.Preferences).FirstOrDefault(u => u.UserId == _id);
                        _evaluation.User = _currentUser;
                        List<Evaluation> _eval = new List<Evaluation>(_currentUser.Preferences);
                        _eval.Add(_evaluation);
                        _currentUser.Preferences = _eval;
                        db.Users.Update(_currentUser);
                        db.SaveChanges();
                        Console.WriteLine("Успешное добавление оценки " + " " + _id.ToString() + " " + rate.ToString() + " " + _book.Title);

                    }
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine("Ошибка " + " " + _id.ToString() + " " + rate.ToString() + " " + _book.Title);
                Console.WriteLine(ex.ToString());
            }
        }
    }
}
