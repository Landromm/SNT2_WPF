using SNT2_WPF.Models.DataModel;
using SNT2_WPF.Services;
using SNT2_WPF.ViewModel.Base;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace SNT2_WPF.ViewModel.Graph
{
    public class GraphCurrentViewModel : DialogViewModel
    {
		private string? _testText;

		private readonly IUserDialog _userDialog = null!;
		private readonly IMessageBus _messageBus = null!;
		private readonly IDisposable _subscription = null!;

		public string? TestText
        {
				get => _testText;
				set
				{
					_testText = value;
					OnPropertyChanged(nameof(TestText));
				}
		}

		public GraphCurrentViewModel(IUserDialog UserDialog, IMessageBus MessageBus)
      {
			_userDialog = UserDialog;
			_messageBus = MessageBus;
			_subscription = MessageBus.RegisterHandler<Message>(OnReceiveMessage);

		}

		private void OnReceiveMessage(Message message)
		{
			TestText = message.Text;
		}

		public void Dispose() => _subscription.Dispose();

	}
}
