using Newtonsoft.Json;
using stratumbot.Interfaces;
using stratumbot.Models.Filters.DataProvider;
using stratumbot.Models.Logs;
using stratumbot.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace stratumbot.Models.Filters
{
    class EmailNotify : IFilter
    {
        public string ID { get; set; } = "14";
        public string Name { get; set; } = "Email Notify";


        //public DataMiner DataMiner { get; set; } = new DataMiner(); // Данные для расчётов
        // Необходимые типы данных
        //public List<string> RequiredData { get; set; } = new List<string> { };

        // Данные для расчётов
        public DataProvider.DataProvider DataProvider { get; set; } = new DataProvider.DataProvider();
        // Необходимые типы данных
        public List<DataType> RequiredDataTypes { get; set; } = new List<DataType>();

        // Настройки
        public string Login { get; set; } = ""; // Gmail account
        public string Password { get; set; } = ""; // Password
        public string Text { get; set; } = ""; // Title
        public int Duration { get; set; } = 0; // Продолжительность действия

        public string MyName { get; set; } = "Email Norify"; // Отображаемое название 
        public string Group { get; set; } // Группа
        public int Weight { get; set; } = 0; // Балл
        public System.Windows.Media.Brush Color { get; set; } // Цвет

        public string FilterSide { get; set; } // Тип фильтра BUY / SELL

        bool result;
        public bool Result // Результат по фильтру 
        {
            get
            {
                this.Compute();
                return result;
            }
            set
            {
                result = value;
                this.CurrentWeight = (value) ? this.Weight : 0;
            }
        }

        // Получить (а предварительно рассчитать) кол-во баллов по фильтру
        int currentWeight;
        public int CurrentWeight
        {
            get
            {
                this.Compute();
                return currentWeight;
            }
            set { currentWeight = value; }
        }

        // Объект JSON
        class JsonObject
        {
            [JsonProperty("ID")]
            public string ID { get; set; }
            [JsonProperty("Text")]
            public string Text { get; set; }
            [JsonProperty("Duration")]
            public int Duration { get; set; }
        }

        // JSON настроек
        public string Json
        {
            get
            {
                var array = new JsonObject()
                {
                    ID = this.ID,
                    Text = this.Text,
                    Duration = this.Duration
                };

                string json = JsonConvert.SerializeObject(array);

                return json;
            }
        }

        // Конструктор
        public EmailNotify(string login, string password, string text, int duration)
        {
            this.Login = login;
            this.Password = password;
            this.Text = text;
            this.Duration = duration;
            this.Result = false;
        }

        /// <summary>
        /// Add required data type for the filter to the list
        /// </summary>
        public void RequiredDataInit()
        {
            throw new Exception("No required data type for the filter");
        }

        /// <summary>
        /// Get options for specefic data type for the filter
        /// </summary>
        /// <param name="dataType">Data type of which options are needly</param>
        /// <param name="cur1">First currency</param>
        /// <param name="cur2">Second (base) currency</param>
        /// <returns>(DataOptions) options for specific data type</returns>
        public DataOptions GetOptions(DataType dataType, string cur1 = null, string cur2 = null)
        {
            throw new Exception("GetOptions() DataType doesn't sent");
        }

        public void Compute()
        {

            try
            {
                // РАЗРЕШИТЬ ДОСТУП
                // https://myaccount.google.com/lesssecureapps

                // https://mail.google.com/mail/u/0/feed/atom

                System.Net.WebClient objClient = new System.Net.WebClient();
                string response;
                string title;
                string issued;

                //Creating a new xml document
                XmlDocument doc = new XmlDocument();

                //Logging in Gmail server to get data
                objClient.Credentials = new System.Net.NetworkCredential(this.Login, this.Password);
                //reading data and converting to string
                response = Encoding.UTF8.GetString(
                           objClient.DownloadData(@"https://mail.google.com/mail/feed/atom"));

                response = response.Replace(
                     @"<feed version=""0.3"" xmlns=""http://purl.org/atom/ns#"">", @"<feed>");

                //loading into an XML so we can get information easily
                doc.LoadXml(response);

                //nr of emails
                var nr = doc.SelectSingleNode(@"/feed/fullcount").InnerText;

                //Reading the title and the summary for every email
                int count = 0;
                foreach (XmlNode node in doc.SelectNodes(@"/feed/entry"))
                {
                    count++;

                    title = node.SelectSingleNode("title").InnerText;
                    issued = node.SelectSingleNode("issued").InnerText;

                    DateTime received = DateTime.Parse(issued); //  DateTime Parse 

                    if (received.AddSeconds(this.Duration) < DateTime.Now)
                    {
                        if (title == this.Text)
                        {
                            this.Result = true;
                            break;
                        }
                    }
                    else
                        this.Result = false;

                    if (count == 10)
                        break;
                }
            }
            catch (Exception exe)
            {
                Logger.ToFile(exe.ToString());
                Logger.Error("Ошибка фильтра Email Notify");
            }
        }
    }
}
