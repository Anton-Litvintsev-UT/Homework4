using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Homework3.Generics;
using Homework3.Interfaces;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows.Input;

namespace Homework3.Classes
{
    public class ViewModel : ObservableObject
    {
        private SqlRepository<AbstractAnimal> AnimalRepository { get; set; } = new();
        public ObservableCollection<AbstractAnimal> Animals { get; set; } = [];
        private List<string> LogsList { get; set; } = [];
        private Dictionary<Type, object>? Enclosures { get; set; } = null;

        private string _food = "";
        public string Food
        {
            get => _food;
            set
            {
                _food = value;
                OnPropertyChanged(nameof(Food));
            }
        }

        private string _logsText = "";
        public string LogsText
        {
            get => _logsText;
            set
            {
                _logsText = value;
                OnPropertyChanged(nameof(LogsText));
            }
        }

        private AbstractAnimal? _selected;
        public AbstractAnimal? SelectedAnimal
        {
            get => _selected;
            set
            {
                _selected = value;
                OnPropertyChanged(nameof(SelectedAnimal));
                OnPropertyChanged(nameof(IsSelectedAnimal));
            }
        }
        public string SelectedDescription => SelectedAnimal?.Describe() ?? "";
        public string IsSelectedAnimal => SelectedAnimal != null ? "Visible" : "Hidden";

        public ICommand BtnVoiceCommand { get; }
        public ICommand BtnAddAnimalCommand { get; }
        public ICommand BtnFeedCommand { get; }
        public ICommand BtnRemoveAnimalCommand { get; }
        public ICommand LoosingConsciousnessCommand { get; }
        public ICommand AddLogCommand { get; }

        private readonly ILogger _logger;
        public ViewModel(ILogger logger)
        {
            _logger = logger;
            Animals = new ObservableCollection<AbstractAnimal>(AnimalRepository.GetAll());

            // on ItemAdded event add animal to Animals list
            // on ItemRemoved event remove animal to Animals list
            AnimalRepository.ItemAdded += animal => OnAddAnimal(animal);
            AnimalRepository.ItemRemoved += animal => Animals.Remove(animal);

            var jellyfishEnclosure = new Enclosure<Jellyfish>();
            var penguinEnclosure = new Enclosure<Penguin>();
            var whaleEnclosure = new Enclosure<Whale>();

            jellyfishEnclosure.AnimalJoinedInSameEnclosure += j => OnJoinEnclosure(jellyfishEnclosure, j);
            penguinEnclosure.AnimalJoinedInSameEnclosure += j => OnJoinEnclosure(penguinEnclosure, j);
            whaleEnclosure.AnimalJoinedInSameEnclosure += j => OnJoinEnclosure(whaleEnclosure, j);

            jellyfishEnclosure.FoodDropped += j => AddLog(string.Format(Properties.Resource.feeding_happy, j.Name, Food));
            penguinEnclosure.FoodDropped += p => AddLog(string.Format(Properties.Resource.feeding_happy, p.Name, Food));
            whaleEnclosure.FoodDropped += w => AddLog(string.Format(Properties.Resource.feeding_happy, w.Name, Food));

            Enclosures = new Dictionary<Type, object>();
            Enclosures[typeof(Jellyfish)] = jellyfishEnclosure;
            Enclosures[typeof(Penguin)] = penguinEnclosure;
            Enclosures[typeof(Whale)] = whaleEnclosure;

            AddInitAnimals();

            BtnVoiceCommand = new RelayCommand(BtnVoice);
            BtnAddAnimalCommand = new RelayCommand(BtnAddAnimal);
            BtnFeedCommand = new RelayCommand(BtnFeed);
            BtnRemoveAnimalCommand = new RelayCommand(BtnRemoveAnimal);
            LoosingConsciousnessCommand = new RelayCommand(LoosingConsciousness);
            AddLogCommand = new RelayCommand<string?>(msg => AddLog(msg));

            if (Animals.Count > 0)
                SelectedAnimal = Animals[0];
        }

        private void OnJoinEnclosure<T>(Enclosure<T> enclosure, T animal) where T : AbstractAnimal
        {
            var oldMembers = enclosure.GetAll().ToList();
            oldMembers.Remove(animal);

            // everyone except one occurense of animal.Name says hello to Animal.Name
            foreach (var a in oldMembers)
            {
                AddLog($"{a.Name} says hello to {animal.Name}!");
            }
            if (oldMembers.Count > 0)
            {
                AddLog($"{animal.Name} says hello everyone!");
            }
        }

