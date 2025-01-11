using Microsoft.Win32;
using PacketGenerator.Model;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;

namespace PacketGenerator;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private List<Field> fields = new List<Field>();
    private int keyIndex = 0;

    public MainWindow()
    {
        InitializeComponent();
    }

    private void AddFieldButton_Click(object sender, RoutedEventArgs e)
    {
        if (string.IsNullOrWhiteSpace(FieldTypeTextBox.Text) || string.IsNullOrWhiteSpace(FieldNameTextBox.Text))
        {
            MessageBox.Show("필드 타입과 이름을 모두 입력하세요.", "입력 오류", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        var field = new Field
        {
            Key = keyIndex,
            DataType = FieldTypeTextBox.Text.Trim(),
            Name = FieldNameTextBox.Text.Trim()
        };

        fields.Add(field);
        FieldsListBox.Items.Add($"[{keyIndex}] {field.DataType} {field.Name}");
        keyIndex++;

        FieldTypeTextBox.Clear();
        FieldNameTextBox.Clear();

        // 자동으로 코드 생성 및 미리보기 반영
        UpdateGeneratedCode();
    }

    private void UpdateFieldButton_Click(object sender, RoutedEventArgs e)
    {
        if (FieldsListBox.SelectedIndex == -1)
        {
            MessageBox.Show("수정할 필드를 선택하세요.", "수정 오류", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        if (string.IsNullOrWhiteSpace(FieldTypeTextBox.Text) || string.IsNullOrWhiteSpace(FieldNameTextBox.Text))
        {
            MessageBox.Show("필드 타입과 이름을 모두 입력하세요.", "입력 오류", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        int selectedIndex = FieldsListBox.SelectedIndex;
        fields[selectedIndex].DataType = FieldTypeTextBox.Text.Trim();
        fields[selectedIndex].Name = FieldNameTextBox.Text.Trim();

        FieldsListBox.Items[selectedIndex] = $"[{fields[selectedIndex].Key}] {fields[selectedIndex].DataType} {fields[selectedIndex].Name}";

        FieldTypeTextBox.Clear();
        FieldNameTextBox.Clear();

        // 자동으로 코드 생성 및 미리보기 반영
        UpdateGeneratedCode();
    }

    private void DeleteFieldButton_Click(object sender, RoutedEventArgs e)
    {
        if (FieldsListBox.SelectedIndex == -1)
        {
            MessageBox.Show("삭제할 필드를 선택하세요.", "삭제 오류", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        int selectedIndex = FieldsListBox.SelectedIndex;
        fields.RemoveAt(selectedIndex);
        FieldsListBox.Items.RemoveAt(selectedIndex);

        FieldTypeTextBox.Clear();
        FieldNameTextBox.Clear();

        // 자동으로 코드 생성 및 미리보기 반영
        UpdateGeneratedCode();
    }

    private void FieldsListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (FieldsListBox.SelectedIndex == -1) return;

        var selectedField = fields[FieldsListBox.SelectedIndex];
        FieldTypeTextBox.Text = selectedField.DataType;
        FieldNameTextBox.Text = selectedField.Name;
    }

    private void GenerateCodeButton_Click(object sender, RoutedEventArgs e)
    {
        if (string.IsNullOrWhiteSpace(ClassNameTextBox.Text))
        {
            MessageBox.Show("클래스 이름을 입력하세요.", "입력 오류", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        if (fields.Count == 0)
        {
            MessageBox.Show("적어도 하나의 필드를 추가해야 합니다.", "입력 오류", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        string generatedCode = GenerateMessagePackClass(ClassNameTextBox.Text, fields);
        GeneratedCodeTextBox.Text = generatedCode;
    }

    private void SaveToFileButton_Click(object sender, RoutedEventArgs e)
    {
        if (string.IsNullOrWhiteSpace(GeneratedCodeTextBox.Text))
        {
            MessageBox.Show("생성된 코드가 없습니다. 먼저 코드를 생성하세요.", "저장 오류", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        SaveFileDialog saveFileDialog = new SaveFileDialog
        {
            Filter = "C# 파일 (*.cs)|*.cs",
            FileName = $"{ClassNameTextBox.Text}.cs"
        };

        if (saveFileDialog.ShowDialog() == true)
        {
            System.IO.File.WriteAllText(saveFileDialog.FileName, GeneratedCodeTextBox.Text);
            MessageBox.Show("파일이 저장되었습니다.", "저장 성공", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }

    private void LoadFromFileButton_Click(object sender, RoutedEventArgs e)
    {
        var openFileDialog = new OpenFileDialog
        {
            Filter = "C# Files (*.cs)|*.cs",
            Title = "Load C# File"
        };

        if (openFileDialog.ShowDialog() == true)
        {
            string fileContent = System.IO.File.ReadAllText(openFileDialog.FileName);
            ParseCsFile(fileContent);
        }
    }

    private void ParseCsFile(string content)
    {
        // 클래스 이름 추출
        var classMatch = Regex.Match(content, @"public\s+class\s+(\w+)");
        if (classMatch.Success)
        {
            ClassNameTextBox.Text = classMatch.Groups[1].Value;
        }
        else
        {
            MessageBox.Show("클래스 이름을 찾을 수 없습니다.", "파일 로드 오류", MessageBoxButton.OK, MessageBoxImage.Error);
            return;
        }

        // 필드 정보 추출
        fields.Clear();
        FieldsListBox.Items.Clear();

        var fieldMatches = Regex.Matches(content, @"\[Key\((\d+)\)\]\s+public\s+(\w+)\s+(\w+)\s*{");
        foreach (Match match in fieldMatches)
        {
            var field = new Field
            {
                Key = int.Parse(match.Groups[1].Value),
                DataType = match.Groups[2].Value,
                Name = match.Groups[3].Value
            };

            fields.Add(field);
            FieldsListBox.Items.Add($"[{field.Key}] {field.DataType} {field.Name}");
        }

        // 키 값 초기화
        keyIndex = fields.Count > 0 ? fields.Max(f => f.Key) + 1 : 0;

        // 코드 미리보기 업데이트
        UpdateGeneratedCode();
    }

    private string GenerateMessagePackClass(string className, List<Field> fields)
    {
        StringBuilder sb = new StringBuilder();

        sb.AppendLine("using MessagePack;");
        sb.AppendLine();
        sb.AppendLine($"[MessagePackObject]");
        sb.AppendLine($"public class {className}");
        sb.AppendLine("{");

        // 필드를 하나씩 추가하면서 불필요한 공백을 방지
        foreach (var field in fields)
        {
            sb.AppendLine($"    [Key({field.Key})]");
            sb.AppendLine($"    public {field.DataType} {field.Name} {{ get; set; }}");
            sb.AppendLine();  // 필드 사이에 공백 추가 (한 줄만 유지)
        }

        sb.AppendLine("}");

        // 공백 줄이 2개 이상 연속해서 나오지 않도록 처리
        var code = sb.ToString();
        code = Regex.Replace(code, @"(\r?\n){2,}", "\n"); // 연속된 두 줄 이상의 공백을 하나로 줄임
        return code.TrimEnd();  // 마지막 공백 제거
    }

    private void UpdateGeneratedCode()
    {
        if (string.IsNullOrWhiteSpace(ClassNameTextBox.Text))
        {
            GeneratedCodeTextBox.Text = "// 클래스 이름을 입력하세요.";
            return;
        }

        if (fields.Count == 0)
        {
            GeneratedCodeTextBox.Text = "";
            return;
        }

        GeneratedCodeTextBox.Text = GenerateMessagePackClass(ClassNameTextBox.Text, fields);
    }
}