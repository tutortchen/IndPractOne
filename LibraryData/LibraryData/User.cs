using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryData
{
    public class User
    {
        public int UserID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string MiddleName { get; set; }
        public int RoleID { get; set; }
        public string Username { get; set; }
        public string UserPassword { get; set; }
    }

    public class UserRole
    {
        public int RoleID { get; set; }
        public string RoleName { get; set; }
    }

    public class ProjectCard
    {
        public int ProjectID { get; set; }
        public string ProjectName { get; set; }
        public string ProjectDescription { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal ProjectBudget { get; set; }
    }

    public class TaskCard
    {
        public int TaskID { get; set; }
        public int ProjectID { get; set; }
        public string TaskName { get; set; }
        public string TaskDescription { get; set; }
        public int AssignedUserID { get; set; }
        public int TaskPriority { get; set; }
        public string TaskStatus { get; set; }
    }

    public class ProjectManagement
    {
        public List<User> Users { get; set; } = new List<User>();
        public List<UserRole> UserRoles { get; set; } = new List<UserRole>();
        public List<ProjectCard> ProjectCards { get; set; } = new List<ProjectCard>();
        public List<TaskCard> TaskCards { get; set; } = new List<TaskCard>();

        public void AddUser(User user) => Users.Add(user);
        public void AddProjectCard(ProjectCard projectCard) => ProjectCards.Add(projectCard);
        public void AddTaskCard(TaskCard taskCard) => TaskCards.Add(taskCard);

        public void AssignTaskToUser(int taskID, int userID)
        {
            var taskCard = TaskCards.Find(t => t.TaskID == taskID);
            if (taskCard != null)
            {
                taskCard.AssignedUserID = userID;
            }
        }

        public void SetTaskPriority(int taskID, int priority)
        {
            var taskCard = TaskCards.Find(t => t.TaskID == taskID);
            if (taskCard != null)
            {
                taskCard.TaskPriority = priority;
            }
        }

        public void SetTaskStatus(int taskID, string status)
        {
            var taskCard = TaskCards.Find(t => t.TaskID == taskID);
            if (taskCard != null)
            {
                taskCard.TaskStatus = status;
            }
        }
    }
}
