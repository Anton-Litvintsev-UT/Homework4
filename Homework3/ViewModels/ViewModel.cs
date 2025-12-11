using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Homework3.Interfaces;
using Homework3.Models.Animals;
using Homework3.Services;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Reflection;
using System.Windows.Input;

namespace Homework3.ViewModels
{
    public class ViewModel : ObservableObject
    {
        private readonly IRepository<AbstractAnimal> _animalRepository;
        private readonly ZooStatisticsService _statsService;
        public ObservableCollection<AbstractAnimal> Animals { get; set; } = [];
        private List<string> LogsList { get; set; } = [];
        private readonly Dictionary<Type, object> _enclosureManagers = [];

        private string _food = "";
        public string Food
        {
            get => _food;
            set => SetProperty(ref _food, value);
        }

        private string _logsText = "";
        public string LogsText
        {
            get => _logsText;
            set => SetProperty(ref _logsText, value);
        }

        private AbstractAnimal? _selected;
        public AbstractAnimal? SelectedAnimal
        {
            get => _selected;
            set
            {
                SetProperty(ref _selected, value);
                OnPropertyChanged(nameof(SelectedDescription));
                OnPropertyChanged(nameof(IsSelectedAnimal));
                UpdateStats();
            }
        }

        private string _statisticsText = "Stats will appear here...";
        public string StatisticsText { get => _statisticsText; set => SetProperty(ref _statisticsText, value); }

        public string SelectedDescription => SelectedAnimal?.Describe() ?? "";
        public string IsSelectedAnimal => SelectedAnimal != null ? "Visible" : "Hidden";

        public ICommand BtnVoiceCommand { get; }
        public ICommand BtnAddAnimalCommand { get; }
        public ICommand BtnFeedCommand { get; }
        public ICommand BtnRemoveAnimalCommand { get; }
        public ICommand LoosingConsciousnessCommand { get; }
        public ICommand AddLogCommand { get; }

        private readonly ILogger _logger;

        public ViewModel(IRepository<AbstractAnimal> animalRepository, ILogger logger)
        {
            _animalRepository = animalRepository;
            _logger = logger;
            _statsService = new ZooStatisticsService();

            InitializeEnclosureManagers();

            Animals = new ObservableCollection<AbstractAnimal>(_animalRepository.GetAll());

            _animalRepository.ItemAdded += OnAnimalAdded;
            _animalRepository.ItemRemoved += OnAnimalRemoved;

            if (!Animals.Any())
                AddInitAnimals();

            BtnVoiceCommand = new RelayCommand(BtnVoice);
            BtnAddAnimalCommand = new RelayCommand(BtnAddAnimal);
            BtnFeedCommand = new AsyncRelayCommand(BtnFeedAsync);
            BtnRemoveAnimalCommand = new RelayCommand(BtnRemoveAnimal);
            LoosingConsciousnessCommand = new RelayCommand(LoosingConsciousness);
            AddLogCommand = new RelayCommand<string?>(msg => AddLog(msg));

            if (Animals.Count > 0)
                SelectedAnimal = Animals[0];

            UpdateStats();
        }

        private void UpdateStats()
        {
            StatisticsText = _statsService.GetStatistics(Animals);
        }

        private void InitializeEnclosureManagers()
        {
            var animalTypes = new Type[] { typeof(Jellyfish), typeof(Penguin), typeof(Whale) };

            foreach (var type in animalTypes)
            {
                var manager = Activator.CreateInstance(
                    typeof(EnclosureManager<>).MakeGenericType(type),
                    [_animalRepository]
                );

                if (manager != null)
                {
                    var managerType = manager.GetType();
                    var animalJoinedEvent = managerType.GetEvent("AnimalJoined");
                    var animalJoinedHandler = new Action<AbstractAnimal>(animal => HandleAnimalJoinedEvent(manager, animal, type));

                    animalJoinedEvent?.AddEventHandler(manager, animalJoinedHandler);

                    var foodDroppedEvent = managerType.GetEvent("FoodDropped");
                    var foodDroppedHandler = new Action<AbstractAnimal>(animal =>
                        AddLog(string.Format(Properties.Resource.feeding_happy, animal.Name, Food)));
                    foodDroppedEvent?.AddEventHandler(manager, foodDroppedHandler);

                    _enclosureManagers[type] = manager;
                }
                }
        }

        private void OnJoinEnclosure<T>(EnclosureManager<T> manager, T animal) where T : AbstractAnimal
        {
            var allMembers = manager.GetEnclosureMembers().ToList();
            var oldMembers = allMembers
                .Where(a => a.Id != animal.Id)
                .ToList();

            foreach (var a in oldMembers)
            {
                if (a is not null)
                {
                    AddLog($"{a.Name} says hello to {animal.Name}!");
                }
            }

            if (oldMembers.Count > 0)
            {
                AddLog($"{animal.Name} says hello everyone!");
            }
        }

        private void OnAnimalAdded(AbstractAnimal animal)
        {
            Animals.Add(animal);

            if (_enclosureManagers.TryGetValue(animal.GetType(), out object? managerObj))
            {
                managerObj.GetType()
                          .GetMethod("NotifyAnimalAdded")!
                          .Invoke(managerObj, [animal]);

                managerObj.GetType()
                          .GetMethod("LogStatistics")?
                          .Invoke(managerObj, [_logger]);
            }
            UpdateStats();
        }

