using System.IO;
using System.Linq;
using System.Text.Json;

namespace Laba_3;

public partial class MainPage : ContentPage
{
    public Dictionary<string, string> filterDormitory = new Dictionary<string, string>();
    ScrollView GridScrollView = new ScrollView() { Orientation = ScrollOrientation.Horizontal };
    StackLayout GridStackLayout = new StackLayout();
    public List<Dormitories> dormitories = new List<Dormitories>();

    public MainPage()
    {
        InitializeComponent();

    }
    public string selectedFilePath = "";
    public class Dormitories
    {
        public PersonalInformation PersonalInformation { get; set; }
        
    }
    public class PersonalInformation
    {
        public string FullName { get; set; }
        public int DormitoryNumber { get; set; }
        public int Floor { get; set; }
        public string RoomNumber { get; set; }
        public string ContractEndDate { get; set; }
        public string IsResidingInDormitory { get; set; }
        public AcademicDetails AcademicDetails { get; set; }
        public PersonalInformation(string fullName, int dormitoryNumber, int floor, string roomNumber, string contractEndDate, string isResidingInDormitory)
        {
            FullName = fullName;
            DormitoryNumber = dormitoryNumber;
            Floor = floor;
            RoomNumber = roomNumber;
            ContractEndDate = contractEndDate;
            IsResidingInDormitory = isResidingInDormitory;
        }
    }

    public class AcademicDetails
    {
        public string Faculty { get; set; }
        public string Department { get; set; }
        public int Course { get; set; }
        public AcademicDetails(string faculty, string department, int course)
        {
            Faculty = faculty;
            Department = department;
            Course = course;
        }
    }