        private void OnAddAnimal(AbstractAnimal animal)
        {
            Animals.Add(animal);

            if (Enclosures == null) return;

            Type animalType = animal.GetType();

            // dynamically put animal to correct enclosure
            if (Enclosures.TryGetValue(animalType, out object enclosure))
            {
                // Add animal to enclosure
                enclosure.GetType().GetMethod("Add")?.Invoke(enclosure, new object[] { animal });

                // Get all animals from enclosure
                var result = enclosure.GetType().GetMethod("GetAll")?.Invoke(enclosure, null) as IEnumerable<AbstractAnimal>;

                if (result != null)
                {
                    string members = string.Join(", ", result.Select(a => a.Name));

                    // show statistics
                    AddLog(string.Format(Properties.Resource.enclosure_update, animalType.Name, result.Count(), members));
                    AddLog($"Enclosure for {animalType.Name} now has {result.Count()} animals.\nMembers: {members}");

                    double avgAge = result.Average(a => a.Age);
                    AddLog($"Average age: {avgAge:F1}");

                    double avgNameLen = result.Average(a => a.Name.Length);
                    AddLog($"Average name length: {avgNameLen:F1}");

                    var allLetters = string.Concat(result.Select(a => a.Name.ToLowerInvariant()));
                    var mostPopularLetter = allLetters
                                            .Where(char.IsLetter)
                                            .GroupBy(c => c)
                                            .OrderByDescending(g => g.Count())
                                            .FirstOrDefault()?.Key;
                    if (mostPopularLetter != null)
                        AddLog($"Most popular letter in names: '{mostPopularLetter}'");
                }
            }
        }

        private void AddInitAnimals()
        {
            AnimalRepository.Add(new Jellyfish("Jell", 9));
            AnimalRepository.Add(new Penguin("Pipo", 20));
            AnimalRepository.Add(new Whale("Sandy", 150));
        }

        private void BtnVoice()
        {
            if (SelectedAnimal is AbstractAnimal animal)
            {
                string sound = animal.MakeSound();
                string logMessage = string.Format(Properties.Resource.make_sound_log, animal.Type, animal.Name, sound);
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

        private void BtnFeed()
        {
            if (SelectedAnimal is AbstractAnimal animal)
            {
                string logMessage;
                if (string.IsNullOrWhiteSpace(Food))
                {
                    logMessage = string.Format(Properties.Resource.teasing, animal.Name);
                    AbstractAnimal.Increment();

                    if (AbstractAnimal.IsTeasingLimit)
                    {
                        AddLog(string.Format(Properties.Resource.teasing_attack, animal.Name));
                        LoosingConsciousness();
                        return;
                    }
                }
                else
                {
                    if (Enclosures != null && Enclosures.TryGetValue(animal.GetType(), out var enclosureObj))
                    {
                        var animals = (enclosureObj.GetType().GetMethod("GetAll")?.Invoke(enclosureObj, null) as IEnumerable<AbstractAnimal>)?.ToList();
                        if (animals == null || animals.Count == 0) return;

                        AddLog(string.Format(Properties.Resource.food_dropped_enclosure, Food, animal.GetType().Name));

                        bool anyFedCorrectly = false;

                        // feed each animal and log rejection if needed
                        foreach (var a in animals)
                        {
                            bool fedCorrectly = a.Feed(Food.Trim());
                            if (fedCorrectly)
                            {
                                anyFedCorrectly = true; // remember at least one animal accepted
                            }
                            else
                            {
                                string rejectLog = string.Format(Properties.Resource.feeding_reject, a.Name, Food);
                                AddLog(rejectLog);
                            }

                            if (a.ShouldActCrazy())
                            {
                                AddLog(a.ActCrazy());
                            }
                        }

                        if (anyFedCorrectly)
                        {
                            _ = (Task)enclosureObj.GetType().GetMethod("DropFoodAsync")!.Invoke(enclosureObj, null)!;
                        }
                        else
                        {
                            bool fedCorrectly = animal.Feed(Food.Trim());

                            if (fedCorrectly)
                            {
                                logMessage = string.Format(Properties.Resource.feeding_happy, animal.Name, Food);
                            }
                            else
                            {
                                logMessage = string.Format(Properties.Resource.feeding_reject, animal.Name, Food);
                            }
                            AddLog(logMessage);
                        }
                    }

                    if (animal.ShouldActCrazy())
                    {
                        AddLog(animal.ActCrazy());
                    }
                }
            }
        }
        private void BtnRemoveAnimal()
        {
            if (SelectedAnimal is AbstractAnimal animal)
            {
                string logMessage;

                if (animal is IFlyable)
                {
                    logMessage = string.Format(Properties.Resource.away_flyable, animal.Name);
                }
                else
                {
                    logMessage = string.Format(Properties.Resource.away_swimmable, animal.Name);
                }

                AddLog(logMessage);
                AnimalRepository.Remove(animal);
            }
        }

        private void BtnAddAnimal()
        {
            var addWindow = new AddAnimalWindow(AnimalRepository.GetAll());

            bool? result = addWindow.ShowDialog();
            if (result == true && addWindow.AddedAnimal != null)
            {
                AbstractAnimal addedAnimal = addWindow.AddedAnimal;
                AnimalRepository.Add(addedAnimal);
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