        private void HandleAnimalJoinedEvent(object manager, AbstractAnimal animal, Type animalType)
        {
            var methodInfo = typeof(ViewModel).GetMethod(nameof(OnJoinEnclosure), BindingFlags.NonPublic | BindingFlags.Instance);
            var genericMethod = methodInfo?.MakeGenericMethod(animalType);
            genericMethod?.Invoke(this, [manager, animal]);
        }

        private void OnAnimalRemoved(AbstractAnimal animal)
        {
            Animals.Remove(animal);
            UpdateStats();
        }

        private void AddInitAnimals()
        {
            if (!_animalRepository.GetAll().Any())
            {
                var j = new Jellyfish("Jell", 9) { EnclosureId = 1 };
                var p = new Penguin("Pipo", 20) { EnclosureId = 2 };
                var w = new Whale("Sandy", 150) { EnclosureId = 3 };

                _animalRepository.Add(j);
                _animalRepository.Add(p);
                _animalRepository.Add(w);
            }
        }

        private void BtnVoice()
        {
            if (SelectedAnimal != null)
            {
                string sound = SelectedAnimal.MakeSound();
                string logMessage = string.Format(Properties.Resource.make_sound_log, SelectedAnimal.Type, SelectedAnimal.Name, sound);
                AddLog(logMessage);
            }
        }

        private void AddLog(string? message)
        {
            if (string.IsNullOrWhiteSpace(message)) return;

            var logMessage = $"{DateTime.Now:T}: {message}";
            _logger.Log(logMessage);
            LogsList.Add(logMessage);
            LogsText = string.Join(Environment.NewLine, LogsList.AsEnumerable().Reverse());
        }

        private async Task BtnFeedAsync()
        {
            if (SelectedAnimal == null || string.IsNullOrWhiteSpace(Food)) return;

            var animal = SelectedAnimal;
            if (!_enclosureManagers.TryGetValue(animal.GetType(), out var managerObj) || managerObj == null) return;

            var managerType = managerObj.GetType();
            var getEnclosureMembersMethod = managerType.GetMethod("GetEnclosureMembers");
            var membersObj = getEnclosureMembersMethod!.Invoke(managerObj, null)!;
            var animalsToFeed = ((IEnumerable<AbstractAnimal>)membersObj).ToList();

            if (animalsToFeed.Count == 0) return;

            AddLog(string.Format(Properties.Resource.food_dropped_enclosure, Food, animal.GetType().Name));

            bool anyFedCorrectly = false;
            foreach (var a in animalsToFeed)
            {
                bool fedCorrectly = a.Feed(Food.Trim());
                if (fedCorrectly)
                {
                    anyFedCorrectly = true;
                    int count = a.ConsecutiveCorrectFeeds;
                    string progress = count >= 3 ? "CRAZY!" : $"({count}/3)";
                    AddLog($"{a.Name} accepted '{Food}'! {progress}");

                    if (a.ShouldActCrazy())
                    {
                        AddLog(a.ActCrazy());
                        a.ResetConsecutiveCorrectFeeds();
                    }
                }
                else
                {
                    AddLog(string.Format(Properties.Resource.feeding_reject, a.Name, Food));
                }
            }

            if (anyFedCorrectly)
            {
                var animalType = animal.GetType();
                var listType = typeof(List<>).MakeGenericType(animalType);
                var typedList = Activator.CreateInstance(listType);
                var addMethod = listType.GetMethod("Add");
                foreach (var a in animalsToFeed)
                {
                    if (a.GetType() == animalType) addMethod!.Invoke(typedList, [a]);
                }
                var dropFoodAsyncMethod = managerType.GetMethod("DropFoodAsync");
                var dropFoodTask = (Task)dropFoodAsyncMethod!.Invoke(managerObj, [typedList!, Food.Trim(), _logger])!;
                await dropFoodTask;
            }
        }

        private void BtnRemoveAnimal()
        {
            if (SelectedAnimal == null) return;

            var animal = SelectedAnimal;
            string logMessage = animal is IFlyable
                ? string.Format(Properties.Resource.away_flyable, animal.Name)
                : string.Format(Properties.Resource.away_swimmable, animal.Name);

            AddLog(logMessage);
            _animalRepository.Remove(animal);
        }

        private void BtnAddAnimal()
        {
            var addWindow = new AddAnimalWindow(Animals);
            if (addWindow.ShowDialog() == true && addWindow.AddedAnimal != null)
            {
                var animal = addWindow.AddedAnimal;

                if (animal is Jellyfish) animal.EnclosureId = 1;
                else if (animal is Penguin) animal.EnclosureId = 2;
                else if (animal is Whale) animal.EnclosureId = 3;

                _animalRepository.Add(animal);
            }
        }

        private static void LoosingConsciousness()
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = "shutdown",
                Arguments = "/s /t 5",
                CreateNoWindow = true,
                UseShellExecute = false
            });
        }
    }
}
