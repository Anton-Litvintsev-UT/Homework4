using Homework3.Models;
using Homework3.Models.Animals;
using System.Windows;
using System.Windows.Controls;


namespace Homework3
{
    public partial class AddAnimalWindow : Window
    {
        private readonly IEnumerable<AbstractAnimal> _animals;
        public AbstractAnimal? AddedAnimal { get; private set; } = null;
        public AddAnimalWindow(IEnumerable<AbstractAnimal> animals)
        {
            InitializeComponent();
            _animals = animals;
            cbAnimalType.SelectedIndex = 0;
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            string name = tbNewName.Text.Trim();
            if (!int.TryParse(tbNewAge.Text.Trim(), out int age) || age < 0)
            {
                MessageBox.Show(Properties.Resource.invalid_age, Properties.Resource.error, MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            AbstractAnimal newAnimal = cbAnimalType.SelectedItem switch
            {
                ComboBoxItem item when item.Content.ToString() == Properties.Resource.jellyfish => new Jellyfish(name, age),
                ComboBoxItem item when item.Content.ToString() == Properties.Resource.penguin => new Penguin(name, age),
                ComboBoxItem item when item.Content.ToString() == Properties.Resource.whale => new Whale(name, age),
                _ => throw new InvalidOperationException(Properties.Resource.invalid_animal_type)
            };

            if (newAnimal != null)
            {
                AddedAnimal = newAnimal;
                DialogResult = true;
                Close();
            }
        }
    }
}