    private void CheckBox_CheckedChanged(object sender, CheckedChangedEventArgs e, Label label, View view)
    {
        if (e.Value)
        {
            label.IsEnabled = e.Value;
            view.IsEnabled = e.Value;
        }
        else
        {
            switch (view)
            {
                case Entry entry:
                    entry.Text = string.Empty;
                    break;
                case Picker picker:
                    picker.SelectedItem = null;
                    break;
            }
            label.IsEnabled = false;
            view.IsEnabled = false;
        }
    }
    private void FullNameBox_CheckedChanged(object sender, CheckedChangedEventArgs e)
    {
        CheckBox_CheckedChanged(sender, e, FullNameLabel, FullNameEntry);
    }
    private void FacultyBox_CheckedChanged(object sender, CheckedChangedEventArgs e)
    {
        CheckBox_CheckedChanged(sender, e, FacultyLabel, FacultyPicker);
    }
    private void DepartmentBox_CheckedChanged(object sender, CheckedChangedEventArgs e)
    {
        CheckBox_CheckedChanged(sender, e, DepartmentLabel, DepartmentPicker);
    }
    void OnLetterEntryTextChanged(object sender, TextChangedEventArgs e)
    {
        var entry = (Entry)sender;
        entry.Text = new String(entry.Text.Where(c => Char.IsLetter(c) || Char.IsWhiteSpace(c)).ToArray());
    }
    private async void LoadPicker(Picker picker, string elementPath)
    {
        try
        {
            List<string> items = new List<string>();
            foreach (var dormitory in dormitories)
            {
                var personalInformation = dormitory.PersonalInformation;
                var academicDetails = personalInformation.AcademicDetails;
                switch (elementPath)
                    {
                        case "Faculty":
                            items.Add(academicDetails.Faculty);
                            break;
                        case "Department":
                            items.Add(academicDetails.Department);
                            break;
                    }
                
            }
            bool canParseToInt = items.All(e => int.TryParse(e, out _));
            var sortedElements = canParseToInt ? items.OrderBy(int.Parse).ToList() : items.OrderBy(x => x).ToList();
            picker.ItemsSource = sortedElements.Distinct().ToList();
        }
        catch (Exception ex) 
        {
            await DisplayAlert("Помилка", $"Сталася помилка: {ex.Message}", "OK");
        }
    }
    private async void ExitButton_Clicked(object sender, EventArgs e)
    {
        bool answer = await DisplayAlert("Підтвердіть вихід", "Ви дійсно хочете вийти?", "Так", "Ні");
        if (answer)
        {
            System.Environment.Exit(0);
        }
    }
    private async void AboutProgramButton_Clicked(object sender, EventArgs e)
    {
        await DisplayAlert("", "©2023. This program was created by the 2nd year student of the Faculty of Computer Science and Cybernetics, group K-27, Oleksandr Vіaltsev, to process files in JSON format. It allows you to open, read, visualize, edit, save, serialize and deserialize JSON files, as well as perform searches by various criteria using LINQ. The program is based on the MAUI template. All rights reserved", "OK");
    }
    private async void PickFileButton_Clicked(object sender, EventArgs e)
    {
        try
        {
            var result = await FilePicker.PickAsync(new PickOptions
            {
                FileTypes = new FilePickerFileType(new Dictionary<DevicePlatform, IEnumerable<string>>
        {
            { DevicePlatform.WinUI, new[] { ".json" } }
        })
            });
            if (result != null)
            {
                Clear();
                selectedFilePath = result.FullPath;
                var fileInfo = new System.IO.FileInfo(selectedFilePath);
                if (fileInfo.Length == 0)
                {
                    SelectedFilePathLabel.IsVisible = false;
                    SearchButton_Enable();
                    await DisplayAlert("Помилка", "Обраний файл є порожнім. Будь ласка, оберіть інший файл", "OK");
                    return;
                }
                SelectedFilePathLabel.IsVisible = true;
                SelectedFilePathLabel.Text = selectedFilePath;
                JSONDeserialize();
                LoadPicker(FacultyPicker, "Faculty");
                LoadPicker(DepartmentPicker, "Department");
                SearchButton_Enable();
                Draw(dormitories);
                await DisplayAlert($"Ви обрали файл: {result.FileName}", "", "OK");
            }
            else
            {
                await DisplayAlert("Відміна вибору", "Ви не обрали файл. Будь ласка, оберіть файл", "OK");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Помилка", $"Сталася помилка: {ex.Message}", "OK");
        }
    }
    private void JSONDeserialize()
    {
        FileStream fs = new FileStream(selectedFilePath, FileMode.Open);
        dormitories = JsonSerializer.Deserialize<List<Dormitories>>(fs);
        fs.Close();
    }
    private void Draw(List<Dormitories> dormitoriesall)
    {
        Grid grid = new Grid
        {
            HorizontalOptions=LayoutOptions.Center,
            BackgroundColor = Colors.Black,
            Margin = new Thickness(0, 0, 5, 0)
        };

        string[] columns = new string[10] { "№", "FullName", "Dorm №", "Floor", "Room №", "ContractEndDate", "IsResidingInDormitory", "Faculty", "Department", "Course" };
        grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(35) });
        grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(30) });
        for (int i = 1; i < columns.Length; i++)
        {
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Auto) });
        }
        for (int i = 0; i < columns.Length; i++)
        {
            var label = new Label { Text = columns[i] };
            AddTitleToGrid(columns[i], i, 0);
        }


        for (int i = 0; i < dormitoriesall.Count; i++)
        {
            var personalInformation = dormitoriesall[i].PersonalInformation;
            var academicDetails = personalInformation.AcademicDetails;

            grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
            string k = (i+1).ToString();
            AddLabelToGrid(k, 0, i+1);
            AddLabelToGrid(personalInformation.FullName, 1, i + 1);
            AddLabelToGrid(personalInformation.DormitoryNumber.ToString(), 2, i + 1);
            AddLabelToGrid(personalInformation.Floor.ToString(), 3, i + 1);
            AddLabelToGrid(personalInformation.RoomNumber, 4, i + 1);
            AddLabelToGrid(personalInformation.ContractEndDate, 5, i + 1);
            AddLabelToGrid(personalInformation.IsResidingInDormitory, 6, i + 1);
            AddLabelToGrid(academicDetails.Faculty, 7, i + 1);
            AddLabelToGrid(academicDetails.Department, 8, i + 1);
            AddLabelToGrid(academicDetails.Course.ToString(), 9, i + 1);
        }
        GridStackLayout.Children.Add(grid);
        GridScrollView.Content = GridStackLayout;
        Main.Children.Add(GridScrollView);
        void AddLabelToGrid(string text, int column, int row)
        {
            var label = new Label { Text = text, FontSize = 13, BackgroundColor = Colors.White, Margin = new Thickness(1, 1, 1, 1), Padding = new Thickness(5, 5, 5, 5) };
            grid.Children.Add(label);
            Grid.SetRow(label, row);
            Grid.SetColumn(label, column);
        }
        void AddTitleToGrid(string text, int column, int row)
        {
            var label = new Label { Text = text, FontFamily = "Impact", FontSize = 14, BackgroundColor = Colors.Beige, Margin = new Thickness(1, 1, 1, 1), Padding = new Thickness(5, 5, 5, 5) };
            grid.Children.Add(label);
            Grid.SetRow(label, row);
            Grid.SetColumn(label, column);
        }
    }
    private void ClearGrid()
    {
        GridStackLayout.Children.Clear();
        GridScrollView.Content = null;
        Main.Children.Remove(GridScrollView);
    }
    private void Clear()
    {
        FullNameBox.IsChecked = false;
        FacultyBox.IsChecked = false;
        DepartmentBox.IsChecked = false;
        ClearGrid();
    }
    private void ClearButton_Clicked(object sender, EventArgs e)
    {
        Clear();
    }
    private void SearchButton_Enable()
    {
        if (selectedFilePath != string.Empty)
        {
            SearchButton.IsEnabled = true;
        }
        else 
        {
            SearchButton.IsEnabled = false;
        }
    }
    private async void SearchButton_Clicked(object sender, EventArgs e)
    {
        try
        {
            ClearGrid();
            AddToFilterDormitory("FullName", FullNameEntry.Text, FullNameEntry.IsEnabled);
            AddToFilterDormitory("Faculty", (string)FacultyPicker.SelectedItem, FacultyPicker.IsEnabled);
            AddToFilterDormitory("Department", (string)DepartmentPicker.SelectedItem, DepartmentPicker.IsEnabled);
            Search();
            filterDormitory.Clear();
        }
        catch (Exception ex)
        {
            await DisplayAlert("Помилка", $"Сталася помилка: {ex.Message}", "OK");
        }
    }
    private void AddToFilterDormitory(string key, string value, bool isEnabled)
    {
        if (!string.IsNullOrEmpty(value) && isEnabled)
        {
            filterDormitory[key] = value;
        }
    }
    private void Search()
    {
        var filteredDormitories = dormitories.Where(dormitory =>
        {
            var personalInformation = dormitory.PersonalInformation;
            var academicDetails = personalInformation.AcademicDetails;

            return filterDormitory.All(filter =>
            {
                if (filter.Key == "FullName")
                {
                    return personalInformation.GetType().GetProperty(filter.Key).GetValue(personalInformation, null).ToString().Contains(filter.Value);
                }
                else if (filter.Key == "Faculty" || filter.Key == "Department")
                {
                    return academicDetails.GetType().GetProperty(filter.Key).GetValue(academicDetails, null).ToString() == filter.Value;
                }
                else
                {
                    return dormitory.GetType().GetProperty(filter.Key).GetValue(dormitory, null)?.ToString() == filter.Value;
                }
            });
        }).ToList();
        if (filteredDormitories.Any())
        {
            Draw(filteredDormitories);
        }
    }
    private void SaveButton_Clicked(object sender, EventArgs e)
    {

    }
    private async void AddDataButton_Clicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new AddData(dormitories));
    }
    protected override void OnAppearing()
    {
        if (selectedFilePath != string.Empty)
        {
            ClearGrid();
            LoadPicker(FacultyPicker, "Faculty");
            LoadPicker(DepartmentPicker, "Department");
            Draw(dormitories);
        }
        base.OnAppearing();
    }
}