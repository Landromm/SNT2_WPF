using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SNT2_WPF.Services.Implementations;
public class MessageBusService : IMessageBus
{
	/// <summary>
	/// Класс реализации мягких ссылок для возвращения "Dispose"-объекта, для того чтобы сборщик муссора смог удалять ненужные ссылки.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public class Subscription<T> : IDisposable
	{
		private readonly WeakReference<MessageBusService> _bus;

		public Action<T> Handler
		{
			get;
		}

		public Subscription(MessageBusService bus, Action<T> handler)
		{
			_bus = new(bus);
			this.Handler = handler;
		}

		// Реализация отписки на подписанные сообщения.
		public void Dispose()
		{
			if (!_bus.TryGetTarget(out var bus))
				return;

			var @lock = bus._lock;
			@lock.EnterWriteLock();
			var message_type = typeof(T);
			try
			{
				//Пытаемся получить перечень ссылок на подписки, если не удалось то выходим.
				if (!bus._Subscriptions.TryGetValue(message_type, out var refs))
					return;

				//Берем все ссылки и оставляем те которые еще живы.
				var updated_refs = refs.Where(r => r.IsAlive).ToList();

				//Перебираем перечень ссылок и находим ссылку на текущий объект.
				WeakReference? current_ref = null;
				foreach (var @ref in updated_refs)
					if (ReferenceEquals(@ref.Target, this))
					{
						current_ref = @ref;
						break;
					}
				//Если по каким-то причинам не нашли, то выходим.
				if (current_ref is null)
					return;
				//Если нашли ссылку на текущий объект - удаляем ее.
				updated_refs.Remove(current_ref);
				//Сохраняем изменения в словаре (обновляем состояние словоря подписок).
				bus._Subscriptions[message_type] = updated_refs;
			}
			finally
			{
				@lock.ExitWriteLock();
			}
		}
	}

	/// <summary>
	/// Словарь, который хранит мягкие ссылки по их типу.
	/// </summary>
	private readonly Dictionary<Type, IEnumerable<WeakReference>> _Subscriptions = new();
	/// <summary>
	/// Блокировщик потоков записи данных.
	/// </summary>
	private readonly ReaderWriterLockSlim _lock = new();

	/// <summary>
	/// При регистрации нового обработчика, создает "мягкую" подписку и возвращает ее.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="Handler"></param>
	/// <returns></returns>
	public IDisposable RegisterHandler<T>(Action<T> Handler)
	{
		var subscription = new Subscription<T>(this, Handler);

		_lock.EnterWriteLock();
		try
		{
			var subscription_ref = new WeakReference(subscription);
			var message_type = typeof(T);
			/* Как это работает.
			 * Если в словаре уже имеются подобные подписчики, то
			 * просто добавим в конец перечисления еще одну подписку, а 
			 * если нет - то добавим масив мягких ссылок из одного элемента.
			*/
			_Subscriptions[message_type] = _Subscriptions.TryGetValue(message_type, out var subscriptions)
						 ? subscriptions.Append(subscription_ref)
						 : new[] { subscription_ref };
		}
		finally
		{
			_lock.ExitWriteLock();
		}

		return subscription;
	}

	private IEnumerable<Action<T>>? GetHandlers<T>()
	{
		//Список обработчиков.
		var handlers = new List<Action<T>>();
		var message_type = typeof(T);
		//Флаг для ммертвых ссылок (по умолчанию - все ссылки живые).
		var exist_die_ref = false;

		_lock.EnterReadLock();
		try
		{
			//Если в словаре нет ссылок по указаному типу, то возвращаем null.
			if (!_Subscriptions.TryGetValue(message_type, out var refs)) //refs - перечисление слабых ссылок.
				return null;

			foreach (var @ref in refs)
				if (@ref.Target is Subscription<T> { Handler: var handler })
					handlers.Add(handler);
				else
					exist_die_ref = true;
		}
		finally
		{
			_lock.ExitReadLock();
		}

		if (!exist_die_ref)
			return handlers;

		_lock.EnterWriteLock();
		try
		{
			if (_Subscriptions.TryGetValue(message_type, out var refs))
				if (refs.Where(r => r.IsAlive).ToArray() is { Length: > 0 } new_refs)
					_Subscriptions[message_type] = new_refs;
				else
					_Subscriptions.Remove(message_type);
		}
		finally
		{
			_lock.ExitWriteLock();
		}

		return handlers;
	}

	public void Send<T>(T Message)
	{
		//Берем все обработчики из словаря и посылаем им сообщение.
		/* Как это работает.
		 * Если не удалось получить список обработчиков после вызова метода "GetHendlers" 
		 * то ничего не делаем и прекращаем работу метода.
		 * Если все хорошо то пробегаемся по списку и каждому обработчику
		 * передаем сообщения.
		*/
		if (GetHandlers<T>() is not { } handlers)
			return;

		foreach (var handler in handlers)
			handler(Message);

	}
}
