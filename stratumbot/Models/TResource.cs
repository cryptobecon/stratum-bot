using stratumbot.Core;
using stratumbot.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace stratumbot.Models
{
    /// <summary>
    /// Ресурсы для объекта TThread
    /// </summary>
    public static class TResource
    {
        public static Dictionary<int, Tokens> API = new Dictionary<int, Tokens>(); // API кючи для потока
        public static Dictionary<int, Strategy> Strategy = new Dictionary<int, Strategy>(); // Стратегия потока
        public static Dictionary<int, Exchange> Exchange = new Dictionary<int, Exchange>(); // Стратегия потока

        /// <summary>
        /// Метод для бронирования потоком API ключа. Вернёт null, если нет ключа свободного или в принципе.
        /// </summary>
        public static Tokens GetReservedAPI(Exchange _exchange, int _tid)
        {
            // Перебираем все доступные API ключи
            foreach (var exchangeAPI in Settings.API)
            {
                if (exchangeAPI.Exchange == _exchange) // список API токенов для данной бирже
                {
                    // Перебираем все ключи по нужной бирже
                    foreach (var token in exchangeAPI.Tokens)
                    {
                        // Перебирем все уже распределенные токены
                        bool founded = false; // Флан, который покажет найден ли ключ в списке уже забронированых
                        foreach (var api in API)
                        {
                            if (token == api.Value)
                            {
                                founded = true; // Текущий токен найден в списке забронированных
                                break; // Прекращаем искать его по списку
                            }
                        }
                        if(!founded) // Если токен не был найден в списке забронированных
                        {
                            API[_tid] = token; // Бронируем токен за данным потоком
                            return token;
                        }
                    }
                    return null; // Для данной биржи не оказалось свободных токенов
                }
            }
            return null; // Токенов нет вообще
        }

        /// <summary>
        /// Метод возвращает API токен для потока. Если для потока нет токена то вернёт null.
        /// </summary>
        public static Tokens GetAPI(int _tid = -1)
        {
            int tid = TID.CurrentID;
            if (_tid != -1)
                tid = _tid;

            foreach (var api in API)
            {
                if (api.Key == tid)
                    return api.Value;
            }

            return null; // В списке забронированных нет ключа для данного потока
        }

        /// <summary>
        /// Метод для освобождения API ключа при остановке работы потока
        /// </summary>
        public static void UnreservedAPI(int _tid)
        {
            API.Remove(_tid);
        }
    }
}
