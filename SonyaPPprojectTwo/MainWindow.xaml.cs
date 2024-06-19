using LibraryData;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SonyaPPprojectTwo
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private ProjectManagement projectManagement = new ProjectManagement();
        private string connectionString;
        private ProjectCard currentProjectCard;
        private TaskCard currentTaskCard;

        public MainWindow()
        {
            InitializeComponent();
            ConnectToDatabase("your_server_name", "ProjectDB"); // Укажите ваш сервер и базу данных
        }

        public void ConnectToDatabase(string serverName, string dbName)
        {
            connectionString = $"Data Source={serverName};Initial Catalog={dbName};Integrated Security=True";
        }

        private void LoadProjectCards()
        {
            projectManagement.ProjectCards.Clear();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string query = "SELECT ProjectID, ProjectName, ProjectDescription, StartDate, EndDate, ProjectBudget FROM ProjectCards";
                SqlCommand command = new SqlCommand(query, connection);
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    projectManagement.ProjectCards.Add(new ProjectCard
                    {
                        ProjectID = reader.GetInt32(0),
                        ProjectName = reader.GetString(1),
                        ProjectDescription = reader.GetString(2),
                        StartDate = reader.GetDateTime(3),
                        EndDate = reader.GetDateTime(4),
                        ProjectBudget = reader.GetDecimal(5)
                    });
                }
            }
            ProjectListBox.ItemsSource = projectManagement.ProjectCards;
        }

        private void LoadTaskCards()
        {
            projectManagement.TaskCards.Clear();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string query = "SELECT TaskID, ProjectID, TaskName, TaskDescription, AssignedUserID, TaskPriority, TaskStatus FROM TaskCards";
                SqlCommand command = new SqlCommand(query, connection);
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    projectManagement.TaskCards.Add(new TaskCard
                    {
                        TaskID = reader.GetInt32(0),
                        ProjectID = reader.GetInt32(1),
                        TaskName = reader.GetString(2),
                        TaskDescription = reader.GetString(3),
                        AssignedUserID = reader.GetInt32(4),
                        TaskPriority = reader.GetInt32(5),
                        TaskStatus = reader.GetString(6)
                    });
                }
            }
            TaskListBox.ItemsSource = projectManagement.TaskCards;
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            string username = UsernameInput.Text;
            string password = UserPasswordInput.Password;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string query = "SELECT UserID, FirstName, LastName, MiddleName, RoleID FROM Users WHERE Username = @username AND UserPassword = @password";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@username", username);
                command.Parameters.AddWithValue("@password", password);
                SqlDataReader reader = command.ExecuteReader();

                if (reader.Read())
                {
                    var user = new User
                    {
                        UserID = reader.GetInt32(0),
                        FirstName = reader.GetString(1),
                        LastName = reader.GetString(2),
                        MiddleName = reader.GetString(3),
                        RoleID = reader.GetInt32(4)
                    };

                    MessageBox.Show($"Login successful. Welcome, {user.FirstName}!");
                    LoadProjectCards();
                    LoadTaskCards();
                }
                else
                {
                    MessageBox.Show("Invalid username or password");
                }
            }
        }

        private void AddProjectButton_Click(object sender, RoutedEventArgs e)
        {
            currentProjectCard = new ProjectCard();
            ShowProjectDialog();
        }

        private void EditProjectButton_Click(object sender, RoutedEventArgs e)
        {
            if (ProjectListBox.SelectedItem is ProjectCard selectedProjectCard)
            {
                currentProjectCard = selectedProjectCard;
                ShowProjectDialog();
            }
        }

        private void AddTaskButton_Click(object sender, RoutedEventArgs e)
        {
            currentTaskCard = new TaskCard();
            ShowTaskDialog();
        }

        private void EditTaskButton_Click(object sender, RoutedEventArgs e)
        {
            if (TaskListBox.SelectedItem is TaskCard selectedTaskCard)
            {
                currentTaskCard = selectedTaskCard;
                ShowTaskDialog();
            }
        }

        private void ShowProjectDialog()
        {
            ProjectNameInput.Text = currentProjectCard.ProjectName;
            ProjectDescriptionInput.Text = currentProjectCard.ProjectDescription;
            ProjectStartDateInput.SelectedDate = currentProjectCard.StartDate;
            ProjectEndDateInput.SelectedDate = currentProjectCard.EndDate;
            ProjectBudgetInput.Text = currentProjectCard.ProjectBudget.ToString();
            ProjectDialogGrid.Visibility = Visibility.Visible;
        }

        private void ShowTaskDialog()
        {
            TaskNameInput.Text = currentTaskCard.TaskName;
            TaskDescriptionInput.Text = currentTaskCard.TaskDescription;
            TaskAssignedUserIDInput.Text = currentTaskCard.AssignedUserID.ToString();
            TaskPriorityInput.Text = currentTaskCard.TaskPriority.ToString();
            TaskStatusInput.Text = currentTaskCard.TaskStatus;
            TaskDialogGrid.Visibility = Visibility.Visible;
        }

        private void SaveProjectButton_Click(object sender, RoutedEventArgs e)
        {
            currentProjectCard.ProjectName = ProjectNameInput.Text;
            currentProjectCard.ProjectDescription = ProjectDescriptionInput.Text;
            currentProjectCard.StartDate = ProjectStartDateInput.SelectedDate ?? DateTime.Now;
            currentProjectCard.EndDate = ProjectEndDateInput.SelectedDate ?? DateTime.Now;
            currentProjectCard.ProjectBudget = decimal.TryParse(ProjectBudgetInput.Text, out decimal budget) ? budget : 0;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                if (currentProjectCard.ProjectID == 0)
                {
                    string query = "INSERT INTO ProjectCards (ProjectName, ProjectDescription, StartDate, EndDate, ProjectBudget) VALUES (@name, @description, @startDate, @endDate, @budget)";
                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@name", currentProjectCard.ProjectName);
                    command.Parameters.AddWithValue("@description", currentProjectCard.ProjectDescription);
                    command.Parameters.AddWithValue("@startDate", currentProjectCard.StartDate);
                    command.Parameters.AddWithValue("@endDate", currentProjectCard.EndDate);
                    command.Parameters.AddWithValue("@budget", currentProjectCard.ProjectBudget);
                    command.ExecuteNonQuery();
                }
                else
                {
                    string query = "UPDATE ProjectCards SET ProjectName = @name, ProjectDescription = @description, StartDate = @startDate, EndDate = @endDate, ProjectBudget = @budget WHERE ProjectID = @id";
                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@name", currentProjectCard.ProjectName);
                    command.Parameters.AddWithValue("@description", currentProjectCard.ProjectDescription);
                    command.Parameters.AddWithValue("@startDate", currentProjectCard.StartDate);
                    command.Parameters.AddWithValue("@endDate", currentProjectCard.EndDate);
                    command.Parameters.AddWithValue("@budget", currentProjectCard.ProjectBudget);
                    command.Parameters.AddWithValue("@id", currentProjectCard.ProjectID);
                    command.ExecuteNonQuery();
                }
            }
            ProjectDialogGrid.Visibility = Visibility.Hidden;
            LoadProjectCards();
        }

        private void SaveTaskButton_Click(object sender, RoutedEventArgs e)
        {
            currentTaskCard.TaskName = TaskNameInput.Text;
            currentTaskCard.TaskDescription = TaskDescriptionInput.Text;
            currentTaskCard.AssignedUserID = int.TryParse(TaskAssignedUserIDInput.Text, out int userId) ? userId : 0;
            currentTaskCard.TaskPriority = int.TryParse(TaskPriorityInput.Text, out int priority) ? priority : 0;
            currentTaskCard.TaskStatus = TaskStatusInput.Text;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                if (currentTaskCard.TaskID == 0)
                {
                    string query = "INSERT INTO TaskCards (ProjectID, TaskName, TaskDescription, AssignedUserID, TaskPriority, TaskStatus) VALUES (@projectId, @name, @description, @assignedUserId, @priority, @status)";
                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@projectId", currentTaskCard.ProjectID);
                    command.Parameters.AddWithValue("@name", currentTaskCard.TaskName);
                    command.Parameters.AddWithValue("@description", currentTaskCard.TaskDescription);
                    command.Parameters.AddWithValue("@assignedUserId", currentTaskCard.AssignedUserID);
                    command.Parameters.AddWithValue("@priority", currentTaskCard.TaskPriority);
                    command.Parameters.AddWithValue("@status", currentTaskCard.TaskStatus);
                    command.ExecuteNonQuery();
                }
                else
                {
                    string query = "UPDATE TaskCards SET TaskName = @name, TaskDescription = @description, AssignedUserID = @assignedUserId, TaskPriority = @priority, TaskStatus = @status WHERE TaskID = @id";
                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@name", currentTaskCard.TaskName);
                    command.Parameters.AddWithValue("@description", currentTaskCard.TaskDescription);
                    command.Parameters.AddWithValue("@assignedUserId", currentTaskCard.AssignedUserID);
                    command.Parameters.AddWithValue("@priority", currentTaskCard.TaskPriority);
                    command.Parameters.AddWithValue("@status", currentTaskCard.TaskStatus);
                    command.Parameters.AddWithValue("@id", currentTaskCard.TaskID);
                    command.ExecuteNonQuery();
                }
            }
            TaskDialogGrid.Visibility = Visibility.Hidden;
            LoadTaskCards();
        }

        private void UsernameInput_GotFocus(object sender, RoutedEventArgs e)
        {
            if (UsernameInput.Text == "Enter Username")
            {
                UsernameInput.Text = "";
                UsernameInput.Foreground = System.Windows.Media.Brushes.Black;
            }
        }

        private void UsernameInput_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(UsernameInput.Text))
            {
                UsernameInput.Text = "Enter Username";
                UsernameInput.Foreground = System.Windows.Media.Brushes.Gray;
            }
        }

        private void UserPasswordInput_GotFocus(object sender, RoutedEventArgs e)
        {
            if (UserPasswordInput.Password == "Enter Password")
            {
                UserPasswordInput.Password = "";
                UserPasswordInput.Foreground = System.Windows.Media.Brushes.Black;
            }
        }

        private void UserPasswordInput_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(UserPasswordInput.Password))
            {
                UserPasswordInput.Password = "Enter Password";
                UserPasswordInput.Foreground = System.Windows.Media.Brushes.Gray;
            }
        }

        private void UserPasswordInput_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (UserPasswordInput.Password == "Enter Password")
            {
                UserPasswordInput.Password = "";
                UserPasswordInput.Foreground = System.Windows.Media.Brushes.Black;
            }
        }
    }
}

