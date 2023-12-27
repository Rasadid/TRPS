using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Intrinsics.X86;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Bot
{
    internal static class Recommender
    {
        public static List<Book> Recommend(int userId, List<Book> books, List<Evaluation> evaluations)
        {
            // Получить все книги, которые пользователь еще не читал
            var userRatings = evaluations.Where(e => e.User.UserId == userId).ToDictionary(e => e.Book.Id, e => e.Rate);
            var unratedBooks = books.Where(b => !userRatings.ContainsKey(b.Id)).ToList();

            // Получить оценки всех пользователей для каждой книги
            var bookRatings = new Dictionary<int, List<int>>();
            foreach (var evaluation in evaluations)
            {
                if (!bookRatings.ContainsKey(evaluation.Book.Id))
                {
                    bookRatings[evaluation.Book.Id] = new List<int>();
                }
                bookRatings[evaluation.Book.Id].Add(evaluation.Rate);
            }

            // Вычислить средний рейтинг каждой книги
            var bookAverages = new Dictionary<int, double>();
            foreach (var bookRating in bookRatings)
            {
                var averageRating = bookRating.Value.Average();
                bookAverages[bookRating.Key] = averageRating;
            }

            // Вычислить похожие пользователей на основе прошлых оценок
            var similarUsers = new Dictionary<int, double>();
            foreach (var evaluation in evaluations)
            {
                if (evaluation.User.UserId != userId)
                {
                    // Вычислить схожесть пользователей по оценкам
                    var userRatings2 = evaluations.Where(e => e.User.UserId == evaluation.User.UserId).ToDictionary(e => e.Book.Id, e => e.Rate);
                    var commonBooks = userRatings.Keys.Intersect(userRatings2.Keys).ToList();
                    if (commonBooks.Count > 0)
                    {
                        var sumSquares = 0.0;
                        foreach (var bookId in commonBooks)
                        {
                            var diff = userRatings[bookId] - userRatings2[bookId];
                            sumSquares += diff * diff;
                        }
                        var similarity = 1 / (1 + Math.Sqrt(sumSquares));
                        similarUsers[evaluation.User.UserId] = similarity;
                    }
                }
            }

            // Вычислить взвешенные оценки для каждой книги
            var weightedRatings = new Dictionary<int, double>();
            foreach (var similarUser in similarUsers)
            {
                var userRatings2 = evaluations.Where(e => e.User.UserId == similarUser.Key).ToDictionary(e => e.Book.Id, e => e.Rate);
                foreach (var unratedBook in unratedBooks)
                {
                    if (unratedBook.Genres.Intersect(books.First(b => b.Id == unratedBook.Id).Genres).Any())
                    {
                        if (userRatings2.ContainsKey(unratedBook.Id))
                        {
                            var weightedRating = similarUser.Value * (userRatings2[unratedBook.Id] - bookAverages[unratedBook.Id]);
                            if (weightedRatings.ContainsKey(unratedBook.Id))
                            {
                                weightedRatings[unratedBook.Id] += weightedRating;
                            }
                            else
                            {
                                weightedRatings[unratedBook.Id] = weightedRating;
                            }
                        }
                    }
                }
            }

            foreach (var weightedRating in weightedRatings) 
            {
                Console.WriteLine(books.First(b=>b.Id == weightedRating.Key).Title + " Rate: " + weightedRating.Value.ToString());
            }


            // Отсортировать книги по взвешенным оценкам и вернуть наиболее рекомендуемые
            var recommendedBooks = weightedRatings.OrderByDescending(kv => kv.Value)
                                                 .Select(kv => books.First(b => b.Id == kv.Key))
                                                 .ToList();

            return recommendedBooks;
        }
    }
    /*
    public static List<Book> RecommendBooks(int userId, List<Book> allBooks, List<Evaluation> allEvaluations, int neighborCount = 5)
    {
        var userEvaluations = allEvaluations.Where(e => e.User.UserId == userId).ToList();

        var otherEvaluations = allEvaluations.Where(e => e.User.UserId != userId).ToList();

        var similarities = new List<Tuple<int, double>>();

        foreach (var otherUserId in otherEvaluations.Select(e => e.User.UserId).Distinct())
        {
            var otherUserEvaluations = otherEvaluations.Where(e => e.User.UserId == otherUserId).ToList();

            double similarity = CalculateSimilarity(userEvaluations, otherUserEvaluations);

            similarities.Add(Tuple.Create(otherUserId, similarity));
        }

        var neighborUserIds = similarities
            .OrderByDescending(s => s.Item2)
            .Take(neighborCount)
            .Select(s => s.Item1)
            .ToList();

        var recommendedBooks = new List<Book>();

        foreach (var book in allBooks)
        {
            if (userEvaluations.Any(e => e.Book.Id == book.Id))
                continue;

            double weightedRatingSum = 0;
            double similaritySum = 0;

            foreach (var neighborUserId in neighborUserIds)
            {
                var neighborEvaluations = otherEvaluations.Where(e => e.User.UserId == neighborUserId && e.Book.Id == book.Id).ToList();

                if (!neighborEvaluations.Any())
                    continue;

                double neighborSimilarity = similarities.Single(s => s.Item1 == neighborUserId).Item2;
                double neighborRating = neighborEvaluations.Single().Rate;
                double neighborWeight = neighborSimilarity * neighborRating;

                weightedRatingSum += neighborWeight;
                similaritySum += neighborSimilarity;
            }

            if (similaritySum == 0)
                continue;

            double predictedRating = weightedRatingSum / similaritySum;

            if (predictedRating >= 4)
                recommendedBooks.Add(book);
        }

        return recommendedBooks;
    }


    private static double CalculateSimilarity(List<Evaluation> evaluations1, List<Evaluation> evaluations2)
    {
        var commonBooks = evaluations1.Join(evaluations2, e1 => e1.Book.Id, e2 => e2.Book.Id, (e1, e2) => e1);

        if (!commonBooks.Any())
            return 0;

        double avg1 = evaluations1.Average(e => e.Rate);
        double avg2 = evaluations2.Average(e => e.Rate);

        double sum1 = 0, sum2 = 0, sum3 = 0;
        foreach (var commonBook in commonBooks)
        {
            double r1 = commonBook.Rate - avg1;
            double r2 = commonBook.Rate - avg2;
            sum1 += r1 * r2;
            sum2 += r1 * r1;
            sum3 += r2 * r2;
        }

        double denominator = Math.Sqrt(sum2) * Math.Sqrt(sum3);

        if (denominator == 0)
            return 0;

        return sum1 / denominator;
    }
    */
}
    