using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace stratumbot.Core
{
	/// <summary>
	/// Manually thread stopping by user
	/// </summary>
	class ManuallyStopException : Exception
    {
		/// <summary>
		/// Manually thread stopping by user
		/// </summary>
		public ManuallyStopException(string message)
            : base(message)
        { }
    }

	/// <summary>
	/// Auto thread stopping by bot
	/// </summary>
	class AutoStopException : Exception
	{
		/// <summary>
		/// Auto thread stopping by bot
		/// </summary>
		public AutoStopException(string message)
			: base(message)
		{ }
	}

	/// <summary>
	/// Tiny order (filled amount less than minimals) was canceled
	/// </summary>
	class TinyOrderCanceledException : Exception
	{
		/// <summary>
		/// Auto thread stopping by bot
		/// </summary>
		public TinyOrderCanceledException(string message)
			: base(message)
		{ }
	}

	// Отмена ордера на бирже вручную
	class OrderCanceledException : Exception
    {
        public OrderCanceledException(string message)
            : base(message)
        { }
    }

    // Ордер исполнился пока мы его отменяли
    class OrderFilledWhileWeCancelingException : Exception
    {
        public OrderFilledWhileWeCancelingException(string message)
            : base(message)
        { }
    }

    // Пришел неверный ответ от сервера
    class InvalidJsonException : Exception
    {
        public InvalidJsonException(string message)
            : base(message)
        { }
    }

    // Невалидные параметры стратегии (Пока только указанной пары не существует)
    class InvalidParamException : Exception
    {
        public InvalidParamException(string message)
            : base(message)
        { }
    }
}
