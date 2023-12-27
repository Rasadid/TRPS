using System.Net.Sockets;
using System.Net;
using System.Text;

namespace Server
{
    internal class Program
    {
        static void Main(string[] args)
        {
            // Устанавливаем для сокета локальную конечную точку
            IPHostEntry ipHost = Dns.GetHostEntry("localhost");
            IPAddress ipAddr = ipHost.AddressList[0];
            IPEndPoint ipEndPoint = new IPEndPoint(ipAddr, 7777);

            // Создаем сокет Tcp/Ip
            Socket sListener = new Socket(ipAddr.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            // Назначаем сокет локальной конечной точке и слушаем входящие сокеты
            try
            {
                sListener.Bind(ipEndPoint);
                sListener.Listen(10);

                // Начинаем слушать соединения
                while (true)
                {
                    Console.WriteLine("Ожидаем соединение через порт {0}", ipEndPoint);

                    // Программа приостанавливается, ожидая входящее соединение
                    Socket handler = sListener.Accept();
                    string data = null;

                    // Мы дождались клиента, пытающегося с нами соединиться

                    byte[] bytes = new byte[1024];
                    int bytesRec = handler.Receive(bytes);

                    data += Encoding.UTF8.GetString(bytes, 0, bytesRec);

                    // Показываем данные на консоли
                    Console.Write("Полученный текст: " + data + "\n\n");

                    string[] _request = data.Split("_");

                    if (_request[1] == "newUser")
                    {
                        AddNewUser(Convert.ToInt32(_request[0]));
                    }
                    else if (_request[1] == "getBook")
                    {

                    }
                    else if (_request[1] == "setEvaluation")
                    {

                    }

                    // Отправляем ответ клиенту\
                    Random rd = new Random();
                    int _rep = rd.Next(0,21);
                    string reply = _rep.ToString();
                    byte[] msg = Encoding.UTF8.GetBytes(reply);
                    handler.Send(msg);

                    handler.Shutdown(SocketShutdown.Both);
                    handler.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            finally
            {
                Console.ReadLine();
            }
        }

        private static void AddNewUser(int _id)
        {
            Console.WriteLine("Добавляю нового чела");
            using (ApplicationContext db = new ApplicationContext())
            {
                // получаем объекты из бд и выводим на консоль
                var users = db.Users.ToList();

                foreach (User u in users)
                {
                    if (u.UserId == _id)
                    {
                        return;
                    }
                }

                User _newUser = new User { UserId = _id, Preferences = new List<Evaluation>()};
                db.Users.Add(_newUser);
                db.SaveChanges();
            }
        }
    }
}