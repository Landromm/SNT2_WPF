namespace SNT2_WPF.ViewModel.Graph
{
    public class GraphCurrentViewModel : ViewModelBase
    {
        private string _testText;

        public string TestText
        {
            get => _testText;
            set
            {
                _testText = value;
                OnPropertyChanged(nameof(TestText));
            }
        }

        public GraphCurrentViewModel()
        {
            this.TestText = "Не передалось!";
        }

        public GraphCurrentViewModel(string testText) : base()
        {
            this.TestText = testText;
        }



    }
}
