namespace Laba_3;
using static Laba_3.MainPage;
public partial class EditData : ContentPage
{
    private Dormitories dormitoryedit;
    public EditData(Dormitories dormitoryToEdit)
	{
        this.dormitoryedit = dormitoryToEdit;

        InitializeComponent();
        Info.FontSize = 13;
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
        if (text.Length > 1 && text.Length <= 2)
        {
            if (text.StartsWith("-"))
            {
                text = text.Trim('-');
            }
            while (text.Contains("--"))
            {
                text = text.Replace("--", "-");
            }
        }
        while (text.Contains("--"))
        {
            text = text.Replace("--", "-");
        }
        entry.Text = new String(text.Where(c => Char.IsDigit(c) || c == '-').ToArray());
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
        entry.Text = new String(entry.Text.Where(c => Char.IsLetter(c) || Char.IsWhiteSpace(c) || c == '`').ToArray());
    }
    private async void SaveChangesButton_Clicked(object sender, EventArgs e)
    {
        bool answer = await DisplayAlert("Підтвердіть редагування даннх", "Ви дійсно хочете відредагувати ці дані в таблиці?", "Так", "Ні");
        if (answer)
        {
            dormitoryedit.PersonalInformation = new PersonalInformation(
            FullNameEntry.IsEnabled ? (string.IsNullOrEmpty(FullNameEntry.Text) ? dormitoryedit.PersonalInformation.FullName : FullNameEntry.Text) : dormitoryedit.PersonalInformation.FullName,
            DormitoryNumberEntry.IsEnabled ? (string.IsNullOrEmpty(DormitoryNumberEntry.Text) ? dormitoryedit.PersonalInformation.DormitoryNumber : int.Parse(DormitoryNumberEntry.Text)) : dormitoryedit.PersonalInformation.DormitoryNumber,
            FloorEntry.IsEnabled ? (string.IsNullOrEmpty(FloorEntry.Text) ? dormitoryedit.PersonalInformation.Floor : int.Parse(FloorEntry.Text)) : dormitoryedit.PersonalInformation.Floor,
            RoomNumberEntry.IsEnabled ? (string.IsNullOrEmpty(RoomNumberEntry.Text) ? dormitoryedit.PersonalInformation.RoomNumber : RoomNumberEntry.Text) : dormitoryedit.PersonalInformation.RoomNumber,
            ContractEndDateEntry.IsEnabled ? (string.IsNullOrEmpty(ContractEndDateEntry.Text) ? dormitoryedit.PersonalInformation.ContractEndDate : ContractEndDateEntry.Text) : dormitoryedit.PersonalInformation.ContractEndDate,
            IsResidingInDormitoryPicker.IsEnabled ? (IsResidingInDormitoryPicker.SelectedItem == null ? dormitoryedit.PersonalInformation.IsResidingInDormitory : IsResidingInDormitoryPicker.SelectedItem.ToString()) : dormitoryedit.PersonalInformation.IsResidingInDormitory,
            new AcademicDetails(
                FacultyEntry.IsEnabled ? (string.IsNullOrEmpty(FacultyEntry.Text) ? dormitoryedit.PersonalInformation.AcademicDetails.Faculty : FacultyEntry.Text) : dormitoryedit.PersonalInformation.AcademicDetails.Faculty,
                DepartmentEntry.IsEnabled ? (string.IsNullOrEmpty(DepartmentEntry.Text) ? dormitoryedit.PersonalInformation.AcademicDetails.Department : DepartmentEntry.Text) : dormitoryedit.PersonalInformation.AcademicDetails.Department,
                CoursePicker.IsEnabled ? (CoursePicker.SelectedItem == null ? dormitoryedit.PersonalInformation.AcademicDetails.Course : int.Parse((string)CoursePicker.SelectedItem)) : dormitoryedit.PersonalInformation.AcademicDetails.Course
                                )
            );
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