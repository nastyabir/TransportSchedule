using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TransportSchedule.Data
{
    public delegate void Message(string message, string name);

    public class UserRepository
    {

        public event Message NewMessage;

        public List<User> Users { get; set; }

        public UserRepository()
        {
            Users = Deserialize();
        }

        public void Serialize(List<User> users)
        {
            JsonSerializer serializer = new JsonSerializer();
            using (StreamWriter sw = new StreamWriter("users.txt"))
            {
                using (JsonWriter writer = new JsonTextWriter(sw))
                {
                    serializer.Serialize(writer, users);
                }
            }
        }

        public List<User> Deserialize()
        {
            var users = new List<User>();
            JsonSerializer serializer = new JsonSerializer();
            using (StreamReader sr = new StreamReader("users.txt"))
            {
                using (JsonReader reader = new JsonTextReader(sr))
                {
                    users = serializer.Deserialize<List<User>>(reader);
                }
            }
            return users;
        }

        public bool CheckUserEmail(string email)
        {
            return Users.Any(user => user.Email == email);
        }

        public bool CheckUserPassword(string email, string password)
        {
            var currentUser = Users.First(user => user.Email == email);
            return currentUser.Password == Hasher.GetHash(password);
        }

        public bool CheckUser(string email, string password)
        {
            if (CheckUserEmail(email))
            {
                if (CheckUserPassword(email, password))
                {
                    return true;
                }
                else
                {
                    NewMessage?.Invoke("Неправильный пароль!", "Ошибка");
                }
            }
            else
            {
                NewMessage?.Invoke("Неверный Email!", "Ошибка");
            }
            return false;
        }

        public bool Add(string fullName, string email, string password)
        {
            if (fullName != "")
            {
                if (email != "" && !Users.Any(user => user.Email == email))
                {
                    if (password != "")
                    {
                        Users.Add(new User(fullName, email, password));
                        Serialize(Users);
                        return true;
                    }
                    else
                    {
                        NewMessage?.Invoke("Пустое поле пароля", "Ошибка");
                    }
                }
                else if (email == "")
                {
                    NewMessage?.Invoke("Пустое поле Email", "Ошибка");
                }
                else if (Users.Any(user => user.Email == email))
                {
                    NewMessage?.Invoke("Данный Email занят", "Ошибка");
                }
            }
            else
            {
                NewMessage?.Invoke("Пустое поле имени", "Ошибка");
            }
            return false;
        }

        public bool AddFav(string name, string description)
        {
            if (description != "")
            {
                if (name != "")
                {

                }
                else
                {
                    NewMessage?.Invoke("Станция не выбрана", "Ошибка");
                }
            }
            else
            {
                NewMessage?.Invoke("Пустое поле описания", "Ошибка");
            }
            return false;
        }
  
    }
}
