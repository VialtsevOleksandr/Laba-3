using Microsoft.Maui.Controls;
using System.Collections.ObjectModel;
using static Laba_3.MainPage;

namespace Laba_3;

public partial class AddData : ContentPage
{
    Dormitories dormitoryadd = new Dormitories();
    private List<Dormitories> dormitories1;
    public AddData(List<Dormitories> dormitories)
    {
        dormitories1 = dormitories;
        InitializeComponent();

        Info.SizeChanged += (s, e) =>
        {
            double start = Info.Width;
            double end = -Info.Width;

            Info.TranslationX = start;

            var animation = new Animation(v => Info.TranslationX = v, start, end);

            animation.Commit(this, "SimpleAnimation", 16, 25000, Easing.Linear, (v, c) => Info.TranslationX = start, () => true);
        };
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

    private void FloorBox_CheckedChanged(object sender, CheckedChangedEventArgs e)
    {
        CheckBox_CheckedChanged(sender, e, FloorLabel, FloorEntry);
    }
    private void FacultyBox_CheckedChanged(object sender, CheckedChangedEventArgs e)
    {
        CheckBox_CheckedChanged(sender, e, FacultyLabel, FacultyEntry);
    }

    private void CourseBox_CheckedChanged(object sender, CheckedChangedEventArgs e)
    {
        CheckBox_CheckedChanged(sender, e, CourseLabel, CoursePicker);
    }

    private void DepartmentBox_CheckedChanged(object sender, CheckedChangedEventArgs e)
    {
        CheckBox_CheckedChanged(sender, e, DepartmentLabel, DepartmentEntry);
    }

    private void DormitoryNumberBox_CheckedChanged(object sender, CheckedChangedEventArgs e)
    {
        CheckBox_CheckedChanged(sender, e, DormitoryNumberLabel, DormitoryNumberEntry);
    }

    private void RoomNumberBox_CheckedChanged(object sender, CheckedChangedEventArgs e)
    {
        CheckBox_CheckedChanged(sender, e, RoomNumberLabel, RoomNumberEntry);
    }

    private void ContractEndDateBox_CheckedChanged(object sender, CheckedChangedEventArgs e)
    {
        CheckBox_CheckedChanged(sender, e, ContractEndDateLabel, ContractEndDateEntry);
    }

    private void IsResidingInDormitoryBox_CheckedChanged(object sender, CheckedChangedEventArgs e)
    {
        CheckBox_CheckedChanged(sender, e, IsResidingInDormitoryLabel, IsResidingInDormitoryPicker);
    }
    void OnNumberEntryTextChanged(object sender, TextChangedEventArgs e)
    {
        var entry = (Entry)sender;
        entry.Text = new String(entry.Text.Where(Char.IsDigit).ToArray());
    }
    void NumberEntryTextChangedWhithPoint(object sender, TextChangedEventArgs e)
    {
        var entry = (Entry)sender;
        string text = entry.Text;
        if (text.Length > 1 && text.Length <=2)
        {
            if (text.StartsWith("."))
            {
                text = text.Trim('.');
            }
            while (text.Contains(".."))
            {
                text = text.Replace("..", ".");
            }
        }
        while (text.Contains(".."))
        {
            text = text.Replace("..", ".");
        }
        entry.Text = new String(text.Where(c => Char.IsDigit(c) || c == '.').ToArray());
    }
    void NumberEntryTextChangedWithSlash(object sender, TextChangedEventArgs e)
    {
        var entry = (Entry)sender;
        string text = entry.Text;
        if (text.Length > 1)
        {
            if (text.StartsWith("/"))
            {
                text = text.TrimStart('/');
            }
            while (text.Contains("//"))
            {
                text = text.Replace("//", "/");
            }
        }
        int slashIndex = text.IndexOf('/');
        if (slashIndex != -1 && slashIndex < text.Length - 2)
        {
            text = text.Remove(slashIndex + 2);
        }
        entry.Text = new String(text.Where(c => Char.IsDigit(c) || c == '/').ToArray());
    }
    void OnLetterEntryTextChanged(object sender, TextChangedEventArgs e)
    {
        var entry = (Entry)sender;
        entry.Text = new String(entry.Text.Where(c => Char.IsLetter(c) || Char.IsWhiteSpace(c)).ToArray());
    }
    private async void SaveChangesButton_Clicked(object sender, EventArgs e)
    {
        bool answer = await DisplayAlert("Підтвердіть додавання в таблицю", "Ви дійсно хочете додати ці дані в таблицю?", "Так", "Ні");
        if (answer)
        {
             dormitoryadd.PersonalInformation = new PersonalInformation(
             FullNameEntry.IsEnabled ? (string.IsNullOrEmpty(FullNameEntry.Text) ? "-" : FullNameEntry.Text) : "-",
             DormitoryNumberEntry.IsEnabled ? (string.IsNullOrEmpty(DormitoryNumberEntry.Text) ? -1 : int.Parse(DormitoryNumberEntry.Text)) : -1,
             FloorEntry.IsEnabled ? (string.IsNullOrEmpty(FloorEntry.Text) ? -1 : int.Parse(FloorEntry.Text)) : -1,
             RoomNumberEntry.IsEnabled ? (string.IsNullOrEmpty(RoomNumberEntry.Text) ? "-" : RoomNumberEntry.Text) : "-",
             ContractEndDateEntry.IsEnabled ? (string.IsNullOrEmpty(ContractEndDateEntry.Text) ? "-" : ContractEndDateEntry.Text) : "-",
             IsResidingInDormitoryPicker.IsEnabled ? (IsResidingInDormitoryPicker.SelectedItem == null ? "-" : IsResidingInDormitoryPicker.SelectedItem.ToString()) : "-"
            );
            dormitoryadd.PersonalInformation.AcademicDetails = new AcademicDetails(
                FacultyEntry.IsEnabled ? (string.IsNullOrEmpty(FacultyEntry.Text) ? "-" : FacultyEntry.Text) : "-",
                DepartmentEntry.IsEnabled ? (string.IsNullOrEmpty(DepartmentEntry.Text) ? "-" : DepartmentEntry.Text) : "-",
                CoursePicker.IsEnabled ? (CoursePicker.SelectedItem == null ? -1 : int.Parse((string)CoursePicker.SelectedItem)) : -1
            );
            dormitories1.Add(dormitoryadd);
            await Navigation.PopAsync();
       }
    }

    private async void CancelChangesButton_Clicked(object sender, EventArgs e)
    {
        bool answer = await DisplayAlert("Підтвердіть вихід", "Ви дійсно хочете вийти? Усі незбережені зміни буде втрачено.", "Так", "Ні");
        if (answer)
        {
            await Navigation.PopAsync();
        }
    }
}